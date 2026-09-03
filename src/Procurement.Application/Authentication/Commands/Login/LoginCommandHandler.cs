using MediatR;
using Microsoft.Extensions.Logging;
using Procurement.Application.Authentication.DTOs;
using Procurement.Application.Common.Interfaces;

namespace Procurement.Application.Authentication.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityUserAuthenticator _identityUserAuthenticator;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IIdentityUserAuthenticator identityUserAuthenticator,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<LoginCommandHandler> logger)
    {
        _identityUserAuthenticator = identityUserAuthenticator;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _identityUserAuthenticator.ValidateCredentialsAsync(
            request.Username,
            request.Password,
            cancellationToken);

        if (user is null)
        {
            _logger.LogWarning(
                "UserLoginFailed {Username}",
                request.Username);

            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        _logger.LogInformation(
            "UserLoginSucceeded {UserId} {Username}",
            user.Id,
            user.UserName);

        return new LoginResponse(token);
    }
}