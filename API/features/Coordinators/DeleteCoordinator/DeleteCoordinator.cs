using API.Database;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.features.Coordinators.DeleteCoordinator;

public record DeleteCoordinatorCommand(int Id) :  IRequest<Result<int>>;

public class DeleteCoordinatorValidator : AbstractValidator<DeleteCoordinatorCommand>
{
    private readonly ApplicationDbContext _context;

    public DeleteCoordinatorValidator(ApplicationDbContext context)
    {
        _context = context;

        RuleFor(e => e.Id).GreaterThan(0).WithMessage("Invalid Id");
    }

    private async Task<bool> GetCoordinatorToDelete(int coordinatorId)
    {
        var coordinator = await _context.Coordinators.FirstOrDefaultAsync(e  => e.CoordinatorId == coordinatorId );
        return coordinator != null;
    }
    
}

public class DeleteCoordinatorHandler : IRequestHandler<DeleteCoordinatorCommand, Result<int>>
{
    private readonly ApplicationDbContext _context;
    private readonly IValidator<DeleteCoordinatorCommand> _validator;

    public DeleteCoordinatorHandler(ApplicationDbContext context, IValidator<DeleteCoordinatorCommand> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<int>> Handle(DeleteCoordinatorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Fail(errors);
        }
        
        var coordinatorToDelete = await _context.Coordinators.FirstOrDefaultAsync(c => c.CoordinatorId == request.Id);
        if (coordinatorToDelete == null)
        {
            return Result.Fail("Coordinator not found");
        }
        
        var resultFromDelete= _context.Remove(coordinatorToDelete);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok(resultFromDelete.Entity.CoordinatorId);
    } 
    
}