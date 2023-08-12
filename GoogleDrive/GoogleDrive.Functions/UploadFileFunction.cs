using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using GoogleDrive.Database;
using GoogleDrive.Database.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleDrive.Functions
{
    public class UploadFileFunction
    {
        [FunctionName("UploadFileFunction")]
        public async Task RunAsync([ServiceBusTrigger("uploadqueue", Connection = "ServiceBusConnection")]
            string myQueueItem,
            ILogger log)
        {
            log.LogInformation("Heeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeey");

            var body = JsonConvert.DeserializeObject<dynamic>(myQueueItem);

            GoogleDriveDbContext context = GoogleDriveDbContext.GetGoogleDriveDbContext();

            var userId = (int)body.UserId;

            log.LogInformation($"Users{context.Users.ToList()}");
            log.LogInformation($"UserId {userId}");
            log.LogInformation($"{context.Users.Count()}");

            var user = context.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                log.LogError("Such user doesn't exists");
            }
            var path = (string)body.Path;
            var stringContent = (string)body.Content;

            var keyVaultName = "akvdevmi";
            var kvUri = $"https://{keyVaultName}.vault.azure.net";

            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
            var secretStorageAccountConnectionString = await client.GetSecretAsync("storageAccountConnectionString");

            var storageAccountConnectionString = secretStorageAccountConnectionString.Value.Value;
            Console.WriteLine($"Your secret is '{storageAccountConnectionString}'.");

            Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
            var secretServiceBusConnectionString = await client.GetSecretAsync("serviceBusConnectionString");

            var serviceBusConnectionString = secretServiceBusConnectionString.Value.Value;
            Console.WriteLine($"Your secret is '{serviceBusConnectionString}'.");

            var file = context.Files.Include(x => x.FileVersionChunks)
                .FirstOrDefault(x => x.Path == path && x.FileUsers.FirstOrDefault(x => x.UserId == userId) != null);
            int fileId = 0;
            if (file == null)
            {
                var newFile = new File { Path = path };
                context.Files.Add(newFile);
                context.SaveChanges();

                var blobServiceClient = new BlobServiceClient(storageAccountConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("filescontainermi");
                BlobClient blobClient = containerClient.GetBlobClient($"{newFile.Id}_1_1");

                byte[] fileContent = Convert.FromBase64String(stringContent);
                BinaryData binaryData = new BinaryData(fileContent);
                await blobClient.UploadAsync(binaryData, true);

                var uri = blobClient.Uri.AbsoluteUri;


                log.LogInformation($"File id {newFile.Id}");

                var versionChunk = new FileVersionChunk
                {
                    Version = 1,
                    ChunkNumber = 1,
                    FileId = newFile.Id,
                    BlobStorageUrl = uri,
                    CreatedTimestamp = DateTime.UtcNow,
                    ChunkHash = stringContent.GetHashCode().ToString()
                };

                context.FileVersionChunks.Add(versionChunk);

                context.SaveChanges();

                context.FileUsers.Add(new FileUser { FileId = newFile.Id, UserId = userId });

                context.SaveChanges();
                fileId = newFile.Id;
            }
            else
            {
                int maxVersion = 0;
                if (file.FileVersionChunks != null && file.FileVersionChunks.Count != 0)
                {
                    maxVersion = file.FileVersionChunks.Max(x => x.Version);
                }

                var blobServiceClient = new BlobServiceClient(storageAccountConnectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("filescontainermi");
                BlobClient blobClient = containerClient.GetBlobClient($"{file.Id}_1_{maxVersion + 1}");

                byte[] fileContent = Convert.FromBase64String(stringContent);
                BinaryData binaryData = new BinaryData(fileContent);
                await blobClient.UploadAsync(binaryData, true);
                var uri = blobClient.Uri.AbsoluteUri;

                var versionChunk = new FileVersionChunk
                {
                    Version = maxVersion + 1,
                    ChunkNumber = 1,
                    FileId = file.Id,
                    BlobStorageUrl = uri,
                    CreatedTimestamp = DateTime.UtcNow,
                    ChunkHash = stringContent.GetHashCode().ToString()
                };

                context.FileVersionChunks.Add(versionChunk);

                context.SaveChanges();

                fileId = file.Id;
            }

            var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
            var clientQueueSender = serviceBusClient.CreateSender("notificationqueue");
            using ServiceBusMessageBatch messageBatch = await clientQueueSender.CreateMessageBatchAsync();

            foreach (var fileUser in context.FileUsers.Where(x => x.FileId == fileId))
            {
                var jsonString = JsonConvert.SerializeObject(new
                {
                    FileId = fileUser.FileId,
                    UserId = fileUser.UserId,
                });
                var success = messageBatch.TryAddMessage(new ServiceBusMessage(jsonString));
            }

            await clientQueueSender.SendMessagesAsync(messageBatch);
        }
    }
}
