using API.Dependency;
using API.Modules;
using API.Modules.Auth.Scheme;
using API.Modules.Filters;
using API.Modules.Middlewares;
using API.Modules.Route.Filter;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddOpenApi();

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
    options.Filters.Add<ApiResultFilter>();
});

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddHttpContextAccessor();

builder.Services.Inject(builder.Configuration);

builder.Services.AddAuthenticationScheme(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionToErrorMiddleware>();

app.MapOpenApi();

app.ConfigureStaticFiles();

app.UseHttpsRedirection();

app.MapScalarApiReference("/", options =>
{
    options.Theme = ScalarTheme.Solarized;
    options.Layout = ScalarLayout.Classic;

    if (!app.Environment.IsDevelopment())
    {
        options.AddServer("https://jurnee.domain-oa.click", "Default");
    }
});

app.UseAuth();

app.MapControllers();

app.UseCors("AllowOrigin");

await app.RunAsync();

public partial class Program;