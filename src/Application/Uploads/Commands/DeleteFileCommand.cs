using Application.Common.Queues;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Uploads.Commands;

public class DeleteFileCommand : IRequest<Result<Success, Error>>
{
    // the main is fileName
    public required string FileName { get; init; }

    public required string Directory { get; init; }
}

public class DeleteFileCommandHandler(AccessQueue accessQueue) : IRequestHandler<DeleteFileCommand, Result<Success, Error>>
{
    public async Task<Result<Success, Error>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        string path = Path.Combine(request.Directory, request.FileName);

        if (!File.Exists(path))
        {
            const string parameterName = "fileName";
            return Error.Create(StatusCodes.Status404NotFound, ErrorContent.Create("File not found", parameterName));
        }

        await accessQueue.ExecuteAsync(path, () =>
        {
            File.Delete(path);
            return Task.CompletedTask;
        }, cancellationToken);

        return Success.Create(StatusCodes.Status204NoContent, new { });
    }
}
