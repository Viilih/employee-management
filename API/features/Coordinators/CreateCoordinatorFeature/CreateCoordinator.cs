using API.Database;
using API.Domain.Entities;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.features.Coordinators.CreateCoordinatorFeature;

public record CreateCoordinatorCommand(string FirstName, string LastName, string Email)
    : IRequest<Result<Coordinator>>;

public class CreateCoordinatorValidator : AbstractValidator<CreateCoordinatorCommand>
{
    private readonly ApplicationDbContext _context;

    public CreateCoordinatorValidator(ApplicationDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name cannot be empty")
            .MaximumLength(20)
            .WithMessage("First name cannot be longer than 20 characters");
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name cannot be empty")
            .MaximumLength(20)
            .WithMessage("Last name cannot be longer than 20 characters");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty")
            .EmailAddress()
            .WithMessage("Invalid email address")
            .MaximumLength(30)
            .WithMessage("Email must not exceed 30 characters")
            .MustAsync(BeUniqueEmail).WithMessage("Email already exists");
    }
    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _context.Coordinators
            .AnyAsync(c => c.Email == email, cancellationToken);
    }
}

public class CreateCoordinatorHandler : IRequestHandler<CreateCoordinatorCommand, Result<Coordinator>>
{
    private readonly IValidator<CreateCoordinatorCommand> _validator;
    private readonly ApplicationDbContext _context;

    public CreateCoordinatorHandler(IValidator<CreateCoordinatorCommand> validator, ApplicationDbContext context)
    {
        _validator = validator;
        _context = context;
    }
    public async Task<Result<Coordinator>> Handle(CreateCoordinatorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage);
            return Result.Fail<Coordinator>(errors);
        }

        var coordinator = new Coordinator
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };
        _context.Coordinators.Add(coordinator);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Ok<Coordinator>(coordinator);
    }
}
