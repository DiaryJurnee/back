using API.Dtos;
using Application.Common.Templates.Response;
using Application.Users.Commands;
using CSharpFunctionalExtensions;
using Mediator.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class SignController(ISender sender) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<Result<Success, Error>> Google([FromForm] GoogleDto dto, CancellationToken cancellationToken)
    {
        var input = new SignCommand
        {
            TokenId = dto.TokenId
        };

        var result = await sender.Send(input, cancellationToken);

        return result;
    }
}
