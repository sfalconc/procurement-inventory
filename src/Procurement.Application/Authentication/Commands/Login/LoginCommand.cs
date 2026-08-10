using MediatR;
using Procurement.Application.Authentication.DTOs;

namespace Procurement.Application.Authentication.Commands.Login;

public record LoginCommand(
    string Username,
    string Password)
    : IRequest<LoginResponse>;