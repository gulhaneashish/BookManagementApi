using BookStoreApi.DTOs;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly TokenService _tokenService;

    public AuthController(
        AuthService authService,
        TokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
    LoginDto dto)
    {
        var user = await _authService.RegisterAsync(
            dto.Username,
            dto.Password);

        if (user == null)
        {
            return BadRequest(
                "Username already exists.");
        }

        return Ok(new
        {
            message = "User registered successfully.",
            username = user.Username,
            role = user.Role
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
     LoginDto dto)
    {
        var user = await _authService.ValidateUserAsync(
            dto.Username,
            dto.Password);

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var accessToken =
            _tokenService.CreateAccessToken(user);

        var refreshToken =
            _tokenService.CreateRefreshToken();

        user.RefreshToken = refreshToken;

        user.RefreshTokenExpiryTime =
            DateTime.UtcNow.AddDays(7);

        await _authService.UpdateUserAsync(user);

        return Ok(new
        {
            accessToken,
            refreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
     RefreshTokenDto dto)
    {
        var user =
            await _authService
                .GetUserByRefreshTokenAsync(
                    dto.RefreshToken);

        if (user == null)
        {
            return Unauthorized(
                "Invalid refresh token.");
        }

        if (user.RefreshTokenExpiryTime <=
            DateTime.UtcNow)
        {
            return Unauthorized(
                "Refresh token has expired.");
        }

        var newAccessToken =
            _tokenService.CreateAccessToken(user);

        var newRefreshToken =
            _tokenService.CreateRefreshToken();

        user.RefreshToken = newRefreshToken;

        user.RefreshTokenExpiryTime =
            DateTime.UtcNow.AddDays(7);

        await _authService.UpdateUserAsync(user);

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken
        });
    }
}