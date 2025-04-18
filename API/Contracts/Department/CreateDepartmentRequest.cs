namespace API.Contracts.Department;

public record CreateDepartmentRequest(string DepartmentName, int CoordinatorId);