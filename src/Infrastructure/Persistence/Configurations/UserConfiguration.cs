using Domain.SystemRoles;
using Domain.Users;
using Infrastructure.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => UserId.New(x));
        builder.Property(x => x.RoleId).IsRequired().HasConversion(x => x.Value, x => SystemRoleId.New(x));

        builder.Property(x => x.FirstName).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.LastName).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Email).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.CreatedAt).IsRequired().HasConversion(new DateTimeUtcConverter());
        builder.Property(x => x.UpdatedAt).IsRequired().HasConversion(new DateTimeUtcConverter());

        builder.HasOne(x => x.Role)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoleId)
            .HasConstraintName("FK_User_SystemRole_RoleId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Workspaces)
            .WithOne(x => x.Owner)
            .HasForeignKey(x => x.OwnerId)
            .HasConstraintName("FK_User_Workspace_OwnerId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.UsersWorkspaces)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .HasConstraintName("FK_User_UserWorkspace_UserId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Clusters)
            .WithOne(x => x.Owner)
            .HasForeignKey(x => x.OwnerId)
            .HasConstraintName("FK_User_Cluster_OwnerId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
