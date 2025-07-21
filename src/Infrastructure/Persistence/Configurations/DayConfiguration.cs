using Domain.Clusters;
using Domain.Days;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class DayConfiguration : IEntityTypeConfiguration<Day>
{
    public void Configure(EntityTypeBuilder<Day> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => DayId.New(x));
        builder.Property(x => x.ClusterId).IsRequired().HasConversion(x => x.Value, x => ClusterId.New(x));

        builder.Property(x => x.Title).IsRequired().HasColumnType("varchar(255)");

        builder.HasOne(x => x.Cluster)
            .WithMany(x => x.Days)
            .HasForeignKey(x => x.ClusterId)
            .HasConstraintName("FK_Day_Cluster_ClusterId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.DayContents)
            .WithOne(x => x.Day)
            .HasForeignKey(x => x.DayId)
            .HasConstraintName("FK_Day_DayContents_DayId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
