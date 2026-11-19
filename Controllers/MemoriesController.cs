using System.Security.Claims;
using HomeOfMemories.Api.Data;
using HomeOfMemories.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeOfMemories.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public MemoriesController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // GET: api/memories — получение списка всех опубликованных воспоминаний
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? categoryId)
    {
        var query = _db.Memories
            .Include(m => m.User)
            .Include(m => m.Category)
            .Include(m => m.Photos)
            .Where(m => m.Status == "published");

        if (categoryId.HasValue)
        {
            query = query.Where(m => m.CategoryId == categoryId);
        }

        var list = await query
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new
            {
                m.Id,
                m.Title,
                m.Story,
                m.YearLabel,
                AuthorId = m.UserId,
                Author = m.User!.Login,
                Category = m.Category != null ? m.Category.Name : "Без категории",
                Cover = m.Photos.OrderBy(p => p.SortOrder).Select(p => p.Path).FirstOrDefault(),
                m.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET: api/memories/5 — получение одного воспоминания со всеми фото
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var memory = await _db.Memories
            .Include(m => m.User)
            .Include(m => m.Category)
            .Include(m => m.Photos)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (memory == null) return NotFound(new { error = "Воспоминание не найдено" });

        return Ok(new
        {
            memory.Id,
            memory.Title,
            memory.Story,
            memory.YearLabel,
            AuthorId = memory.UserId,
            Author = memory.User?.Login,
            Category = memory.Category?.Name,
            Photos = memory.Photos.OrderBy(p => p.SortOrder).Select(p => p.Path),
            memory.CreatedAt
        });
    }

    // POST: api/memories — добавление нового воспоминания
    [Authorize] // только для авторизованных пользователей
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateMemoryDto dto)
    {
        // получение id пользователя из cookie
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        var userId = int.Parse(userIdString);

        // создание записи в базе
        var memory = new Memory
        {
            UserId = userId,
            Title = dto.Title,
            Story = dto.Story,
            YearLabel = dto.YearLabel,
            CategoryId = dto.CategoryId,
            Status = "published"
        };

        _db.Memories.Add(memory);
        await _db.SaveChangesAsync();

        // обработка загрузки фотографий
        if (dto.Photos != null && dto.Photos.Count > 0)
        {
            // создание папки wwwroot/uploads, если её нет
            var uploadsDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            Directory.CreateDirectory(uploadsDir);

            var count = Math.Min(dto.Photos.Count, 12); // ограничение в 12 фото
            for (int i = 0; i < count; i++)
            {
                var file = dto.Photos[i];
                if (file.Length > 10 * 1024 * 1024) continue; // максимум 10мб

                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp") continue;

                // генерация уникального имени файла
                var newName = Guid.NewGuid().ToString("N") + ext;
                var filePath = Path.Combine(uploadsDir, newName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _db.Photos.Add(new Photo
                {
                    MemoryId = memory.Id,
                    Path = $"uploads/{newName}", // путь для сохранения в бд
                    SortOrder = i
                });
            }
            await _db.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetById), new { id = memory.Id }, new { id = memory.Id, message = "Воспоминание добавлено" });
    }

    // GET: api/memories/my — получение воспоминания текущего пользователя
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyMemories()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        var userId = int.Parse(userIdString);

        var myMemories = await _db.Memories
            .Include(m => m.Category)
            .Include(m => m.Photos)
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new
            {
                m.Id,
                m.Title,
                m.Story,
                m.YearLabel,
                Category = m.Category != null ? m.Category.Name : "Без категории",
                Cover = m.Photos.OrderBy(p => p.SortOrder).Select(p => p.Path).FirstOrDefault(),
                m.CreatedAt
            })
            .ToListAsync();

        return Ok(myMemories);
    }

    // GET: api/memories/user/5 — получение воспоминания конкретного пользователя
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserMemories(int userId)
    {
        // проверка валидности айди
        var userExists = await _db.Users.AnyAsync(u => u.Id == userId);
        if (!userExists) return NotFound(new { error = "Пользователь не найден" });

        var memories = await _db.Memories
            .Include(m => m.Category)
            .Include(m => m.Photos)
            .Where(m => m.UserId == userId && m.Status == "published")
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new
            {
                m.Id,
                m.Title,
                m.Story,
                m.YearLabel,
                Category = m.Category != null ? m.Category.Name : "Без категории",
                Cover = m.Photos.OrderBy(p => p.SortOrder).Select(p => p.Path).FirstOrDefault(),
                m.CreatedAt
            })
            .ToListAsync();

        return Ok(memories);
    }

    // GET: api/memories/user/5/info — получение информации о пользователе
    [HttpGet("user/{userId}/info")]
    public async Task<IActionResult> GetUserInfo(int userId)
    {
        var user = await _db.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.Id, u.Login, u.Bio })
            .FirstOrDefaultAsync();

        if (user == null) return NotFound();
        return Ok(user);
    }
}

// класс для приема данных из формы
public class CreateMemoryDto
{
    public string Title { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
    public string? YearLabel { get; set; }
    public int? CategoryId { get; set; }
    public List<IFormFile>? Photos { get; set; }
}