using Domain.Workspaces;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => WorkspaceId.New(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.CreatedAt).IsRequired().HasConversion(new DateTimeUtcConverter());

        builder.HasOne(x => x.Owner)
            .WithMany(x => x.Workspaces)
            .HasForeignKey(x => x.OwnerId)
            .HasConstraintName("FK_Workspace_User_OwnerId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Users)
            .WithOne(x => x.Workspace)
            .HasForeignKey(x => x.WorkspaceId)
            .HasConstraintName("FK_Workspace_UserWorkspace_WorkspaceId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Clusters)
            .WithOne(x => x.Workspace)
            .HasForeignKey(x => x.WorkspaceId)
            .HasConstraintName("FK_Workspace_Cluster_WorkspaceId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
