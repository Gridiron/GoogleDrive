using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = GoogleDrive.Database.Entities.File;

namespace GoogleDrive.Database.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.Id, e.Path }).IsUnique();

        builder.HasMany(e => e.FileUsers)
            .WithOne(e => e.File)
            .HasForeignKey(fu => fu.FileId)
            .IsRequired();

        builder.HasMany(e => e.FileVersionChunks)
            .WithOne(e => e.File)
            .HasForeignKey(fvc => fvc.FileId)
            .IsRequired();
    }
}
