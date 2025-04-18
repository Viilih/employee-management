using System.Data;
using API.Database;
using API.Domain.Entities;
using API.Infra.ServiceBus;
using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.features.Employees.CreateEmployee;

public record CreateEmployeeCommand(string FirstName, string LastName,
    string Email, int DepartmentId) :  IRequest<Result<Employee>>;

public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeCommand>
{
    private readonly ApplicationDbContext _context;

    public CreateEmployeeValidator(ApplicationDbContext context)
    {
        _context = context;

        RuleFor(e => e.FirstName)
            .NotEmpty()
            .WithMessage("First name cannot be empty")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters");
        
        RuleFor(e => e.LastName)
            .NotEmpty()
            .WithMessage("Last name cannot be empty")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters");

        RuleFor(e => e.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty")
            .EmailAddress()
            .WithMessage("Invalid email address")
            .MustAsync(EmailIsUnique)
            .WithMessage("Email already exists");
        
        RuleFor(e =>e.DepartmentId)
            .NotEmpty()
            .WithMessage("Department id cannot be empty")
            .MustAsync(DepartmentExist)
            .WithMessage("Department id does not exist");
    }
    private async Task<bool> EmailIsUnique(string email, CancellationToken cancellationToken){  
         var resultValidationEmail =await _context.Employees.FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
         return resultValidationEmail == null;
    }
    
    private async Task<bool> DepartmentExist(int dpId, CancellationToken cancellationToken){  
        return await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == dpId
            , cancellationToken) != null;
    }
    
}


public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, Result<Employee>>
{
    private readonly ApplicationDbContext _context;
    private readonly IValidator<CreateEmployeeCommand> _validator;
    private readonly IServiceBusSenderService _serviceBusSender;

    public CreateEmployeeHandler(ApplicationDbContext context, IValidator<CreateEmployeeCommand> validator, IServiceBusSenderService serviceBusSender)
    {
        _context = context;
        _validator = validator;
        _serviceBusSender = serviceBusSender;
    }

    public async Task<Result<Employee>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage);
            return Result.Fail(errors);
        }

        var employeeToAdd = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            DepartmentId = request.DepartmentId
        };
        
        await _context.Employees.AddAsync(employeeToAdd, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        var coordinatorResponsibleForEmployeeDepartment = await GetCoordinator(employeeToAdd.DepartmentId, cancellationToken);
        var employeeDepartment = await _context.Departments.AsNoTracking().FirstOrDefaultAsync(d 
            => d.DepartmentId == employeeToAdd.DepartmentId, cancellationToken);
        if (employeeDepartment == null) {
            return Result.Fail($"Department with ID {employeeToAdd.DepartmentId} not found.");
        }
        var employeeDepartmentSector = employeeDepartment.DepartmentSector;
        var messageToQueue = new CreateEmployeeMessage
        {                                                                                                                                                                                                                                                                                                                                           
            EmployeeId = employeeToAdd.EmployeeId, 
            EmployeeFirstName = employeeToAdd.FirstName,
            EmployeeLastName = employeeToAdd.LastName,
            EmployeeEmail = employeeToAdd.Email,
            DepartmentId = employeeToAdd.DepartmentId,
            DepartmentSector = employeeDepartmentSector,
            CoordinatorId = coordinatorResponsibleForEmployeeDepartment.CoordinatorId, 
            CoordinatorEmail = coordinatorResponsibleForEmployeeDepartment.Email,
            CoordinatorFirstName = coordinatorResponsibleForEmployeeDepartment.FirstName,
            CoordinatorLastName = coordinatorResponsibleForEmployeeDepartment.LastName,
        };
        
        await _serviceBusSender.SendAsync(messageToQueue, cancellationToken);
        return Result.Ok<Employee>(employeeToAdd);
    }                                                                                                               

    private async Task<Coordinator> GetCoordinator(int employeeDepartmentId, CancellationToken cancellationToken)
    {               
        var employeeDepartment = await _context.Departments.AsNoTracking().FirstOrDefaultAsync(d => 
            d.DepartmentId == employeeDepartmentId, cancellationToken);
        
        var coordinator = await _context.Coordinators.AsNoTracking().FirstOrDefaultAsync(c => 
            employeeDepartment != null && c.Departments.Contains(employeeDepartment), cancellationToken);
        
        return coordinator!;
    }
}