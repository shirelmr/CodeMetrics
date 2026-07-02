using FluentValidation;
using MetricsAPI.Data;
using MetricsAPI.DTOs;
using MetricsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MetricsAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IValidator<RegisterUserDto> _validator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        AppDbContext context,
        IValidator<RegisterUserDto> validator,
        ILogger<AuthController> logger)
    {
        _context = context;
        _validator = validator;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        var validation = await _validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var emailTaken = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (emailTaken)
        {
            _logger.LogWarning("Register failed - email already exists: {Email}", dto.Email);
            return Conflict(new { message = "Email already registered." });
        }

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("New user registered: {Email} with role {Role}", user.Email, user.Role);
        return Created($"/api/auth/{user.Id}", new
        {
            message = "Registration successful",
            email = user.Email,
            role = user.Role
        });
    }

}