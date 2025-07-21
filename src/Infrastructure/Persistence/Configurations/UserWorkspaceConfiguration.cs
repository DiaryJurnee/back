using Domain.Users;
using Domain.UsersWorkspaces;
using Domain.Workspaces;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserWorkspaceConfiguration : IEntityTypeConfiguration<UserWorkspace>
{
    public void Configure(EntityTypeBuilder<UserWorkspace> builder)
    {
        builder.HasKey(x => new { x.UserId, x.WorkspaceId });

        builder.Property(x => x.WorkspaceId).IsRequired().HasConversion(x => x.Value, x => WorkspaceId.New(x));
        builder.Property(x => x.UserId).IsRequired().HasConversion(x => x.Value, x => UserId.New(x));

        builder.Property(x => x.CreatedAt).IsRequired().HasConversion(new DateTimeUtcConverter());
        builder.Property(x => x.UpdatedAt).IsRequired().HasConversion(new DateTimeUtcConverter());

        builder.Property(x => x.CanReadAll).IsRequired().HasColumnType("boolean").HasDefaultValue(true);
        builder.Property(x => x.CanCreate).IsRequired().HasColumnType("boolean").HasDefaultValue(true);
        builder.Property(x => x.CanUpdate).IsRequired().HasColumnType("boolean").HasDefaultValue(true);
        builder.Property(x => x.CanDelete).IsRequired().HasColumnType("boolean").HasDefaultValue(true);
        builder.Property(x => x.CanInviteOtherUser).IsRequired().HasColumnType("boolean").HasDefaultValue(false);

        builder.HasOne(x => x.User)
            .WithMany(x => x.UsersWorkspaces)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_UserWorkspace_User_UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Workspace)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.WorkspaceId)
            .HasConstraintName("FK_UserWorkspace_Workspace_WorkspaceId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
