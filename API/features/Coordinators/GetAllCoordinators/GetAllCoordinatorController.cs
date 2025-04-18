using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using API.Domain.Entities;

namespace API.features.Coordinators.GetAllCoordinators;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Coordinators")]
public class CoordinatorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoordinatorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<Coordinator>), StatusCodes.Status200OK)]
    [Route("/api/[controller]/[action]")]
    public async Task<ActionResult<IReadOnlyList<Coordinator>>> GetAllCoordinators(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllCoordinators(), cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);

        return BadRequest(result.Errors);
    }
}