using Domain.Clusters;
using Domain.DayContents;
using Domain.Days;
using Domain.SystemRoles;
using Domain.Users;
using Domain.UsersWorkspaces;
using Domain.Workspaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Cluster> Clusters => Set<Cluster>();
    public DbSet<DayContent> DayContents => Set<DayContent>();
    public DbSet<Day> Days => Set<Day>();
    public DbSet<SystemRole> SystemRoles => Set<SystemRole>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserWorkspace> UsersWorkspaces => Set<UserWorkspace>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
