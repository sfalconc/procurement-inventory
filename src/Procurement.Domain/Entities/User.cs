using Procurement.Domain.Common;
using Procurement.Domain.Enums;

namespace Procurement.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public UserRole Role { get; private set; }

    private User()
    {
    }

    public User(
        string username,
        string passwordHash,
        UserRole role)
    {
        Username = username;
        PasswordHash = passwordHash;
        Role = role;
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