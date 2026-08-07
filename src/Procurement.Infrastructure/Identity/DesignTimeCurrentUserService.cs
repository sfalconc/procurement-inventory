using Procurement.Application.Common.Interfaces;

namespace Procurement.Infrastructure.Identity;

public class DesignTimeCurrentUserService : ICurrentUserService
{
    public string? UserId => "System";
}