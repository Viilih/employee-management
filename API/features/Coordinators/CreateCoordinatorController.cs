using API.Contracts.Coordinator;
using API.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.features.Coordinators;

[ApiController]
[Route("api/[controller]")]
public class CreateCoordinatorController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateCoordinatorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
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