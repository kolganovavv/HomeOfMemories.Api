using System.Security.Claims;
using HomeOfMemories.Api.Data;
using HomeOfMemories.Api.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeOfMemories.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _hasher;

    public AuthController(AppDbContext db, IPasswordHasher<User> hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        // проверка, занят ли логин или почта
        if (await _db.Users.AnyAsync(u => u.Login == dto.Login || u.Email == dto.Email))
            return Conflict(new { error = "Логин или email уже занят" });

        // создание пользователя
        var user = new User { Login = dto.Login, Email = dto.Email };

        // хеш пароля
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { id = user.Id, message = "Успешная регистрация" });
    }

    // POST: api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        // поиск пользователя по логину
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == dto.Login);
        if (user == null)
            return Unauthorized(new { error = "Неверный логин или пароль" });

        // проверка пароля
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized(new { error = "Неверный логин или пароль" });

        // создание "удостоверения" пользователя (Claims)
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Login)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        // запись cookie в браузер
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return Ok(new { id = user.Id, login = user.Login, message = "Успешный вход" });
    }

    // POST: api/auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // удаление cookie
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Вы вышли из системы" });
    }

    // GET: api/auth/me — проверка, авторизован ли пользователь
    [HttpGet("me")]
    public IActionResult CheckAuth()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return Ok(new
            {
                authenticated = true,
                login = User.Identity.Name
            });
        }
        return Unauthorized(new { authenticated = false });
    }
}

// классы для приема данных с фронтенда
public record RegisterDto(string Login, string Email, string Password);
public record LoginDto(string Login, string Password);