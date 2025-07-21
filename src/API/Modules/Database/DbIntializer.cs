using Infrastructure.Persistence;

namespace API.Modules.Database;

public static class DbIntializer
{
    public static async Task InitializeDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
        await initializer.InitializeAsync();
    }
}
