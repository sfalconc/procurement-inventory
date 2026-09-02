using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Procurement.Application.Authentication.DTOs;
using Procurement.Application.Common.Interfaces;

namespace Procurement.Application.Authentication.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => u.UserName == request.Username &&
                     u.IsActive,
                cancellationToken);

        if (user is null ||
            !_passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash))
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