// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using DcmSharp.Benchmarks;

BenchmarkRunner.Run<ValueBenchmarks>();

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
