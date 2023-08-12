namespace GoogleDrive.Database.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public virtual ICollection<FileUser> FileUsers { get; set; } = new List<FileUser>();
}
