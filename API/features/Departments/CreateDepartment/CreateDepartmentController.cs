using API.Contracts.Department;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.features.Departments.CreateDepartment;


[ApiController]
[Route("/api/[controller]")]
[ApiExplorerSettings(GroupName = "Departments")]
public class CreateDepartmentController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateDepartmentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("/api/[controller]/[action]")]
    public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
    {
        var command = new CreateDepartmentCommand(request.DepartmentName, request.CoordinatorId);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Value);
    }
}