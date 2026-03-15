namespace warehouse.Shared.Models;
using warehouse.Shared.Interfaces;

public class DatabaseItem : IDatabaseItem
{
    public int Id { get; set; }
    
    public int ImageID { get; set; }
    public int AppliancesID { get; set; }
    public int ManufacturerID { get; set; }
    public int CountryID { get; set; }
    public string Model { get; set; } = string.Empty;
    
    public float Price { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DatabaseFilemanager? Image { get; set; }
    public DatabaseAppliances? Appliance { get; set; }
    public DatabaseManufacturer? Manufacturer { get; set; }
    public DatabaseCountry? Country { get; set; }
}