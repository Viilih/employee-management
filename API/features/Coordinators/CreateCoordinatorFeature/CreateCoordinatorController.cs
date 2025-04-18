using API.Contracts.Coordinator;
using API.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.features.Coordinators.CreateCoordinatorFeature;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Coordinators")]

public class CreateCoordinatorController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateCoordinatorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a Coordinator
    /// </summary>
    /// <param name="request"></param>
    /// <returns>The Coordinator object that was created</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Coordinator), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<IError>), StatusCodes.Status400BadRequest)]
    [Route("/api/[controller]/[action]")]
    public async Task<IActionResult> CreateCoordinator([FromBody] CreateCoordinatorRequest request)
    {
        var command = new CreateCoordinatorCommand(request.FirstName, request.LastName, request.Email);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Value);
    }
}