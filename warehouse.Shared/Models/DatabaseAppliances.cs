namespace warehouse.Shared.Models;
using warehouse.Shared.Interfaces;

public class DatabaseAppliances : IDatabaseAppliances
{
    public int Id { get; set; }
    public string Name { get; set; }  = string.Empty;
    public string Description { get; set; }  = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}