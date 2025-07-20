using Microsoft.Extensions.FileProviders;

namespace API.Modules;

public static partial class Configure
{
    public static string UploadsDir { get; private set; } = "";

    public static string ImagesDir => Path.Combine(UploadsDir, "Images");
    public static IApplicationBuilder ConfigureStaticFiles(this IApplicationBuilder app)
    {
        UploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        if (!Directory.Exists(UploadsDir))
            Directory.CreateDirectory(UploadsDir);

        if (!Directory.Exists(ImagesDir))
            Directory.CreateDirectory(ImagesDir);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(UploadsDir),
            RequestPath = "/files"
        });

        return app;
    }
}
