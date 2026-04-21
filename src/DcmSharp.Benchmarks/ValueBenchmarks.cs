using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;
using DcmSharp.Parser;
using FellowOakDicom;
using Microsoft.Extensions.DependencyInjection;

namespace DcmSharp.Benchmarks;

[MemoryDiagnoser]
/*
[DotTraceDiagnoser]
*/
[ShortRunJob]
public class ValueBenchmarks
{
    private FileInfo _file = default!;
    private IDicomParser _dicomParser = default!;
    private ServiceProvider _serviceProvider = default!;
    private DicomFile _dicomFile = default!;
    private ReadOnlyDicomDataset _readOnlyDicomDataset;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddFellowOakDicom();
        services.AddDcmParse();
        _serviceProvider = services.BuildServiceProvider();
        DicomSetupBuilder.UseServiceProvider(_serviceProvider);
        _file = new FileInfo($"./Dicom/ExplicitVR.dcm");
        _dicomParser = _serviceProvider.GetRequiredService<IDicomParser>();
        _dicomFile = DicomFile.Open(_file.FullName, FileReadOption.ReadLargeOnDemand);
        _readOnlyDicomDataset = _dicomParser.ParseReadOnlyAsync(_file).Result;
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _readOnlyDicomDataset.Dispose();
        _serviceProvider.Dispose();
    }

    [Benchmark(Baseline = true)]
    public int FellowOakDicom()
    {
        _ = _dicomFile.Dataset.TryGetString(
            global::FellowOakDicom.DicomTag.SOPInstanceUID,
            out string sopInstanceUID
        );
        _ = _dicomFile.Dataset.TryGetString(
            global::FellowOakDicom.DicomTag.PatientID,
            out string patientId
        );
        _ = _dicomFile.Dataset.TryGetString(
            global::FellowOakDicom.DicomTag.Modality,
            out string modality
        );
        _ = _dicomFile.Dataset.TryGetSingleValue(
            global::FellowOakDicom.DicomTag.ExposureTime,
            out int? exposureTime
        );

        return (sopInstanceUID?.Length ?? 0)
            + (patientId?.Length ?? 0)
            + (modality?.Length ?? 0)
            + (exposureTime ?? 0);
    }

    [Benchmark]
    public int DicomParser()
    {
        _ = _readOnlyDicomDataset.TryGetString(
            DicomTags.SOPInstanceUID,
            out string? sopInstanceUID
        );
        _ = _readOnlyDicomDataset.TryGetString(DicomTags.PatientID, out string? patientId);
        _ = _readOnlyDicomDataset.TryGetString(DicomTags.Modality, out string? modality);
        _ = _readOnlyDicomDataset.TryGetInt(DicomTags.ExposureTime, out int exposureTime);

        return (sopInstanceUID?.Length ?? 0)
            + (patientId?.Length ?? 0)
            + (modality?.Length ?? 0)
            + exposureTime;
    }
}
