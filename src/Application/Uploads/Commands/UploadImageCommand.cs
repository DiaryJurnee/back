using Application.Common.Queues;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
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
        var fileName = $"{Guid.NewGuid()}-{Guid.NewGuid()}{request.Extension}";
        var filePath = Path.Combine(request.Directory, fileName);

        await accessQueue.ExecuteAsync(fileName, async () =>
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            await request.File.CopyToAsync(stream, cancellationToken);
        }, cancellationToken);

        return Success.Create(StatusCodes.Status200OK, new { fileName });
    }
}
