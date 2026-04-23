using System.Threading.Channels;
using DcmSharp;
using DcmSharp.Parser;
using Microsoft.Extensions.Logging;

namespace DcmFind;

public class DicomFileMatcher(IDicomParser dicomParser, ILogger<DicomFileMatcher> logger)
{
    public async Task MatchAsync(
        ChannelReader<string> input,
        ChannelWriter<string> output,
        bool writeToConsoleOutput,
        ChannelWriter<ConsoleOutput> consoleOutput,
        List<IQuery> queries,
        CancellationToken cancellationToken)
    {
        try
        {
            while (await input.WaitToReadAsync(cancellationToken))
            {
                while (input.TryRead(out string? file))
                {
                    if (writeToConsoleOutput)
                    {
                        consoleOutput.TryWrite(new ConsoleOutput(file, true));
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    ReadOnlyDicomDataset dicomDataset;
                    try
                    {
                        dicomDataset = await dicomParser.ParseReadOnlyAsync(new FileInfo(file), cancellationToken);
                    }
                    catch (Exception ex) when (cancellationToken.IsCancellationRequested == false)
                    {
                        logger.LogDebug(ex, "Skipping file {File}: could not be parsed as DICOM", file);
                        continue;
                    }

                    using (dicomDataset)
                    {
                        var matches = true;
                        for (var i = 0; i < queries.Count; i++)
                        {
                            var query = queries[i];
                            if (!query.Matches(dicomDataset))
                            {
                                matches = false;
                                break;
                            }
                        }

                        if (matches)
                        {
                            await output.WriteAsync(file, cancellationToken);
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ignored
        }
    }
}
