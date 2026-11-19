using HomeOfMemories.Api.Data;
using HomeOfMemories.Api.Models;
using HomeOfMemories.Api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// подключение к бд
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// регистрация сервиса для хеширования паролей
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// настройка куки-аутентификации
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// добавление контроллеров и сервиса тестирования
builder.Services.AddHttpClient<TranslateService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// настройка конвейера обработки запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles(); // главная страница по умолчанию
app.UseStaticFiles(); // позволяет отдавать картинки из корневой папки

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();