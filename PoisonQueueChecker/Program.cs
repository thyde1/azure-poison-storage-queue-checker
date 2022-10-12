using Microsoft.Extensions.Configuration;
using PoisonQueueChecker;

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

IEnumerable<StorageAccount> storageAccounts = new List<StorageAccount>();
configuration.GetSection("StorageAccounts").Bind(storageAccounts);
var logMessages = configuration.GetValue<bool>("LogMessages");
var askForConnectionString = configuration.GetValue<bool>("AskForConnectionString");

if (askForConnectionString)
{
    Console.WriteLine("Storage Account connection string: ");
    var connectionString = Console.ReadLine();
    await PoisonQueueChecker.PoisonQueueChecker.Check(new StorageAccount { ConnectionString = connectionString, Name = "User storage account" }, logMessages);
}
else
{
    foreach (var storageAccount in storageAccounts)
    {
        await PoisonQueueChecker.PoisonQueueChecker.Check(storageAccount, logMessages);
    }
}

Console.ReadKey();
Console.ResetColor();
Console.Clear();