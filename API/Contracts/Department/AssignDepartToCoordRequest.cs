namespace API.Contracts.Department;

public record AssignDepartToCoordRequest(int NewCoordinatorId, int DepartmentId);