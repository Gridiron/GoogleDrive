using Newtonsoft.Json;

namespace GoogleDrive.Database.Entities;

public class FileVersionChunk
{
    [JsonProperty("id")]
    public int Id { get; set; }
    public int Version { get; set; }
    public string ChunkHash { get; set; }
    public string BlobStorageUrl { get; set; }
    public int ChunkNumber { get; set; }
    public DateTime CreatedTimestamp { get; set; }
    public int FileId { get; set; }
    public virtual File File { get; set; }
}
