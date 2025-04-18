using API.Database;
using API.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.features.Departments.ListDepartments;

public record ListDepartmentQuery() : IRequest<Result<IReadOnlyList<Department>>>;

public class ListDepartmentHandler : IRequestHandler<ListDepartmentQuery, Result<IReadOnlyList<Department>>> 
{
    private readonly ApplicationDbContext _context;
    public ListDepartmentHandler(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result<IReadOnlyList<Department>>> Handle(ListDepartmentQuery request
        , CancellationToken cancellationToken)
    {
        var departments = await _context.Departments.Include(dep => dep.Employees)
            .ToListAsync(cancellationToken: cancellationToken);
        return Result.Ok((IReadOnlyList<Department>)departments);
    }
}