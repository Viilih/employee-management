using API.Database;
using API.Domain.Entities;
using FluentResults;
using FluentValidation;
using MediatR;

namespace API.features.Departments.AssignDepartmentToAnotherCoordinator;

public record AssignDepartmentToAnotherCoordinatorCommand(int NewCoordinatorId, int DepartmentId)
    : IRequest<Result<Department>>;

public class AssignDepartmentToAnotherCoordinatorValidator :
    AbstractValidator<AssignDepartmentToAnotherCoordinatorCommand>
{
    private readonly ApplicationDbContext _context;

    public AssignDepartmentToAnotherCoordinatorValidator(ApplicationDbContext context)
    {
        _context = context;
        
        RuleFor(d => d.DepartmentId).GreaterThan(0);
        RuleFor(d => d.NewCoordinatorId).MustAsync((async (i, token) =>
        {
            var coord = await _context.Coordinators.FindAsync(i);
            return coord != null;
        }));
    }
    
}

public class AssignDepartmentToCoordinatorHandler : IRequestHandler<AssignDepartmentToAnotherCoordinatorCommand,
    Result<Department>>
{
    private readonly ApplicationDbContext _context;
    private readonly IValidator<AssignDepartmentToAnotherCoordinatorCommand> _validator;

    public AssignDepartmentToCoordinatorHandler(ApplicationDbContext context, 
        IValidator<AssignDepartmentToAnotherCoordinatorCommand> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<Department>> Handle(AssignDepartmentToAnotherCoordinatorCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail("Invalid request");
        }
        
        var department = await _context.Departments.FindAsync(request.DepartmentId);
        var coordinatorToAssignDepart = await _context.Coordinators.FindAsync(request.NewCoordinatorId);

        if (department == null)
        {
            return Result.Fail("Department not found");
        }
        
        if (coordinatorToAssignDepart == null)
        {
            return Result.Fail("Coordinator not found");
        }

   
        department.CoordinatorId = coordinatorToAssignDepart!.CoordinatorId;
        _context.Departments.Update(department);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok<Department>(department);
    }
}