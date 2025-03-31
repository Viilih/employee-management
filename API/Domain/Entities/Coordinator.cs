namespace API.Domain.Entities;

public class Coordinator
{
    public int CoordinatorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
    public ICollection<Department> Departments { get; } = new List<Department>();

}