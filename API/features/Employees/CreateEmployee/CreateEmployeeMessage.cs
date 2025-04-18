using API.Domain.Entities;

namespace API.features.Employees.CreateEmployee;

public class CreateEmployeeMessage
{
  public int EmployeeId { get; set; }
  public string EmployeeFirstName { get; set; }
  public string EmployeeLastName { get; set; }
  public string EmployeeEmail { get; set; }
  public int DepartmentId { get; set; }
  public string DepartmentSector { get; set; }
  public int? CoordinatorId { get; set; } 
  public string CoordinatorFirstName { get; set; }
  public string CoordinatorLastName { get; set; }
  public string CoordinatorEmail { get; set; }
}