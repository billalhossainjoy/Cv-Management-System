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

        builder.HasOne(x => x.Attribute).WithMany(x => x.ProfileValues);
    }
}