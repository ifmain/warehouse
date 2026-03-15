namespace warehouse.Shared.Models;
using warehouse.Shared.Interfaces;

public class DatabaseCountry : IDatabaseCountry
{
    public int Id { get; set; }
    public string Name { get; set; }  = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}