using Procurement.Application.Common.Interfaces;

namespace Procurement.UnitTests.Common;

public class TestCurrentUserService : ICurrentUserService
{
    public string? UserId => "TestUser";
}