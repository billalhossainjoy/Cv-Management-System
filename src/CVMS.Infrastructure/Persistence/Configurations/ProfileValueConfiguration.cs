using CVMS.Domain.Entities;
using CVMS.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CVMS.Infrastructure.Persistence.Configurations;

public class ProfileValueConfiguration: IEntityTypeConfiguration<ProfileValue>
{
    public void Configure(EntityTypeBuilder<ProfileValue> builder)
    {
        builder.ToTable("ProfileValues");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Value).HasMaxLength(500);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasIndex(x => new { x.ProfileId, x.AttributeId });
        
        builder.HasOne(x => x.Profile)
            .WithMany(x => x.Values)
            .HasForeignKey(x => x.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Attribute)
            .WithMany(x => x.ProfileValues)
            .HasForeignKey(x => x.AttributeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}