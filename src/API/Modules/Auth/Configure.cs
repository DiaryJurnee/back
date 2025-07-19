namespace API.Modules;

public static partial class Configure
{
    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app
            .UseAuthentication()
            .UseAuthorization();

        return app;
    }
}
