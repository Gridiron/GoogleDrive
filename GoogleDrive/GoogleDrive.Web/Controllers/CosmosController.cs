using GoogleDrive.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace GoogleDrive.Web.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CosmosController : ControllerBase
{
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;

    public CosmosController()
    {
        _cosmosClient = new CosmosClient("AccountEndpoint=https://cosmos-devmi.documents.azure.com:443/;AccountKey=97wbaeIFoGgBiAcIvqkoyjfHdYQSnMReXkZw2FkHzcUMnC0NmIVOoEUYsBsdFeUrikR3V4nQ1aAQACDbmdWHdQ==;");
        _container = _cosmosClient.GetContainer("databaseContainer", "filesContainer");
    }

    [HttpGet("{id}-{fileId}")]
    public async Task<IActionResult> GetVersionChunk(string id, int fileId)
    {
        try
        {
            var partitionKey = new PartitionKey(fileId);
            var response = await _container.ReadItemAsync<dynamic>(id, partitionKey);

            //var response = await _container.GetItemQueryIterator<dynamic>("select * from c").ReadNextAsync();
            return Ok();
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateVersionChunk(FileVersionChunk item)
    {
        try
        {
            var response = await _container.CreateItemAsync(new
            {
                id = item.Id.ToString(),
                item.ChunkHash,
                item.BlobStorageUrl,
                item.ChunkNumber,
                item.CreatedTimestamp,
                fileId = item.FileId
            });
            return CreatedAtAction(nameof(GetVersionChunk), new { id = item.Id }, response.Resource);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


}
