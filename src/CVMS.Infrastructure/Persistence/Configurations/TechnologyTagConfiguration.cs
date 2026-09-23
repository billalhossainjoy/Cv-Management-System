using CVMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CVMS.Infrastructure.Persistence.Configurations;

public class TechnologyTagConfiguration
    : IEntityTypeConfiguration<TechnologyTag>
{
    public void Configure(EntityTypeBuilder<TechnologyTag> builder)
    {
        builder.ToTable("TechnologyTags");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasMany(x => x.Projects)
            .WithMany(x => x.TechnologyTags)
            .UsingEntity(j =>
                j.ToTable("ProjectTechnologyTags"));

        builder.HasMany(x => x.Positions)
            .WithMany(x => x.TechnologyTags)
            .UsingEntity(j =>
                j.ToTable("PositionTechnologyTags"));
        
        
    }
}