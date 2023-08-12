using GoogleDrive.Database.Configurations;
using GoogleDrive.Database.Entities;
using Microsoft.EntityFrameworkCore;
using File = GoogleDrive.Database.Entities.File;

namespace GoogleDrive.Database;

public class GoogleDriveDbContext : DbContext
{
    public GoogleDriveDbContext(DbContextOptions<GoogleDriveDbContext> options) : base(options)
    {
    }

    public static GoogleDriveDbContext GetGoogleDriveDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<GoogleDriveDbContext>();

        optionsBuilder.UseSqlServer($@"Server=tcp:sqlserver-dev-mi.database.windows.net,1433;Initial Catalog=sqlDBName-dev-mi;Persist Security Info=False;User ID=mikita_ishchanka;Password=31415Qwerty;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", sqlServerOptionsAction =>
        {
            sqlServerOptionsAction.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
        });

        return new GoogleDriveDbContext(optionsBuilder.Options);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<FileUser> FileUsers { get; set; }
    public DbSet<FileVersionChunk> FileVersionChunks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new FileConfiguration());
        modelBuilder.ApplyConfiguration(new FileUserConfiguration());
        modelBuilder.ApplyConfiguration(new FileVersionChunkConfiguration());
    }
}
