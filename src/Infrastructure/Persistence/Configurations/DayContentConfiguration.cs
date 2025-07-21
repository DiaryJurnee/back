using Domain.DayContents;
using Domain.Days;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DayContentConfiguration : IEntityTypeConfiguration<DayContent>
{
    public void Configure(EntityTypeBuilder<DayContent> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => DayContentId.New(x));
        builder.Property(x => x.DayId).IsRequired().HasConversion(x => x.Value, x => DayId.New(x));

        builder.Property(x => x.Text).IsRequired().HasColumnType("text");
        builder.Property(x => x.StartAt).HasConversion(new DateTimeUtcConverter());
        builder.Property(x => x.EndAt).HasConversion(new DateTimeUtcConverter());
        builder.Property(x => x.CreatedAt).IsRequired().HasConversion(new DateTimeUtcConverter());
        builder.Property(x => x.UpdatedAt).IsRequired().HasConversion(new DateTimeUtcConverter());

        builder.HasOne(x => x.Day)
            .WithMany(x => x.DayContents)
            .HasForeignKey(x => x.DayId)
            .HasConstraintName("FK_DayContent_Day_DayId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
