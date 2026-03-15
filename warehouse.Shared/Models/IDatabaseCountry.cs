namespace warehouse.Shared.Interfaces;

public interface IDatabaseCountry
{
    int Id { get; set; }
    string Name { get; set; }
    DateTime CreatedAt { get; set; }
    string GetFormattedDate() => CreatedAt.ToString("HH:mm:ss dd.MM.yyyy");
}