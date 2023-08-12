using GoogleDrive.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoogleDrive.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.UserName).IsUnique();

        builder.HasMany(e => e.FileUsers)
            .WithOne(e => e.User)
            .HasForeignKey(fu => fu.UserId)
            .IsRequired();
    }
}
