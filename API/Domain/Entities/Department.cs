namespace API.Domain.Entities;

public class Department
{
    public int DepartmentId { get; set; }
    
    public string DepartmentSector { get; set; }
    
    public int CoordinatorId { get; set; }

    public Coordinator Coordinator { get; set; } = null!;
    public IEnumerable<Employee> Employees { get; set; }
    
    
}