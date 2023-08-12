using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using GoogleDrive.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GoogleDrive.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private GoogleDriveDbContext _context;

    public FileController(GoogleDriveDbContext context)
    {
        _context = context;
    }

    [HttpGet("get-file-paths")]
    public async Task<ActionResult> GetFilePaths(int userId)
    {
        var files = await _context.FileUsers
            .Include(x => x.File)
            .Where(x => x.UserId == userId)
            .Select(x => x.File.Path)
            .ToListAsync();

        return Ok(files);
    }

    [HttpPost("upload-file")]
    public async Task<ActionResult> UploadFile(int userId, IFormFile file, string filePath)
    {
        var extension = Path.GetExtension(file.FileName);
        //split message logic

        var keyVaultName = "akvdevmi";
        var kvUri = $"https://{keyVaultName}.vault.azure.net";

        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

        Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
        var secretServiceBusConnectionString = await client.GetSecretAsync("serviceBusConnectionString");

        var serviceBusConnectionString = secretServiceBusConnectionString.Value.Value;
        Console.WriteLine($"Your secret is '{serviceBusConnectionString}'.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        var serviceBusClient = new ServiceBusClient(serviceBusConnectionString);
        var clientQueueSender = serviceBusClient.CreateSender("uploadqueue");
        using ServiceBusMessageBatch messageBatch = await clientQueueSender.CreateMessageBatchAsync();
        var messageBytes = memoryStream.ToArray();

        var jsonString = JsonConvert.SerializeObject(new
        {
            Path = filePath,
            UserId = userId,
            Content = Convert.ToBase64String(messageBytes),
        });

        var success = messageBatch.TryAddMessage(new ServiceBusMessage(jsonString));
        await clientQueueSender.SendMessagesAsync(messageBatch);

        return Ok();
    }

    [HttpGet("download-file")]
    public async Task<ActionResult> DownloadFile(int userId, string filePath)
    {
        var file = await _context.FileUsers.Include(x => x.File).FirstOrDefaultAsync(x => x.UserId == userId && x.File.Path == filePath);
        if (file == null)
        {
            return NotFound();
        }

        const string secretName = "storageAccountConnectionString";
        var keyVaultName = "akvdevmi";
        var kvUri = $"https://{keyVaultName}.vault.azure.net";

        var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

        Console.WriteLine($"Retrieving your secret from {keyVaultName}.");
        var secret = await client.GetSecretAsync(secretName);

        var storageAccountConnectionString = secret.Value.Value;
        Console.WriteLine($"Your secret is '{storageAccountConnectionString}'.");


        var allChunks = await _context.FileVersionChunks.Where(x => x.FileId == file.FileId).ToListAsync();
        var chunk = allChunks.First();

        var blobServiceClient = new BlobServiceClient(storageAccountConnectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("filescontainermi");
        BlobClient blobClient = containerClient.GetBlobClient($"{chunk.FileId}_1_{chunk.Version}");

        var memoryStream = new MemoryStream();
        await blobClient.DownloadToAsync(memoryStream);
        memoryStream.Position = 0;
        var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
        var lastSegment = filePath.Substring(filePath.LastIndexOf('/') + 1);
        var extensionAndName = lastSegment.Split('.');
        provider.TryGetContentType(lastSegment, out var contentType);

        return File(memoryStream, contentType ?? extensionAndName[1], lastSegment);
    }
}
