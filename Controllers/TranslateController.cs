using HomeOfMemories.Api.Data;
using HomeOfMemories.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeOfMemories.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // базовый маршрут: api/translate
public class TranslateController : ControllerBase
{
    private readonly TranslateService _translateService;
    private readonly AppDbContext _db;

    public TranslateController(TranslateService translateService, AppDbContext db)
    {
        _translateService = translateService;
        _db = db;
    }

    // GET: api/translate?text=Привет&lang=en
    [HttpGet]
    public async Task<IActionResult> Translate([FromQuery] string text, [FromQuery] string lang = "en")
    {
        if (string.IsNullOrWhiteSpace(text))
            return BadRequest(new { error = "Текст не может быть пустым" });

        var result = await _translateService.TranslateAsync(text, lang);
        return Ok(new { original = text, translated = result, targetLang = lang });
    }

    // GET: api/translate/dictionary?lang=en
    [HttpGet("dictionary")]
    public async Task<IActionResult> GetDictionary([FromQuery] string lang = "en")
    {
        var dict = await _db.UiTranslations
            .Where(t => t.Language == lang)
            .ToDictionaryAsync(t => t.Key, t => t.Text);

        return Ok(dict);
    }
}