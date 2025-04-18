using API.Contracts.Department;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.features.Departments.AssignDepartmentToAnotherCoordinator;


[ApiController]
[Route("/api/[controller]")]
[ApiExplorerSettings(GroupName = "Departments")]
public class AssignDepartToCoordEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public AssignDepartToCoordEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    [Route("/api/[controller]/[action]")]
    public async Task<IActionResult> AssignDepartmentToAnotherCoordinator([FromBody] AssignDepartToCoordRequest request)
    {
        var command = new AssignDepartmentToAnotherCoordinatorCommand(request.NewCoordinatorId, request.DepartmentId);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Value);
        
    }
}