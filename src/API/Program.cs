using API.Modules.Route.Filter;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddOpenApi();

builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
});

builder.Services.AddCors(c =>
{
    c.AddPolicy("AllowOrigin", options => options
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapOpenApi();

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

// await app.InitialiseDb();

app.MapControllers();

app.UseCors("AllowOrigin");

await app.RunAsync();

public partial class Program;