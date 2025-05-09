using System.Text.Json.Serialization;

namespace API.Domain.Entities;
public class Employee
{
    public int EmployeeId {get; set;}
    
    public string FirstName {get; set;}
    
    public string LastName {get; set;}
    
    public string Email {get; set;}
    
    public int DepartmentId {get; set;}

    [JsonIgnore]
    public Department Department { get; set; } = null!;
    
}