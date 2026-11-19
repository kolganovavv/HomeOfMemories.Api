namespace HomeOfMemories.Api.Models;

public class UiTranslation
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}