using GoogleDrive.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoogleDrive.Database.Configurations;

public class FileVersionChunkConfiguration : IEntityTypeConfiguration<FileVersionChunk>
{
    public void Configure(EntityTypeBuilder<FileVersionChunk> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.File)
            .WithMany(e => e.FileVersionChunks)
            .HasForeignKey(e => e.FileId);
    }
}
