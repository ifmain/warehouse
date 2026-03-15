namespace warehouse.Shared.Models;

public class ItemDto
{
    public int Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public float Price { get; set; }
    
    public int ManufacturerID { get; set; }
    public int AppliancesID { get; set; }
    public int CountryID { get; set; }
    public int ImageID { get; set; }

    public string ManufacturerName { get; set; } = string.Empty;
    public string ApplianceName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    
    public string CreatedAt { get; set; } = string.Empty;
    public string UpdatedAt { get; set; } = string.Empty;
}