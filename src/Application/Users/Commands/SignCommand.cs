using System.Security.Claims;
using Application.Common.Interfaces.Services;
using Application.Common.Templates.Response;
using CSharpFunctionalExtensions;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Users.Commands;

public class SignCommand : IRequest<Result<Success, Error>>
{
    public string TokenId { get; init; } = string.Empty;
}

public class SignCommandHandler(ISender sender, IGoogleTokenService googleToken, IJwtService jwtService) : IRequestHandler<SignCommand, Result<Success, Error>>
{
    const string parameterName = "tokenId";

    public async Task<Result<Success, Error>> Handle(SignCommand request, CancellationToken cancellationToken)
    {
        var principal = googleToken.Principal;

        if (principal is null)
            return Error.Create(StatusCodes.Status406NotAcceptable, ErrorContent.Create("Token is not valid", parameterName));

        var claims = principal.Claims.ToDictionary(x => x.Type, x => x.Value);
        var command = new CreateUserOrGetCommand
        {
            FirstName = claims[ClaimTypes.GivenName],
            LastName = claims[ClaimTypes.Surname],
            Email = claims[ClaimTypes.Email],
        };

        var user = await sender.Send(command, cancellationToken);

        if (user.IsFailure)
            return user.Error;

        string token = await jwtService.GenerateToken(user.Value, cancellationToken);

        return Success.Create(StatusCodes.Status200OK, new { token });
    }
}
