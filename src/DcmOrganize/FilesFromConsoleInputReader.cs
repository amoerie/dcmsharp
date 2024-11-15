using System.Runtime.CompilerServices;

namespace DcmOrganize;

internal interface IFilesFromConsoleInputReader
{
    IAsyncEnumerable<FileInfo> Read(CancellationToken cancellationToken);
}

internal class FilesFromConsoleInputReader : IFilesFromConsoleInputReader
{
    private readonly ILinesFromConsoleInputReader _linesFromConsoleInputReader;

    public FilesFromConsoleInputReader(ILinesFromConsoleInputReader linesFromConsoleInputReader)
    {
        _linesFromConsoleInputReader = linesFromConsoleInputReader ?? throw new ArgumentNullException(nameof(linesFromConsoleInputReader));
    }

    public async IAsyncEnumerable<FileInfo> Read([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var line in _linesFromConsoleInputReader.Read(cancellationToken))
        {
            if (File.Exists(line))
                yield return new FileInfo(line);
        }
    }
}