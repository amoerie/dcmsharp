// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;
using BenchmarkDotNet.Running;
using DcmSharp.Benchmarks;
using DcmSharp.Parser;
using FellowOakDicom;
using Microsoft.Extensions.DependencyInjection;

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

namespace DcmSharp.Benchmarks
{
    [MemoryDiagnoser]
    [DotTraceDiagnoser]
    [ShortRunJob]
    public class Benchmarks
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
}
