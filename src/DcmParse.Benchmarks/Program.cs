// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;
using BenchmarkDotNet.Running;
using DcmParser;
using FellowOakDicom;
using Microsoft.Extensions.Logging.Abstractions;

BenchmarkRunner.Run<Benchmarks>();

// var b = new Benchmarks
// {
//     FileName = "ImplicitVR.dcm"
// };
//
// b.Setup();
//
// for (int i = 0; i < 20_000; i++)
// {
//     /*if (i % 100 == 0)
//     {
//         Console.WriteLine(i);
//     }*/
//     await b.DicomParser();
// }

[MemoryDiagnoser]
[DotTraceDiagnoser]
[ShortRunJob]
public class Benchmarks
{
    private FileInfo _file = default!;
    private DicomParser _dicomParser = default!;

    [Params("ExplicitVR.dcm", "ImplicitVR.dcm", "Large.dcm")]
    public string? FileName { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _file = new FileInfo($"./Files/{FileName}");
        new DicomSetupBuilder().Build();
        _dicomParser = new DicomParser(NullLogger<DicomParser>.Instance);
    }

    [Benchmark(Baseline = true)]
    public async Task<DicomFile> FellowOakDicom()
    {
        return await DicomFile.OpenAsync(_file.FullName, FileReadOption.ReadAll);
    }

    [Benchmark]
    public async Task DicomParser()
    {
        using var _ = await _dicomParser.ParseAsync(_file);
    }
}
