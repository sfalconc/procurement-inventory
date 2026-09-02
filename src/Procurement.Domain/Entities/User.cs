using Microsoft.AspNetCore.Identity;
using Procurement.Domain.Enums;

namespace Procurement.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsActive { get; private set; } = true;

    public UserRole Role { get; private set; }

    private User()
    {
    }

    public User(
        string username,
        string passwordHash,
        UserRole role)
    {
        Id = Guid.NewGuid();
        UserName = username;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void Delete()
    {
        IsActive = false;
    }

    public void MarkAsUpdated(string? updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public void UpdateRole(UserRole role)
    {
        Role = role;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }
}