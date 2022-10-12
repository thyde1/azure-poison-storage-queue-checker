namespace PoisonQueueChecker;

using Azure.Storage.Queues;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class PoisonQueueChecker
{
    public async static Task Check(StorageAccount storageAccount, bool logMessages)
    {
        var queueServiceClient = new QueueServiceClient(storageAccount.ConnectionString);

        var queues = queueServiceClient.GetQueuesAsync();
        var pages = queues.AsPages();

        Console.WriteLine($"Checking {storageAccount.Name}");

        var anyPoisonItems = false;
        await foreach (var page in pages)
        {
            foreach (var queue in page.Values.Where(q => q.Name.EndsWith("-poison")))
            {
                var client = queueServiceClient.GetQueueClient(queue.Name);
                if (logMessages)
                {

                    var peeked = await client.PeekMessagesAsync(client.MaxPeekableMessages);
                    if (peeked?.Value != null && peeked.Value.Length > 0)
                    {
                        if (peeked.Value.Length == client.MaxPeekableMessages)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Showing FIRST 32 messages from {queue.Name}");
                            Console.ResetColor();
                        }
                        foreach (var message in peeked.Value)
                        {
                            if (message != null)
                            {
                                anyPoisonItems = true;
                                Console.WriteLine($"{queue.Name} - {message.InsertedOn}");
                                var base64 = Encoding.UTF8.GetString(message.Body);
                                Console.WriteLine($"{TransformToReadableString(base64)}");
                            }
                        }
                    }
                }
                else
                {
                    var peeked = await client.PeekMessageAsync();
                    if (peeked.Value != null)
                    {
                        anyPoisonItems = true;
                        Console.WriteLine($"{queue.Name} - {peeked.Value.InsertedOn}");
                    }
                }
            }
        }

        if (!anyPoisonItems)
        {
            Console.WriteLine("No poison queue items found.");
        }

        Console.WriteLine("---");
    }

    private static string TransformToReadableString(string input)
    {
        Span<byte> buffer = new Span<byte>(new byte[input.Length]);
        var isBase64 = Convert.TryFromBase64String(input, buffer, out var bytesParsed);
        return isBase64 ? Encoding.UTF8.GetString(buffer) : input;
    }
}
