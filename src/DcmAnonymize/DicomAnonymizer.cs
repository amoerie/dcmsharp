using System.Collections.Concurrent;
using DcmAnonymize.Blanking;
using DcmAnonymize.Instance;
using DcmAnonymize.Order;
using DcmAnonymize.Patient;
using DcmAnonymize.Recursive;
using DcmAnonymize.Series;
using DcmAnonymize.Study;
using FellowOakDicom;

namespace DcmAnonymize;

public class DicomAnonymizer
{
    private readonly PatientAnonymizer _patientAnonymizer;
    private readonly StudyAnonymizer _studyAnonymizer;
    private readonly SeriesAnonymizer _seriesAnonymizer;
    private readonly InstanceAnonymizer _instanceAnonymizer;
    private readonly RecursiveAnonymizer _recursiveAnonymizer;
    private readonly OrderAnonymizer _orderAnonymizer;
    private readonly BlankingAnonymizer _blankingAnonymizer;
    private readonly ConcurrentDictionary<string, DicomUID> _anonymizedUIDs = new ConcurrentDictionary<string, DicomUID>();

    public DicomAnonymizer(PatientAnonymizer patientAnonymizer, StudyAnonymizer studyAnonymizer,
        SeriesAnonymizer seriesAnonymizer, InstanceAnonymizer instanceAnonymizer, 
        RecursiveAnonymizer recursiveAnonymizer, OrderAnonymizer orderAnonymizer,
        BlankingAnonymizer blankingAnonymizer)
    {
        _patientAnonymizer = patientAnonymizer ?? throw new ArgumentNullException(nameof(patientAnonymizer));
        _studyAnonymizer = studyAnonymizer ?? throw new ArgumentNullException(nameof(studyAnonymizer));
        _seriesAnonymizer = seriesAnonymizer ?? throw new ArgumentNullException(nameof(seriesAnonymizer));
        _instanceAnonymizer = instanceAnonymizer ?? throw new ArgumentNullException(nameof(instanceAnonymizer));
        _recursiveAnonymizer = recursiveAnonymizer ?? throw new ArgumentNullException(nameof(recursiveAnonymizer));
        _orderAnonymizer = orderAnonymizer ?? throw new ArgumentNullException(nameof(orderAnonymizer));
        _blankingAnonymizer = blankingAnonymizer ?? throw new ArgumentNullException(nameof(blankingAnonymizer));
    }

    public Task AnonymizeAsync(DicomFile dicomFile, AnonymizationOptions anonymizationOptions)
    {
        return AnonymizeAsync(dicomFile.FileMetaInfo, dicomFile.Dataset, anonymizationOptions);
    }

    public async Task AnonymizeAsync(DicomFileMetaInformation metaInfo, DicomDataset dataset, AnonymizationOptions options)
    {
        var context = new DicomAnonymizationContext(metaInfo, dataset, _anonymizedUIDs, options);
        await _orderAnonymizer.AnonymizeAsync(context);
        await _patientAnonymizer.AnonymizeAsync(context);
        await _studyAnonymizer.AnonymizeAsync(context);
        await _seriesAnonymizer.AnonymizeAsync(context);
        await _instanceAnonymizer.AnonymizeAsync(context);
        await _recursiveAnonymizer.AnonymizeAsync(context);
        await _blankingAnonymizer.AnonymizeAsync(context);
    }
    
}
