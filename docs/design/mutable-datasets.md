# Design Proposal: Mutable DICOM Datasets and Multi-Modal Parser API

## Problem Statement

The current parser always produces a `ReadOnlyDicomDataset`: a struct that holds raw
`ReadOnlyMemory<byte>` slices and decodes them lazily on each `TryGetXxx()` call.  This
is excellent for **read-only** workloads, but three common use-cases are not yet served:

1. **Read-then-edit** — parse a DICOM file from disk and modify one or more tags before
   writing the result back (e.g. anonymization, worklist correction).
2. **In-memory construction** — build a `DicomDataset` from scratch in C# without
   round-tripping through bytes (unit testing, synthetic data generation).
3. **Selective partial parse** — stop parsing early once the required tags have been read
   (`StopParsing` already exists) while still being able to get a dataset that knows about
   only those tags.

The core tension is that items naturally exist in two states:

| State | Representation | Source |
|---|---|---|
| **Raw** | `ReadOnlyMemory<byte>` slice into the rented `ArrayPool` buffer | File / network |
| **Parsed** | Strongly-typed `IDicomItem` (`DicomDate`, `DicomPersonName`, …) | In-memory construction or after editing |

An ideal model keeps items in the raw state until they are needed (lazy decode), and allows
individual items to be replaced with parsed values without re-encoding the whole dataset.

---

## Background: Current Architecture

```
DicomParser (System.IO.Pipelines, async state-machine)
  └─► ReadOnlyDicomDataset   (SortedDictionary<uint, ReadOnlyDicomItem>)
           ├─ ReadOnlyDicomItem  { Group, Element, VR, ReadOnlyDicomItemContent }
           │       ReadOnlyDicomItemContent = Memory<byte> | Fragments | SequenceItems
           └─ Lazy decode on TryGetXxx()  (delegates to VR parsers via DicomValueParser)

DicomDataset  (mutable, SortedDictionary<uint, IDicomItem>)   ← partially started
  └─ IDicomItem  ← DicomItem<TValue>  ← DicomDate, DicomPersonName, …
```

The parser is tightly coupled to producing `ReadOnlyDicomDataset`.  The `DicomDataset`
class exists but has no constructor from a parsed dataset (it throws
`NotSupportedException`) and `ToDicomDataset()` on `ReadOnlyDicomDataset` is a stub.

---

## Use-Case Matrix

| Use-case | Needs raw bytes? | Needs `StopParsing`? | Needs mutation? | Needs construction? |
|---|---|---|---|---|
| Extract a single tag | ✓ (perf) | ✓ | — | — |
| Full read-only scan | ✓ | — | — | — |
| Anonymize (read+edit) | ✓ (lazy) | — | ✓ | — |
| Unit test / synthetic data | — | — | ✓ | ✓ |
| Write back to file | — | — | ✓ | ✓ |

---

## Option A — SAX-style Parser with Pluggable Dataset Builders

### Idea

Decouple the parser's byte-scanning loop from the object it produces by introducing an
event-handler interface (analogous to SAX in XML):

```csharp
public interface IDicomDatasetHandler
{
    void OnItem(ushort group, ushort element, DicomVR vr, ReadOnlyMemory<byte> rawValue);
    void OnSequenceStart(ushort group, ushort element);
    void OnSequenceItemStart();
    void OnSequenceItemEnd();
    void OnSequenceEnd();
    void OnFragmentsStart(ushort group, ushort element, DicomVR vr);
    void OnFragment(ReadOnlyMemory<byte> fragment);
    void OnFragmentsEnd();
    void OnEncoding(Encoding encoding);
}
```

The parser calls these methods as it streams through the bytes.  Callers supply any
implementation of `IDicomDatasetHandler` they like:

```csharp
// Existing fast path — raw bytes, no decode
var handler = new ReadOnlyDicomDatasetHandler();
await parser.ParseAsync(file, handler, options, ct);
ReadOnlyDicomDataset result = handler.Result;

// New path — eagerly decode to IDicomItem
var handler = new DicomDatasetHandler(valueParser);
await parser.ParseAsync(file, handler, options, ct);
DicomDataset result = handler.Result;

// Streaming / analytics path — no materialization at all
var handler = new MyCountingHandler();
await parser.ParseAsync(file, handler, options, ct);
```

The public-facing `IDicomParser` gains overloads:

```csharp
Task<ReadOnlyDicomDataset> ParseReadOnlyAsync(FileInfo file, DicomParserOptions? options = null, CancellationToken ct = default);
Task<DicomDataset>         ParseAsync        (FileInfo file, CancellationToken ct = default);
Task                       ParseAsync        (FileInfo file, IDicomDatasetHandler handler, DicomParserOptions? options = null, CancellationToken ct = default);
```

`DicomDataset` is the mutable type for both the "read+edit" and "in-memory construction"
use-cases.  It holds only `IDicomItem` objects (fully decoded):

```csharp
var ds = new DicomDataset();                                    // construction
ds.Add(DicomTags.PatientName, new PersonName("Doe^John"));

var ds = await parser.ParseAsync(file, ct);                     // read+edit
ds.Set(DicomTags.AccessionNumber, "NEW001");
ds.Remove(DicomTags.PatientBirthDate);
```

### Pros

- Strict separation of concerns: the parser knows nothing about the output type.
- Ultra-flexible: any downstream system can consume events without materializing anything.
- `StopParsing` continues to work exactly as today — the handler simply never receives
  events after the stop tag.
- Easy to add a `DicomDatasetHandler` that lazily decodes (see Option B below) without
  changing the parser at all.
- No changes to `ReadOnlyDicomDataset` — the fast read-only path is completely untouched.

### Cons

- The parser's internal `DicomParseState` currently writes directly into
  `ReadOnlyDicomDataset`; refactoring it to call the handler interface requires a
  moderate-sized rewrite of the `Parse()` state machine.
- The handler interface is low-level — callers who want a simple `DicomDataset` from a
  file have to know to use the right overload.
- Memory lifetime for `rawValue` passed to `OnItem` is only guaranteed for the duration of
  the callback — implementations that want to keep raw bytes must copy them (current
  behaviour already does this into `ArrayPool`-backed `DicomMemory`).

---

## Option B — Dual-State Item: Raw/Parsed Union in `DicomDataset`

### Idea

`DicomDataset` holds items that can be in *either* the raw state (bytes) **or** the parsed
state (typed value), with lazy promotion on first access:

```csharp
internal sealed class DicomDatasetEntry
{
    private ReadOnlyMemory<byte> _rawBytes;
    private IDicomItem?          _parsed;   // null = not yet decoded

    public DicomVR VR { get; }
    public ushort  Group { get; }
    public ushort  Element { get; }

    public IDicomItem GetOrDecode(DicomValueParser valueParser)
    {
        if (_parsed is not null) return _parsed;
        _parsed = valueParser.Parse(Group, Element, VR, _rawBytes);
        return _parsed;
    }
}

public sealed class DicomDataset
{
    private SortedDictionary<uint, DicomDatasetEntry> _entries;
    private DicomMemories _memories;   // keeps ArrayPool buffers alive
    private DicomValueParser _valueParser;

    // When you edit a tag, just replace the entry with a parsed-state one
    public void Set(DicomTag tag, string value) { ... }
    // Disposing the dataset returns the ArrayPool buffers
}
```

The parser builds a `DicomDataset` by creating raw-state entries (copying bytes into
`ArrayPool` blocks as today).  Accessor methods (`TryGetString`, `TryGetDate`, …) trigger
lazy decode.  Editing a tag replaces the entry with a parsed-state one; the raw bytes for
that tag are abandoned (they will be freed when `_memories` is disposed).

### Pros

- Zero extra decode cost for tags that are never accessed after loading.
- Avoids keeping two separate types (`ReadOnlyDicomDataset` vs `DicomDataset`) for the
  "read from file" scenario — a single `DicomDataset` covers both read and read+edit.
- The pool-based memory model that already exists for `ReadOnlyDicomDataset` transfers
  directly.

### Cons

- `DicomDataset` becomes a significantly more complex type: it must carry `DicomValueParser`
  and `DicomMemories` as fields, making DI wiring more involved.
- The lazy-decode mutation (`_parsed = ...`) means `DicomDatasetEntry` is not thread-safe
  by default; readers would need locking or `Interlocked` if concurrent reads are expected.
- "Abandoning" raw bytes of edited tags means `DicomMemories` buffers are not reclaimed
  until the whole dataset is disposed — small memory overhead for anonymization workloads
  that replace many tags.
- The distinction between "raw-state" and "parsed-state" entries leaks into the serializer,
  which needs to handle both.
- `DicomDataset` can no longer be a simple `sealed record` — it needs to be a `class` to
  allow the mutable lazy state.

---

## Option C — Immutable Dataset with Structural Sharing (Functional Style)

### Idea

Keep `ReadOnlyDicomDataset` as the only parsed type.  Mutations return a *new* dataset
that shares all unmodified entries with the original:

```csharp
// Returns a new dataset with the one tag replaced; all other items share raw bytes
ReadOnlyDicomDataset edited = original.With(DicomTags.PatientName, new PersonName("Doe^John"));
ReadOnlyDicomDataset edited = original.Without(DicomTags.PatientBirthDate);
```

Internally, the new dataset's `SortedDictionary` is built by copying all entries from the
original and replacing / removing the targeted entry.  A modified item is stored as a
special `ReadOnlyDicomItem` whose `Content.Memory` is the re-encoded byte form of the
new value.

For in-memory construction, a builder pattern:

```csharp
var builder = new DicomDatasetBuilder();
builder.Add(DicomTags.PatientName, new PersonName("Doe^John"));
ReadOnlyDicomDataset ds = builder.Build();
```

### Pros

- No new mutable type — `ReadOnlyDicomDataset` stays the canonical, safe-to-share type.
- Thread-safe by construction — sharing raw-byte slices between the original and the edited
  copy is safe because both are read-only.
- Mental model is very simple.

### Cons

- Every `With()` call copies the entire `SortedDictionary` — O(n) cost even for a single
  tag change.  For anonymization (tens of tags edited) this means O(n × k) total work.
- Requires encoding typed values *back to bytes* just to store them in a `ReadOnlyDicomItem`
  — this demands a full DICOM serializer even for simple in-memory edits.
- Sequences are deep structures; structural sharing at the item level is straightforward but
  sharing inside sequence items requires more thought.
- The codebase would need a serializer before `With()` could work, introducing a large
  dependency for a seemingly simple feature.

---

## Option D — Parser API Split: Two Methods, Two Types (Minimal Change)

### Idea

Formalise the already-started split: `ReadOnlyDicomDataset` for read-only, `DicomDataset`
for mutable.  The parser gains a second entry point that eagerly decodes everything:

```csharp
// Existing — raw bytes, lazy decode, StopParsing supported
Task<ReadOnlyDicomDataset> ParseReadOnlyAsync(FileInfo file, DicomParserOptions? options = null, CancellationToken ct = default);

// New — eager decode, returns fully-typed IDicomItem values
Task<DicomDataset> ParseAsync(FileInfo file, CancellationToken ct = default);
```

`DicomDataset` only ever holds `IDicomItem` objects (parsed values, never raw bytes).
Implementing `ParseAsync` is essentially `ParseReadOnlyAsync` followed by calling
`ToDicomDataset()` — the stub that already exists on `ReadOnlyDicomDataset`.

```csharp
// In-memory construction remains simple
var ds = new DicomDataset();
ds.Add(DicomTags.PatientName, new PersonName("Doe^John"));

// Edit after parsing
var ds = await parser.ParseAsync(file, ct);
ds.Set(DicomTags.AccessionNumber, "NEW001");
```

### Pros

- Lowest risk: `ReadOnlyDicomDataset` and the fast read path are untouched.
- `DicomDataset` stays a simple, straightforward type with no raw-byte complexity.
- In-memory construction already works today (`DicomDataset.Add()`).
- `ToDicomDataset()` is already stubbed out — just needs to be filled in.

### Cons

- `ParseAsync` eagerly decodes *all* tags even if only a few will be edited — potentially
  wasteful for large datasets (pixel data, large sequences).
- Two separate parse paths in the code share little logic, which can lead to drift.
- `StopParsing` has no equivalent for `ParseAsync` (you get everything or nothing).
- There is no memory-pool benefit for `DicomDataset` — all values are heap-allocated
  managed objects.

---

## Recommendation

The four options are not mutually exclusive.  A pragmatic roadmap:

### Phase 1 — Formalise the easy wins (Option D, low risk)

1. Complete `ToDicomDataset()` in `ReadOnlyDicomDataset.ToDicomDataset.cs`.
2. Add `ParseAsync(FileInfo) → Task<DicomDataset>` to `IDicomParser`.
3. Fill in `DicomDataset(ReadOnlyDicomDataset)` constructor using the completed converter.
4. Add `Set()` / `Remove()` / `Replace()` mutations to `DicomDataset`.
5. Covers: in-memory construction ✓, read+edit (eager) ✓.

### Phase 2 — SAX decoupling (Option A, higher reward)

1. Introduce `IDicomDatasetHandler`.
2. Refactor `DicomParser` to call the handler instead of writing directly into
   `ReadOnlyDicomDataset`.
3. `ReadOnlyDicomDatasetHandler` wraps the current behaviour exactly — zero regression.
4. `DicomDatasetHandler` produces a `DicomDataset` via eager decode.
5. Covers: streaming / analytics workloads ✓, future lazy-decode handler ✓.

### Phase 3 — Lazy-decode `DicomDataset` (Option B, optional perf optimisation)

If profiling shows that eager decode in Phase 1 is a bottleneck for large anonymization
workloads, introduce the dual-state entry described in Option B *inside* `DicomDataset`
without changing its public API.  The Phase 1 and Phase 2 APIs are preserved.

### What is intentionally out of scope

- Option C (immutable-with-copy mutations) is ruled out because it requires a serializer
  (bytes → bytes round-trip) before any editing can work at all, and it has poor
  performance characteristics for bulk-edit use-cases.
- A DICOM serializer / writer is a natural next step after Phase 1 but is a separate
  design.

---

## Appendix: Sketch of Public API after Phase 1 + 2

```csharp
// ── Parser ────────────────────────────────────────────────────────────────────

public interface IDicomParser
{
    // Fast, read-only.  StopParsing supported.
    Task<ReadOnlyDicomDataset> ParseReadOnlyAsync(
        FileInfo file,
        DicomParserOptions? options = null,
        CancellationToken cancellationToken = default);

    // Eagerly decoded, mutable.
    Task<DicomDataset> ParseAsync(
        FileInfo file,
        CancellationToken cancellationToken = default);

    // SAX / event-based.  Supply your own handler.
    Task ParseAsync(
        FileInfo file,
        IDicomDatasetHandler handler,
        DicomParserOptions? options = null,
        CancellationToken cancellationToken = default);
}

// ── Mutable dataset ───────────────────────────────────────────────────────────

public sealed partial class DicomDataset
{
    public DicomDataset() { }
    public DicomDataset(ReadOnlyDicomDataset source) { ... }   // eager decode

    // Adding items
    public void Add(IDicomItem item);
    public void Add(DicomTag tag, string value);
    public void Add(DicomTag tag, int value);
    // … (existing overloads)

    // Updating items (upsert)
    public void Set(DicomTag tag, string value);
    public void Set(DicomTag tag, int value);
    // …

    // Removing items
    public bool Remove(DicomTag tag);

    // Reading items (same TryGetXxx pattern as ReadOnlyDicomDataset)
    public bool TryGetString(DicomTag tag, out string? value);
    // …
}

// ── SAX handler ───────────────────────────────────────────────────────────────

public interface IDicomDatasetHandler
{
    void OnItem(ushort group, ushort element, DicomVR vr, ReadOnlyMemory<byte> rawValue);
    void OnSequenceStart(ushort group, ushort element);
    void OnSequenceItemStart();
    void OnSequenceItemEnd();
    void OnSequenceEnd();
    void OnFragmentsStart(ushort group, ushort element, DicomVR vr);
    void OnFragment(ReadOnlyMemory<byte> fragment);
    void OnFragmentsEnd();
    void OnEncoding(System.Text.Encoding encoding);
}
```
