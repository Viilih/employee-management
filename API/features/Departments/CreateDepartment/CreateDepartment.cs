using API.Database;
using API.Domain.Entities;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.features.Departments.CreateDepartment;

public record CreateDepartmentCommand(string DepartmentSector, int CoordinatorId) : IRequest<Result<Department>>;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    private readonly ApplicationDbContext _context;

    public CreateDepartmentValidator(ApplicationDbContext context)
    {
        _context = context;
        
        RuleFor(d => d.DepartmentSector).NotEmpty().WithMessage("Department sector is required");
        RuleFor(d => d.CoordinatorId).GreaterThan(0).WithMessage("Invalid coordinator Id")
            .MustAsync(CheckIfCoordinatorExist)
            .WithMessage("You must select a coordinator for creating a department");

    }

    private async Task<bool> CheckIfCoordinatorExist(int coordinatorId, CancellationToken cancellationToken)
    {
        var coordinator = await _context.Coordinators
            .FirstOrDefaultAsync(c => c.CoordinatorId == coordinatorId, cancellationToken);
        
        return coordinator != null;
    }
}

public class CreateDepartmentHandler : IRequestHandler<CreateDepartmentCommand, Result<Department>>
{
    private readonly ApplicationDbContext _context;
    private IValidator<CreateDepartmentCommand> _validator;


    public CreateDepartmentHandler(ApplicationDbContext context, IValidator<CreateDepartmentCommand> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<Department>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var validationResult =  await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Fail(errors);
        }

        var department = new Department
        {
            DepartmentSector = request.DepartmentSector,
            CoordinatorId = request.CoordinatorId
        };
        
        _context.Departments.Add(department);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok<Department>(department);
    }
}