using AuthProject.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthProject.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Many-to-Many: User <-> Role
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // Many-to-Many: Role <-> Permission
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Email Unique Constraint
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // --- SEED DATA  ---

        // 1. Define Permissions
        var permissions = new List<Permission>
        {
            new Permission { Id = 1, Name = "Permissions.Users.Read", Description = "Can view user details" },
            new Permission { Id = 2, Name = "Permissions.Users.Edit", Description = "Can edit user information" },
            new Permission { Id = 3, Name = "Permissions.Users.Delete", Description = "Can delete users from system" }
        };
        modelBuilder.Entity<Permission>().HasData(permissions);

        // 2. Define Roles
        var adminRole = new Role { Id = 1, Name = "Admin", Description = "Full system administrator with all privileges" };
        var managerRole = new Role { Id = 2, Name = "Manager", Description = "Management staff with limited administrative access" };
        var userRole = new Role { Id = 3, Name = "User", Description = "Standard registered user" };
        
        modelBuilder.Entity<Role>().HasData(adminRole, managerRole, userRole);

        // 3. Define Role-Permission Assignments
        modelBuilder.Entity<RolePermission>().HasData(
            // Admin: Has all permissions
            new RolePermission { RoleId = 1, PermissionId = 1 },
            new RolePermission { RoleId = 1, PermissionId = 2 },
            new RolePermission { RoleId = 1, PermissionId = 3 },
            
            // Manager: Can read and edit, but not delete
            new RolePermission { RoleId = 2, PermissionId = 1 },
            new RolePermission { RoleId = 2, PermissionId = 2 },

            // User: Can only read (Optional, usually users only see their own data)
            new RolePermission { RoleId = 3, PermissionId = 1 }
        );
    }
}