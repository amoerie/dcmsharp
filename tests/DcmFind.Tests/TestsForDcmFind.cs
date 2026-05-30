using System.Text;
using FluentAssertions;
using Spectre.Console;
using Xunit;
using Xunit.Abstractions;

namespace DcmFind.Tests;

[Collection("DcmFind")]
public class TestsForDcmFind : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly StringBuilder _output;
    private readonly StringWriter _outputWriter;
    private readonly IAnsiConsole _ansiConsole;
    private readonly DirectoryInfo _testFilesDirectory;
    private readonly FileInfo _testFile0;
    private readonly FileInfo _testFile1;
    private readonly FileInfo _testFile2;

    public TestsForDcmFind(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        _output = new StringBuilder();
        _outputWriter = new StringWriter(_output);
        _ansiConsole = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Out = new AnsiConsoleOutput(_outputWriter),
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
        });

        _testFilesDirectory = new DirectoryInfo("./TestFiles");
        _testFile0 = new FileInfo(Path.Join(_testFilesDirectory.Name, "0.jpg"));
        _testFile1 = new FileInfo(Path.Join(_testFilesDirectory.Name, "1.dcm"));
        _testFile2 = new FileInfo(Path.Join(_testFilesDirectory.Name, "2.dcm"));
    }

    public void Dispose()
    {
        _testOutputHelper.WriteLine(_output.ToString());
        _outputWriter.Dispose();
    }

    private ProgramOptions CreateOptions() => new ProgramOptions(false, 400, _ansiConsole);

    [Fact]
    public async Task ShouldFindAllTestFiles()
    {
        // Arrange
        var expected = new[] { _testFile1.FullName, _testFile2.FullName };

        // Act
        var statusCode = await new Program(CreateOptions()).MainAsync(Array.Empty<string>());

        // Assert
        var actual = _output.ToString().Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        actual.Should().BeEquivalentTo(expected, c => c.WithoutStrictOrdering());
        Assert.Equal(0, statusCode);
    }

    [Fact]
    public async Task ShouldFindWithDirectory()
    {
        // Arrange
        var expected = new[] { _testFile1.FullName, _testFile2.FullName };

        // Act
        var statusCode = await new Program(CreateOptions()).MainAsync(new []
        {
            "--directory", _testFilesDirectory.FullName
        });

        // Assert
        var actual = _output.ToString().Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        actual.Should().BeEquivalentTo(expected, c => c.WithoutStrictOrdering());
        Assert.Equal(0, statusCode);
    }

    [Fact]
    public async Task ShouldFindWithQuery()
    {
        // Arrange
        var expected = new[] { _testFile2.FullName };

        // Act
        var statusCode = await new Program(CreateOptions()).MainAsync(new []
        {
            "--directory", _testFilesDirectory.FullName,
            "--query", "AccessionNumber=CR2022062117111"
        });

        // Assert
        var actual = _output.ToString().Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        actual.Should().BeEquivalentTo(expected, c => c.WithoutStrictOrdering());
        Assert.Equal(0, statusCode);
    }

    [Fact]
    public async Task ShouldFindWithQueryWithLimit()
    {
        // Arrange
        var expected = new[] { _testFile2.FullName };

        // Act
        var statusCode = await new Program(CreateOptions()).MainAsync(new []
        {
            "--directory", _testFilesDirectory.FullName,
            "--query", "AccessionNumber=CR2022062117111",
            "--limit", "1",
        });

        // Assert
        var actual = _output.ToString().Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        actual.Should().BeEquivalentTo(expected, c => c.WithoutStrictOrdering());
        Assert.Equal(0, statusCode);
    }

}
