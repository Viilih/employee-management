using API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
    
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Coordinator> Coordinators { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId);

            entity.Property(dep => dep.DepartmentId).HasColumnName("DEPARTMENT_ID");
            entity.Property(dep => dep.DepartmentSector).HasColumnName("DEPARTMENT_SECTOR");
            entity.Property(dep => dep.CoordinatorId).HasColumnName("DEPARTMENT_COORDINATOR_ID");
            
            entity
                .HasMany<Employee>(e => e.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId);

        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);

            entity.Property(emp => emp.EmployeeId).HasColumnName("EMPLOYEE_ID");
            entity.Property(emp => emp.FirstName).HasColumnName("EMPLOYEE_FIRST_NAME");
            entity.Property(emp => emp.LastName).HasColumnName("EMPLOYEE_LAST_NAME");
            entity.Property(emp => emp.Email).HasColumnName("EMPLOYEE_EMAIL");
            entity.Property(emp => emp.DepartmentId).HasColumnName("EMPLOYEE_DEPARTMENT");
        });

        modelBuilder.Entity<Coordinator>(entity =>
        {
            entity.HasKey(e => e.CoordinatorId);

            entity.Property(coord => coord.CoordinatorId).HasColumnName("COORDINATOR_ID");
            entity.Property(coord => coord.FirstName).HasColumnName("COORDINATOR_FIRST_NAME");
            entity.Property(coord => coord.LastName).HasColumnName("COORDINATOR_LAST_NAME");
            entity.Property(coord => coord.Email).HasColumnName("COORDINATOR_EMAIL");


            entity
                .HasMany<Department>(c => c.Departments)
                .WithOne(d => d.Coordinator)
                .HasForeignKey(d => d.CoordinatorId);

        });
    }
}