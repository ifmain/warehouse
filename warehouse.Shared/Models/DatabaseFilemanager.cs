namespace warehouse.Shared.Models;
using warehouse.Shared.Interfaces;

public class DatabaseFilemanager : IDatabaseFilemanager
{
    public int Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}