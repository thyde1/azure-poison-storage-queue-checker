using Microsoft.Extensions.Configuration;
using PoisonQueueChecker;

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

IEnumerable<StorageAccount> storageAccounts = new List<StorageAccount>();
configuration.GetSection("StorageAccounts").Bind(storageAccounts);
var skipLoggingMessages = configuration.GetValue<bool>("SkipLoggingMessages");
var skipAskingForConnectionString = configuration.GetValue<bool>("SkipAskingForConnectionString");

if (!skipAskingForConnectionString)
{
    Console.WriteLine("Storage Account connection string: ");
    var connectionString = Console.ReadLine();
    await PoisonQueueChecker.PoisonQueueChecker.Check(new StorageAccount { ConnectionString = connectionString, Name = "User storage account" }, !skipLoggingMessages);
}
else
{
    foreach (var storageAccount in storageAccounts)
    {
        await PoisonQueueChecker.PoisonQueueChecker.Check(storageAccount, !skipLoggingMessages);
    }
}

Console.ReadLine();
Console.ResetColor();
Console.Clear();