using API.Contracts.Coordinator;
using API.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.features.Coordinators.GetCoordinatorByIdFeature;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Coordinators")]
public class GetCoordinatorByIdController : ControllerBase
{
    private readonly IMediator _mediator;

    public GetCoordinatorByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get a coordinator by id
    /// </summary>
    /// <param name="request">int id</param>
    /// <returns>The Coordinator object that was created</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Coordinator), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(List<IError>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] GetCoordinatorByIdRequest request)
    {
        var query = new GetCoordinatorByIdQuery(request.Id);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Value);
    }
    
}