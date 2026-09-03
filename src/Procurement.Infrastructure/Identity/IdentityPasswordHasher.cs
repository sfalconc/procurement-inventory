using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace Procurement.Infrastructure.Identity;

public sealed class IdentityPasswordHasher
    : IPasswordHasher<Procurement.Domain.Entities.User>
{
    private readonly PasswordHasher<Procurement.Domain.Entities.User> _identityHasher = new();
    private readonly PasswordHasher _legacyHasher = new();

    public string HashPassword(
        Procurement.Domain.Entities.User user,
        string password)
    {
        return _identityHasher.HashPassword(user, password);
    }

    public PasswordVerificationResult VerifyHashedPassword(
        Procurement.Domain.Entities.User user,
        string hashedPassword,
        string providedPassword)
    {
        if (TryParseLegacyHash(hashedPassword))
        {
            try
            {
                return _legacyHasher.VerifyPassword(providedPassword, hashedPassword)
                    ? PasswordVerificationResult.SuccessRehashNeeded
                    : PasswordVerificationResult.Failed;
            }
            catch (ArgumentException)
            {
                return PasswordVerificationResult.Failed;
            }
            catch (FormatException)
            {
                return PasswordVerificationResult.Failed;
            }
            catch (CryptographicException)
            {
                return PasswordVerificationResult.Failed;
            }
        }

        return _identityHasher.VerifyHashedPassword(
            user,
            hashedPassword,
            providedPassword);
    }

    private static bool TryParseLegacyHash(string hashedPassword)
    {
        var parts = hashedPassword.Split('.');

        if (parts.Length != 3 ||
            !int.TryParse(parts[0], out var iterations) ||
            iterations <= 0 ||
            string.IsNullOrEmpty(parts[1]) ||
            string.IsNullOrEmpty(parts[2]))
        {
            return false;
        }

        try
        {
            Convert.FromBase64String(parts[1]);
            Convert.FromBase64String(parts[2]);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
