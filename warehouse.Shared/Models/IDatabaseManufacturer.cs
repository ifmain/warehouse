namespace warehouse.Shared.Interfaces;

public interface IDatabaseManufacturer
{
    int Id { get; set; }
    string Name { get; set; }
    string Description { get; set; }

    public int CountryID { get; set; }

    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}