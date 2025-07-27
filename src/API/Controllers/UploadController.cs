using API.Attributes;
using API.Modules;
using Application.Common.Templates.Response;
using Application.Uploads.Commands;
using CSharpFunctionalExtensions;
using Domain.SystemRoles;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[CustomAuthorize(SystemRole.Admin)]
public class UploadController(ISender sender) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<Result<Success, Error>> Image([FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        UploadImageCommand command = new()
        {
            File = file,
            Directory = Configure.ImagesDir,
            Extension = Path.GetExtension(file.FileName).ToLowerInvariant()
        };

        var result = await sender.Send(command, cancellationToken);

        return result;
    }

    [HttpDelete("[action]")]
    public async Task<Result<Success, Error>> Image([FromQuery] string fileName, CancellationToken cancellationToken)
    {
        DeleteFileCommand command = new()
        {
            FileName = fileName,
            Directory = Configure.ImagesDir
        };

        var result = await sender.Send(command, cancellationToken);

        return result;
    }
}
