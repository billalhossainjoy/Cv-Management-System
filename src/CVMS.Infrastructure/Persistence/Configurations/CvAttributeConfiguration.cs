using CVMS.Domain.Attributes;
using CVMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CVMS.Infrastructure.Persistence.Configurations;

public sealed class CvAttributeConfiguration: IEntityTypeConfiguration<CvAttribute>
{
    public void Configure(EntityTypeBuilder<CvAttribute> builder)
    {
            builder.ToTable("Attributes");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
            builder.HasIndex(x => x.Name).IsUnique();

            builder.Property(x => x.Description)
                .HasMaxLength(1000);
            
            builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.IsBuiltIn)
                .IsRequired();
            builder.Property(x => x.Version).IsRowVersion();

            builder.HasData(
                new CvAttribute
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "First Name",
                    Description = "Candidate First Name",
                    Category = AttributeCategory.PersonalInformation,
                    Type = AttributeType.String,
                    IsBuiltIn = true,
                },
                new CvAttribute
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Last Name",
                    Description = "Candidate Last Name",
                    Category = AttributeCategory.PersonalInformation,
                    Type = AttributeType.String,
                    IsBuiltIn = true,
                },
                new CvAttribute
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Location",
                    Description = "Candidate Last Name",
                    Category = AttributeCategory.PersonalInformation,
                    Type = AttributeType.String,
                    IsBuiltIn = true,
                },
                new CvAttribute
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Photo",
                    Description = "Candidate Profile Photo",
                    Category = AttributeCategory.PersonalInformation,
                    Type = AttributeType.Image,
                    IsBuiltIn = true,
                }
                );
    }
}