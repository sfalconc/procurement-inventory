using Procurement.Domain.Entities;

namespace Procurement.Application.Common.Interfaces;

public interface IIdentityUserAuthenticator
{
    Task<User?> ValidateCredentialsAsync(
        string username,
        string password,
        CancellationToken cancellationToken);
}
