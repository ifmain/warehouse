namespace warehouse.Shared.Models;

public class ItemMutateDto
{
    public int AppliancesID { get; set; }
    public int ManufacturerID { get; set; }
    public int CountryID { get; set; }
    public int ImageID { get; set; }
    public string Model { get; set; } = string.Empty;
    public float Price { get; set; }
}