namespace PoisonQueueChecker
{
    using Azure.Storage.Queues;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class PoisonQueueChecker
    {
        public async static Task Check(StorageAccount storageAccount)
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
                    var peeked = await client.PeekMessageAsync();
                    if (peeked.Value != null)
                    {
                        anyPoisonItems = true;
                        Console.WriteLine($"{queue.Name} - {peeked.Value.InsertedOn}");
                    }
                }
            }

            if (!anyPoisonItems)
            {
                Console.WriteLine("No poison queue items found");
            }

            Console.WriteLine("---");
        }
    }
}
