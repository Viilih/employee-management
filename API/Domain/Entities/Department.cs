using System.Text.Json.Serialization;

namespace API.Domain.Entities;

public class Department
{
    public int DepartmentId { get; set; }
    
    public string DepartmentSector { get; set; }
    
    public int CoordinatorId { get; set; }

    [JsonIgnore]
    public virtual Coordinator Coordinator { get; set; } = null!;
    public IEnumerable<Employee> Employees { get; set; } = new List<Employee>();
    
    
}