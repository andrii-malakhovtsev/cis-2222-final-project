using Microsoft.EntityFrameworkCore;

namespace Final_Project.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; } = null!;

        public DbSet<SalesData> SalesData { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique employee constraint
            modelBuilder.Entity<Employee>()
                .HasIndex(e => new { e.Firstname, e.Lastname, e.DOB })
                .IsUnique();

            // Unique sales constraint
            modelBuilder.Entity<SalesData>()
                .HasIndex(s => new { s.EmployeeId, s.Quarter, s.Year })
                .IsUnique();

            // Set precision for Amount
            modelBuilder.Entity<SalesData>()
                .Property(s => s.Amount)
                .HasColumnType("decimal(18,2)");  // precision to 18 and scale to 2

            // Configure the relationship between SalesData and Employee
            modelBuilder.Entity<SalesData>()
                .HasOne(sd => sd.Employee)
                .WithMany(e => e.Sales)
                .HasForeignKey(sd => sd.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship between Employee and Manager (self-referencing)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed manager
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    EmployeeId = 1,
                    Firstname = "Joyce",
                    Lastname = "Valdez",
                    DOB = new DateTime(1956, 12, 10),
                    DateOfHire = new DateTime(1995, 1, 1),
                    ManagerId = null
                });
        }
    }
}