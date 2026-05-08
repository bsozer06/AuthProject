namespace AuthProject.Models;

public class Permission
{
    public int Id { get; set; }
    public string? Name { get; set; }  // Örn: CanEditUser, CanViewReports
    public string? Description { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}