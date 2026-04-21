using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;
using DcmSharp.Parser;
using FellowOakDicom;
using Microsoft.Extensions.DependencyInjection;

namespace DcmSharp.Benchmarks;

[MemoryDiagnoser]
[DotTraceDiagnoser]
[ShortRunJob]
public class ParseBenchmarks
{
    private FileInfo _file = default!;
    private IDicomParser _dicomParser = default!;
    private ServiceProvider _serviceProvider = default!;

    [Params("ExplicitVR.dcm", "ImplicitVR.dcm", "Large.dcm")]
    public string? FileName { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddFellowOakDicom();
        services.AddDcmParse();
        _serviceProvider = services.BuildServiceProvider();
        DicomSetupBuilder.UseServiceProvider(_serviceProvider);
        _file = new FileInfo($"./Dicom/{FileName}");
        _dicomParser = _serviceProvider.GetRequiredService<IDicomParser>();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _serviceProvider.Dispose();
    }

    [Benchmark(Baseline = true)]
    public async Task<DicomFile> FellowOakDicom()
    {
        return await DicomFile.OpenAsync(_file.FullName, FileReadOption.ReadAll);
    }

    [Benchmark]
    public async Task DicomParser()
    {
        using var _ = await _dicomParser.ParseReadOnlyAsync(_file);
    }
}
