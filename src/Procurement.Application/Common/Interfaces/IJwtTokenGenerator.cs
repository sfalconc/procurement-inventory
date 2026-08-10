using Procurement.Domain.Entities;

namespace Procurement.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}