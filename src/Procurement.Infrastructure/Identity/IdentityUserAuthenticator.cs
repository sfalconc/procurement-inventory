using Microsoft.AspNetCore.Identity;
using Procurement.Application.Common.Interfaces;
using Procurement.Domain.Entities;

namespace Procurement.Infrastructure.Identity;

public sealed class IdentityUserAuthenticator : IIdentityUserAuthenticator
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public IdentityUserAuthenticator(
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<User?> ValidateCredentialsAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: false);

        return result.Succeeded ? user : null;
    }
}
