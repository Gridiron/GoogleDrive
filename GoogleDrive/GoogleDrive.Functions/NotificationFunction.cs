
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace NotificationFunction
{
    public class NotificationFunction
    {
        [FunctionName("NotificationFunction")]
        public async Task Run([ServiceBusTrigger("notificationqueue", Connection = "ServiceBusConnection")] string myQueueItem, ILogger log)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(myQueueItem);
            var fileId = (int)body.FileId;
            var userId = (int)body.UserId;
            Console.WriteLine($"Notify all user {userId} that have access file {fileId}");

            //if user is offline put item to message storage
            var jsonString = JsonConvert.SerializeObject(new
            {
                FileId = fileId,
                UserId = userId,
            });


            const string secretName = "storageAccountConnectionString";
            var keyVaultName = "akvdevmi";
            var kvUri = $"https://{keyVaultName}.vault.azure.net";

            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
            var secret = await client.GetSecretAsync(secretName);

            var storageAccountConnectionString = secret.Value.Value;
            Console.WriteLine($"Your secret is '{storageAccountConnectionString}'.");


            var queue = new QueueClient(storageAccountConnectionString, "uploadhistoryqueuemi");
            await queue.SendMessageAsync(jsonString);
        }
    }
}
