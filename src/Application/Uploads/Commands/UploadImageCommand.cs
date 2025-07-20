using Application.Common.Queues;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Domain.Users;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Uploads.Commands;

public class UploadImageCommand : IRequest<Result<Success, Error>>
{
    public required IFormFile File { get; init; }
    public required string Directory { get; init; }
    public required string Extension { get; init; }
}

public class UploadImageCommandHandler(AccessQueue accessQueue) : IRequestHandler<UploadImageCommand, Result<Success, Error>>
{
    public async Task<Result<Success, Error>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(request.Directory))
        {
            const string parameterName = "directory";
            return Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("Directory not found", parameterName));
        }

        var fileName = $"{Guid.NewGuid()}-{Guid.NewGuid()}{request.Extension}";
        var filePath = Path.Combine(request.Directory, fileName);

        await accessQueue.ExecuteAsync(fileName, async () =>
        {
            await using FileStream stream = new(filePath, FileMode.Create);
            await request.File.CopyToAsync(stream, cancellationToken);
        }, cancellationToken);

        return Success.Create(StatusCodes.Status200OK, new { fileName });
    }
}
