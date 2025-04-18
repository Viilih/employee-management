using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.features.Departments.ListDepartments;

[ApiController]
[Route("/api/[controller]")]
[ApiExplorerSettings(GroupName = "Departments")]
public class ListDepartmentsEndpoint : ControllerBase
{
    
    private readonly IMediator _mediator;

    public ListDepartmentsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("/api/[controller]/[action]")]
    public async Task<IActionResult> GetDepartments()
    {
        var query = new ListDepartmentQuery();
        var result = await _mediator.Send(query);
    
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }
    
        return BadRequest(result.Errors);
    }
    
}