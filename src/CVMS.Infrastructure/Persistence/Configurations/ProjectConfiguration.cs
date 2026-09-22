using CVMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CVMS.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration: IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500).IsRequired();
        builder.Property(p => p.StartDate).IsRequired();

        builder.HasOne(p => p.Profile).WithMany(p => p.Projects).HasForeignKey(x => x.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(x => x.ProfileId);

    }
}