using Refit;
using warehouse.Shared.Models;

namespace warehouse.Shared;

public record WarehouseStats(int Value);

public interface IWarehouseApi
{
    [Get("/api/items")]
    Task<PagedResult<ItemDto>> GetItems(
        int? manufacturerId = null, 
        int? applianceId = null, 
        string? search = null, 
        float? minPrice = null,
        float? maxPrice = null,
        int page = 1, 
        int pageSize = 20);

    [Get("/api/items/{id}")]
    Task<ItemDto> GetItem(int id);

    [Post("/api/items")]
    Task CreateItem([Body] ItemMutateDto item);

    [Put("/api/items/{id}")]
    Task UpdateItem(int id, [Body] ItemMutateDto item);

    [Delete("/api/items/{id}")]
    Task DeleteItem(int id);

    [Get("/api/items/filters/appliances")]
    Task<List<DatabaseAppliances>> GetAvailableAppliances(int? manufacturerId = null);

    [Get("/api/items/filters/manufacturers")]
    Task<List<DatabaseManufacturer>> GetAvailableManufacturers(int? applianceId = null);



    [Multipart]
    [Post("/api/Filemanager/upload")]
    Task<DatabaseFilemanager> UploadFile([AliasAs("file")] StreamPart file);

    [Get("/api/appliances")]
    Task<List<DatabaseAppliances>> GetAppliances();

    [Post("/api/appliances")]
    Task<DatabaseAppliances> CreateAppliance([Body] DatabaseAppliances appliance);

    [Delete("/api/appliances/{id}")]
    Task DeleteAppliance(int id);

    [Put("/api/appliances/{id}")]
    Task UpdateAppliance(int id, [Body] DatabaseAppliances appliance);



    [Get("/api/manufacturers")]
    Task<List<DatabaseManufacturer>> GetManufacturers();

    [Post("/api/manufacturers")]
    Task<DatabaseManufacturer> CreateManufacturer([Body] DatabaseManufacturer manufacturer);

    [Delete("/api/manufacturers/{id}")]
    Task DeleteManufacturer(int id);

    [Put("/api/manufacturers/{id}")]
    Task UpdateManufacturer(int id, [Body] DatabaseManufacturer manufacturer);



    [Get("/api/countries")]
    Task<List<DatabaseCountry>> GetCountries();

    [Post("/api/countries")]
    Task<DatabaseCountry> CreateCountry([Body] DatabaseCountry country);

    [Delete("/api/countries/{id}")]
    Task DeleteCountry(int id);

    [Put("/api/countries/{id}")]
    Task UpdateCountry(int id, [Body] DatabaseCountry country);
}