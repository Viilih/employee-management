namespace API.Contracts.Employee;

public record CreateEmployeeRequest(string FirstName, string LastName, string Email,int DepartmentId);