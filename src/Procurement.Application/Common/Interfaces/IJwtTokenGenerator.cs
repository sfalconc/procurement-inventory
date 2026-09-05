using Procurement.Domain.Entities;

namespace Procurement.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(
        User user,
        CancellationToken cancellationToken = default);
}