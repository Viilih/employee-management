using API.Contracts.Coordinator;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.features.Coordinators.DeleteCoordinator;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Coordinators")]
public class DeleteCoordinatorController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeleteCoordinatorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<IError>), StatusCodes.Status400BadRequest)]
    [Route("/api/[controller]/[action]")]
    public async Task<IActionResult> DeleteCoordinator(DeleteCoordinatorRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCoordinatorCommand(request.id);
        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }
        
        return Ok(result.Value);
    }
}