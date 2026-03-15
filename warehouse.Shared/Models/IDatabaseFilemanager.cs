namespace warehouse.Shared.Interfaces;

public interface IDatabaseFilemanager
{
    int Id { get; set; }
    string Path { get; set; }
    DateTime CreatedAt { get; set; }
    string GetFormattedDate() => CreatedAt.ToString("HH:mm:ss dd.MM.yyyy");
}