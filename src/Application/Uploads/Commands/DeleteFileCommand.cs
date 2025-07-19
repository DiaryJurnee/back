using Application.Common.Queues;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Uploads.Commands;

public class DeleteFileCommand : IRequest<Result<Success, Error>>
{
    // the main is file name
    public required string Path { get; init; }
}

public class DeleteFileCommandHandler(AccessQueue accessQueue) : IRequestHandler<DeleteFileCommand, Result<Success, Error>>
{
    public async Task<Result<Success, Error>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        await accessQueue.ExecuteAsync(request.Path, () =>
        {
            File.Delete(request.Path);
            return Task.CompletedTask;
        }, cancellationToken);

        return Success.Create(StatusCodes.Status204NoContent, new { });
    }
}
