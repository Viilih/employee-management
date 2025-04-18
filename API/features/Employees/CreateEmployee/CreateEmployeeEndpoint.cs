using API.Contracts.Employee;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.features.Employees.CreateEmployee;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Employees")]

public class CreateEmployeeEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateEmployeeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("/api/[controller]/[action]")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        var command = new CreateEmployeeCommand(request.FirstName, request.LastName, request.Email, request.DepartmentId);
        var result = await _mediator.Send(command);
        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }
        
        return Ok(result.Value);
    }
}