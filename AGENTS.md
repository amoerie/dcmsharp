# DCMSharp — Agent Guide

## Project Overview

DCMSharp is a high-performance .NET 9 library for parsing DICOM (Digital Imaging and Communications in Medicine) files, plus three CLI tools:

| Tool | Command | Purpose |
|---|---|---|
| DcmFind | `dcmfind` | Search DICOM files by tag criteria |
| DcmAnonymize | `DcmAnonymize` | Anonymize patient and pixel data |
| DcmOrganize | `dcmorganize` | Reorganize files into folder structures |

**GitHub:** https://github.com/amoerie/dcmsharp  
**License:** MIT  
**Author:** Alexander Moerman

---

## Repository Layout

```
DcmSharp.sln
├── src/
│   ├── DcmSharp/                    Core parsing library
│   ├── DcmSharp.SourceGenerator/    Compile-time DICOM tag generator
│   ├── DcmSharp.Benchmarks/         BenchmarkDotNet benchmarks
│   ├── DcmFind/                     CLI: search
│   ├── DcmAnonymize/                CLI: anonymize
│   └── DcmOrganize/                 CLI: organize
├── tests/
│   ├── DcmSharp.Tests/
│   ├── DcmSharp.SourceGenerator.Tests/
│   ├── DcmFind.Tests/
│   ├── DcmAnonymize.Tests/
│   └── DcmOrganize.Tests/
└── dicom/                           Test DICOM files (7 files)
```

---

## Build & Test

```bash
dotnet build --configuration Release
dotnet test  --configuration Release
```

CI runs on **ubuntu-latest** (GitHub Actions: `.github/workflows/build.yml`). Both commands must pass before merging.

- `TreatWarningsAsErrors` is **on** — every compiler warning is a build failure.
- Nullable reference types are **enabled**.
- Target framework: **net9.0** (source generator targets `netstandard2.0`).

---

## Core Architecture

### Parser (`src/DcmSharp/Parser/`)

`DicomParser` is a streaming, async state-machine parser built on `System.IO.Pipelines`.

- **Stages:** ParseGroup → ParseElement → ParseVR → ParseLength → ParseValue
- State is tracked in `DicomParseState` (mutable struct with sequence stacks).
- Supports both Explicit and Implicit VR; switches transfer syntax mid-stream.
- Configurable early termination via `DicomParserOptions.StopParsing`.
- All per-VR parsing is delegated to `DicomValueParser` and its 25+ sub-parsers.

### Data Model

| Class | Notes |
|---|---|
| `ReadOnlyDicomDataset` | Main parse output; struct; implements `IReadOnlyDictionary<uint, ReadOnlyDicomItem>`; disposable |
| `DicomDataset` | Mutable builder for constructing datasets |
| `ReadOnlyDicomItem` | Single tag with group, element, and raw content |
| `DicomTag` | Immutable record with VR, VM, keyword, and name; ~5000+ constants in `DicomTags` (source-generated) |
| `DicomItem<TValue>` | Abstract base; 30+ sealed record subtypes per VR (`DicomDate`, `DicomPersonName`, etc.) |

### Memory Management

Performance-critical code uses object pooling throughout:

- `DicomMemory` — rented `ArrayPool` byte arrays; dispose to return.
- `DicomDatasetsPool`, `DicomMemoriesPool`, `DicomFragmentsPool`, `DicomItemDictionaryPool` — reuse allocations across parses.
- Do not allocate new dictionaries or byte arrays in hot paths; use the pools.

### Source Generator (`src/DcmSharp.SourceGenerator/`)

`DicomTagsGenerator` (an `IIncrementalGenerator`) reads `standard.zip` at compile time and emits `DicomTags.g.cs` with a static property per standard DICOM tag. Do not hand-edit this generated file.

---

## Key Design Patterns

### Partial Classes

Large classes are split into many files by feature:

- `ReadOnlyDicomDataset` — 50+ partial files, one per `TryGetXxx` method group.
- `DicomDataset` — separate files for `Add*` and `TryGet*` operations.
- `DicomItemFactory` — split by value type.

When adding a new accessor method, follow the existing per-file convention rather than appending to a single file.

### Dependency Injection

All services are registered via `AddDcmParse()` (`DependencyInjection.cs`). Every parser (including all VR sub-parsers) is a singleton. Follow this pattern when adding new parsers.

### Records and Sealed Types

Prefer sealed records for immutable domain types. The codebase avoids inheritance hierarchies in favour of discriminated-union-style sealed record hierarchies under `DicomItem<TValue>`.

---

## Coding Conventions

Follow the rules in `.editorconfig`. Key points:

- **Indentation:** 4 spaces for C#; 2 spaces for XML/YAML/JSON.
- **Braces:** Allman style — opening brace on its own line.
- **Naming:** `_camelCase` for private fields; `PascalCase` everywhere else.
- **`var`:** Only when the type is obvious from the right-hand side; use explicit types for built-in primitives.
- **Expression bodies:** Preferred for single-expression methods and properties.
- **Pattern matching:** Preferred over `is`/`as` casts.
- **No `this.` qualification.**
- **No comments** unless the _why_ is non-obvious — well-named identifiers are documentation enough.

---

## Adding Common Features

### New VR parser

1. Create `Parser/ValueRepresentations/XxxParser.cs` following the pattern of existing parsers.
2. Register it as a singleton in `DependencyInjection.cs`.
3. Add it to `DicomValueParser`'s dispatch table.
4. Add a corresponding `DicomXxx` sealed record in `DicomItem.cs`.
5. Add tests in `DcmSharp.Tests/`.

### New `ReadOnlyDicomDataset` accessor

1. Create `ReadOnlyDicomDataset.TryGetXxx.cs` as a partial class file.
2. Add matching tests to `DcmSharp.Tests/`.

### New CLI command (DcmFind / DcmOrganize / DcmAnonymize)

1. Add a command/handler class in the relevant `src/<tool>/` project.
2. Wire it into the root command factory.
3. Add integration tests in `tests/<tool>.Tests/`.

---

## Test Setup

- **Framework:** xUnit 2.9.2 + FluentAssertions 6.12.2
- **Fixtures:** `DicomParserFixture` initialises a DI container; tests use `[Collection(nameof(DicomParserCollection))]` for shared setup.
- **Test data:** DICOM files live in `dicom/` and are copied to test output automatically.
- Always test against both `ExplicitVR.dcm` and `ImplicitVR.dcm` when touching the parser.

---

## What to Avoid

- **Do not skip `TreatWarningsAsErrors`** — fix the warning, do not suppress it unless there is a documented reason.
- **Do not allocate in hot paths** — use the existing object pools.
- **Do not edit `DicomTags.g.cs`** — it is auto-generated by the source generator.
- **Do not add backwards-compatibility shims** for removed API surface — just change the call sites.
- **Do not add comments** that restate what the code does — only add one when a constraint or invariant would surprise a future reader.
