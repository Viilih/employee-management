using API.Database;
using API.Domain.Entities;
using FluentResults;
using FluentValidation;
using MediatR;

namespace API.features.Coordinators.GetCoordinatorByIdFeature;

public record GetCoordinatorByIdQuery(int Id) : IRequest<Result<Coordinator>>;

public class GetCoordinatorByIdQueryValidator : AbstractValidator<GetCoordinatorByIdQuery>
{
    public GetCoordinatorByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty()
            .WithMessage("Id cannot be empty").GreaterThan(0)
            .WithMessage("Id must be greater than 0");
    }
    
}


public class GetCoordinatorByIdQueryHandler : IRequestHandler<GetCoordinatorByIdQuery, Result<Coordinator>?>
{
    private readonly IValidator<GetCoordinatorByIdQuery> _validator;
    private readonly ApplicationDbContext _context;
    
    public GetCoordinatorByIdQueryHandler(IValidator<GetCoordinatorByIdQuery> validator, ApplicationDbContext context)
    {
        _validator = validator;
        _context = context;
    }

    public async Task<Result<Coordinator>?> Handle(GetCoordinatorByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage);
            return Result.Fail<Coordinator>(errors);
        }
        var coordinator = _context.Coordinators.FirstOrDefault(c => c.CoordinatorId == request.Id);
        
        if (coordinator is null)
        {
            return Result.Fail<Coordinator>("Coordinator not found.");
        }
        return Result.Ok(coordinator);
    }
}