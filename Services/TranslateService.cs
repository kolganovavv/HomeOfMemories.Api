using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HomeOfMemories.Api.Data;
using HomeOfMemories.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeOfMemories.Api.Services;

public class TranslateService
{
    private readonly AppDbContext _db;
    private readonly HttpClient _httpClient;

    public TranslateService(AppDbContext db, HttpClient httpClient)
    {
        _db = db;
        _httpClient = httpClient;
    }

    public async Task<string> TranslateAsync(string text, string targetLang)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;

        // вычисление хеша исходного текста
        var sourceHash = ComputeSha256Hash(text);

        // поиск в кеше бд
        var cached = await _db.Translations
            .FirstOrDefaultAsync(t => t.SourceHash == sourceHash
                                   && t.SourceLang == "ru"
                                   && t.TargetLang == targetLang);

        if (cached != null)
        {
            return cached.TranslatedText; // возврат из кеша
        }

        // отсутствие в кеше => MyMemory API
        var url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair=ru|{targetLang}&de=homeofmemories@example.com";

        string translatedText = text; // оригинал по умолчанию

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);

                translatedText = doc.RootElement
                    .GetProperty("responseData")
                    .GetProperty("translatedText")
                    .GetString() ?? text;
            }
            else
            {
                Console.WriteLine($"API вернул ошибку: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            // нет доступа к api => возврат оригинального текста
            Console.WriteLine($"Ошибка подключения к MyMemory: {ex.Message}");
            return text;
        }

        if (string.IsNullOrEmpty(translatedText) || translatedText == text)
            return text;

        // сохранение в кеш
        var translation = new Translation
        {
            SourceHash = sourceHash,
            SourceLang = "ru",
            TargetLang = targetLang,
            SourceText = text,
            TranslatedText = translatedText
        };

        _db.Translations.Add(translation);
        await _db.SaveChangesAsync();

        return translatedText;
    }

    // вспомогательный метод для хеширования
    private string ComputeSha256Hash(string rawData)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        var builder = new StringBuilder();
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}