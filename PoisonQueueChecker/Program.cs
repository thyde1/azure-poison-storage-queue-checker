using Microsoft.Extensions.Configuration;
using PoisonQueueChecker;

var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

IEnumerable<StorageAccount> storageAccounts = new List<StorageAccount>();
configuration.GetSection("StorageAccounts").Bind(storageAccounts);
var logMessages = configuration.GetValue<bool>("LogMessages");

foreach (var storageAccount in storageAccounts)
{
    await PoisonQueueChecker.PoisonQueueChecker.Check(storageAccount, logMessages);
}

Console.ReadKey();
Console.ResetColor();
Console.Clear();