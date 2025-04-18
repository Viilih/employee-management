using API.Database;
using API.Domain.Entities;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.features.Coordinators.GetAllCoordinators;

public record GetAllCoordinators() : IRequest<Result<IReadOnlyList<Coordinator>>>;

public class GetAllCoordinatorHandler : IRequestHandler<GetAllCoordinators, Result<IReadOnlyList<Coordinator>>>
{
    private readonly ApplicationDbContext _context;

    public GetAllCoordinatorHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<Coordinator>>> Handle(GetAllCoordinators request,
        CancellationToken cancellationToken)
    {
        var coordinators = await _context.Coordinators
            .Include(d => d.Departments).ToListAsync(cancellationToken);
        return Result.Ok((IReadOnlyList<Coordinator>)coordinators);
    }
}