using MartinCostello.Logging.XUnit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DcmParse.Tests;

public sealed class DicomParserFixture : IDisposable, ITestOutputHelperAccessor
{
    private readonly ServiceProvider _services;

    public DicomParserFixture()
    {
        _services = new ServiceCollection()
            .AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddXUnit(this);
            })
            .AddDcmParse()
            .BuildServiceProvider();
        DicomParser = _services.GetRequiredService<IDicomParser>();
    }

    public IDicomParser DicomParser { get; }
    public ITestOutputHelper? OutputHelper { get; set; }

    public void Dispose()
    {
        _services.Dispose();
    }
}

[CollectionDefinition(nameof(DicomParserCollection))]
public sealed class DicomParserCollection : ICollectionFixture<DicomParserFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
