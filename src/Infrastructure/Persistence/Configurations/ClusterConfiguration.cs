using Domain.Clusters;
using Domain.Users;
using Domain.Workspaces;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ClusterConfiguration : IEntityTypeConfiguration<Cluster>
{
    public void Configure(EntityTypeBuilder<Cluster> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => ClusterId.New(x));
        builder.Property(x => x.WorkspaceId).IsRequired().HasConversion(x => x.Value, x => WorkspaceId.New(x));
        builder.Property(x => x.OwnerId).IsRequired().HasConversion(x => x.Value, x => UserId.New(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.CreatedAt).IsRequired().HasConversion(new DateTimeUtcConverter());

        builder.HasOne(x => x.Workspace)
            .WithMany(x => x.Clusters)
            .HasForeignKey(x => x.WorkspaceId)
            .HasConstraintName("FK_Cluster_Workspace_WorkspaceId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Owner)
            .WithMany(x => x.Clusters)
            .HasForeignKey(x => x.OwnerId)
            .HasConstraintName("FK_Cluster_User_OwnerId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Days)
            .WithOne(x => x.Cluster)
            .HasForeignKey(x => x.ClusterId)
            .HasConstraintName("FK_Cluster_Days_ClusterId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
