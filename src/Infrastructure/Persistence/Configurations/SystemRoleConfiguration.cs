using Domain.SystemRoles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SystemRoleConfiguration : IEntityTypeConfiguration<SystemRole>
{
    public void Configure(EntityTypeBuilder<SystemRole> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => SystemRoleId.New(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");

        builder.HasMany(x => x.Users)
            .WithOne(x => x.Role)
            .HasForeignKey(x => x.RoleId)
            .HasConstraintName("FK_User_SystemRole_RoleId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
