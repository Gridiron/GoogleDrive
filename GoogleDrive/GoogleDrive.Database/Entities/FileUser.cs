namespace GoogleDrive.Database.Entities;

public class FileUser
{
    public int FileId { get; set; }
    public int UserId { get; set; }
    public virtual File File { get; set; }
    public virtual User User { get; set; }
}
