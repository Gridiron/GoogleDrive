using GoogleDrive.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoogleDrive.Database.Configurations;

public class FileUserConfiguration : IEntityTypeConfiguration<FileUser>
{
    public void Configure(EntityTypeBuilder<FileUser> builder)
    {
        builder.HasKey(e => new { e.FileId, e.UserId });

        builder.HasOne(e => e.User)
            .WithMany(e => e.FileUsers)
            .HasForeignKey(e => e.UserId)
            .IsRequired();

        builder.HasOne(e => e.File)
            .WithMany(e => e.FileUsers)
            .HasForeignKey(e => e.FileId)
            .IsRequired();
    }
}
