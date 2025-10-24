using Microsoft.EntityFrameworkCore;
using CMCS_Prototype.Models;

namespace CMCS_Prototype.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        // Added DbSets for application entities so controllers/services can access them via _context.<Name>
        public DbSet<Claim> Claims { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Coordinator> Coordinators { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<TeachingRequest> TeachingRequests { get; set; }
        public DbSet<Approval> Approvals { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed demo users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "lecturer@cmcs.edu",
                    PasswordHash = "demo",
                    Role = "lecturer",
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    UserID = 2,
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Email = "coordinator@cmcs.edu",
                    PasswordHash = "demo",
                    Role = "coordinator",
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    UserID = 3,
                    FirstName = "Michael",
                    LastName = "Brown",
                    Email = "manager@cmcs.edu",
                    PasswordHash = "demo",
                    Role = "manager",
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}