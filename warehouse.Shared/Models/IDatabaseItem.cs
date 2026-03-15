namespace warehouse.Shared.Interfaces;

public interface IDatabaseItem
{
    int Id { get; set; }
    
    int ImageID { get; set; }
    int AppliancesID { get; set; }
    int ManufacturerID { get; set; }
    int CountryID { get; set; }
    string Model { get; set; }

    float Price { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }

}