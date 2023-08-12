namespace GoogleDrive.Database.Entities;

public class File
{
    public int Id { get; set; }
    public string Path { get; set; }

    public virtual ICollection<FileUser> FileUsers { get; set; } = new List<FileUser>();
    public virtual ICollection<FileVersionChunk> FileVersionChunks { get; set; } = new List<FileVersionChunk>();
}
