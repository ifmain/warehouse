namespace warehouse.Shared.Interfaces;

public interface IDatabaseAppliances
{
    int Id { get; set; }
    string Name { get; set; }
    string Description { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}