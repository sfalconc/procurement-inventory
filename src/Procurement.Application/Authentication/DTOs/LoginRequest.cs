namespace Procurement.Application.Authentication.DTOs;

public record LoginRequest(
    string Username,
    string Password);