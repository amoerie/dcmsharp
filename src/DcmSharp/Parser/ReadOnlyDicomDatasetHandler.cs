using System.Text;
using DcmSharp.Memory;

namespace DcmSharp.Parser;

internal sealed class ReadOnlyDicomDatasetHandler : IDicomDatasetHandler
{
    private static readonly DicomDatasetsPool _datasetsPool = new DicomDatasetsPool(256, 8);
    private static readonly DicomMemoriesPool _memoriesPool = new DicomMemoriesPool(1024, 32);
    private static readonly DicomFragmentsPool _fragmentsPool = new DicomFragmentsPool(1024, 32);

    private readonly DicomItemDictionaryPool _smallDictionaryPool;
    private readonly DicomValueParser _valueParser;
    private readonly ReadOnlyDicomDataset _rootDataset;

    private ReadOnlyDicomSequenceItem? _currentSequenceItem;
    private (ushort Group, ushort Element, DicomDatasets Items)? _currentSequence;
    private readonly Stack<(ushort Group, ushort Element, DicomDatasets Items)> _sequences = new();
    private readonly Stack<ReadOnlyDicomSequenceItem> _sequenceItems = new();

    private ushort _fragmentsGroup;
    private ushort _fragmentsElement;
    private DicomVR _fragmentsVr;
    private DicomFragments? _currentFragments;

    public ReadOnlyDicomDataset Result => _rootDataset;

    public ReadOnlyDicomDatasetHandler(
        DicomItemDictionaryPool largeDictionaryPool,
        DicomItemDictionaryPool smallDictionaryPool,
        DicomMemories memories,
        DicomValueParser valueParser
    )
    {
        _smallDictionaryPool = smallDictionaryPool;
        _valueParser = valueParser;
        _rootDataset = new ReadOnlyDicomDataset(largeDictionaryPool, memories, valueParser);
    }

    public void OnItem(ushort group, ushort element, DicomVR vr, ReadOnlyMemory<byte> rawValue)
    {
        var item = new ReadOnlyDicomItem(
            group,
            element,
            vr,
            ReadOnlyDicomItemContent.Create(rawValue)
        );
        var dataset = _currentSequenceItem?.ReadOnlyDicomDataset ?? _rootDataset;
        dataset.Add(group, element, item);
    }

    public void OnSequenceStart(ushort group, ushort element)
    {
        if (_currentSequence is { } cs)
            _sequences.Push(cs);
        if (_currentSequenceItem is { } csi)
            _sequenceItems.Push(csi);
        _currentSequence = (group, element, new DicomDatasets(_datasetsPool));
        _currentSequenceItem = null;
    }

    public void OnSequenceItemStart()
    {
        var itemDataset = new ReadOnlyDicomDataset(
            _smallDictionaryPool,
            new DicomMemories(_memoriesPool),
            _valueParser
        );
        _currentSequenceItem = new ReadOnlyDicomSequenceItem(itemDataset, null);
    }

    public void OnSequenceItemEnd()
    {
        if (_currentSequence is { } cs && _currentSequenceItem is { } csi)
        {
            cs.Items.Add(csi.ReadOnlyDicomDataset);
            _currentSequenceItem = null;
        }
    }

    public void OnSequenceEnd()
    {
        if (_currentSequence is not { } cs)
            return;

        var seqItem = new ReadOnlyDicomItem(
            cs.Group,
            cs.Element,
            DicomVR.SQ,
            ReadOnlyDicomItemContent.Create(cs.Items.ToReadOnly())
        );

        if (_sequenceItems.TryPop(out var parentItem))
        {
            parentItem.ReadOnlyDicomDataset.Add(cs.Group, cs.Element, seqItem);
            _currentSequenceItem = parentItem;
        }
        else
        {
            _rootDataset.Add(cs.Group, cs.Element, seqItem);
            _currentSequenceItem = null;
        }

        _currentSequence = _sequences.TryPop(out var parentSeq) ? parentSeq : null;
    }

    public void OnFragmentsStart(ushort group, ushort element, DicomVR vr)
    {
        _fragmentsGroup = group;
        _fragmentsElement = element;
        _fragmentsVr = vr;
        _currentFragments = new DicomFragments(_fragmentsPool);
    }

    public void OnFragment(ReadOnlyMemory<byte> fragment)
    {
        _currentFragments!.Add(fragment);
    }

    public void OnFragmentsEnd()
    {
        if (_currentFragments is not { } frags)
            return;

        var item = new ReadOnlyDicomItem(
            _fragmentsGroup,
            _fragmentsElement,
            _fragmentsVr,
            ReadOnlyDicomItemContent.Create(frags.ToReadOnly())
        );
        _rootDataset.Add(_fragmentsGroup, _fragmentsElement, item);
        _fragmentsGroup = default;
        _fragmentsElement = default;
        _fragmentsVr = default;
        _currentFragments = null;
    }

    public void OnEncoding(Encoding encoding)
    {
        var dataset = _currentSequenceItem?.ReadOnlyDicomDataset ?? _rootDataset;
        dataset.Encoding = encoding;
    }
}
