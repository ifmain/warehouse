using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using warehouse.API.Data;
using warehouse.Shared.Models;
using Microsoft.AspNetCore.SignalR;

using warehouse.API.Hubs;
using warehouse.API.Services;

namespace warehouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<UpdateHub> _hubContext;
    private readonly FileService _fileService;

    public ItemsController(
        AppDbContext context, 
        IHubContext<UpdateHub> hubContext, 
        FileService fileService) 
    {
        _context = context;
        _hubContext = hubContext;
        _fileService = fileService; 
    }

    [HttpGet("filters/appliances")]
    public async Task<ActionResult<IEnumerable<DatabaseAppliances>>> GetAvailableAppliances([FromQuery] int? manufacturerId)
    {
        var query = _context.Items.AsQueryable();
        
        if (manufacturerId.HasValue && manufacturerId > 0)
            query = query.Where(i => i.ManufacturerID == manufacturerId);

        var applianceIds = await query.Select(i => i.AppliancesID).Distinct().ToListAsync();
        
        return await _context.Appliances.Where(a => applianceIds.Contains(a.Id)).ToListAsync();
    }

    [HttpGet("filters/manufacturers")]
    public async Task<ActionResult<IEnumerable<DatabaseManufacturer>>> GetAvailableManufacturers([FromQuery] int? applianceId)
    {
        var query = _context.Items.AsQueryable();
        
        if (applianceId.HasValue && applianceId > 0)
            query = query.Where(i => i.AppliancesID == applianceId);

        var manufacturerIds = await query.Select(i => i.ManufacturerID).Distinct().ToListAsync();
        
        return await _context.Manufacturers.Where(m => manufacturerIds.Contains(m.Id)).ToListAsync();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ItemDto>>> GetItems(
        [FromQuery] int? manufacturerId,
        [FromQuery] int? applianceId,
        [FromQuery] string? search,
        [FromQuery] float? minPrice,
        [FromQuery] float? maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Items.AsQueryable();

        if (manufacturerId.HasValue && manufacturerId > 0)
            query = query.Where(i => i.ManufacturerID == manufacturerId);

        if (applianceId.HasValue && applianceId > 0)
            query = query.Where(i => i.AppliancesID == applianceId);

        if (!string.IsNullOrEmpty(search))
        {
            var s = search.ToLower();
            query = query.Where(i =>
                i.Model.ToLower().Contains(s) ||
                (i.Appliance != null && i.Appliance.Name.ToLower().Contains(s)) ||
                (i.Manufacturer != null && i.Manufacturer.Name.ToLower().Contains(s)));
        }

        if (minPrice.HasValue)
            query = query.Where(i => i.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(i => i.Price <= maxPrice.Value);

        int totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new ItemDto
            {
                Id = i.Id,
                Model = i.Model,
                Price = i.Price,
                ManufacturerID = i.ManufacturerID,
                AppliancesID = i.AppliancesID,
                CountryID = i.CountryID,
                ImageID = i.ImageID,
                ManufacturerName = i.Manufacturer != null ? i.Manufacturer.Name : "—",
                ApplianceName = i.Appliance != null ? i.Appliance.Name : "—",
                CountryName = i.Country != null ? i.Country.Name : "—",
                ImagePath = i.Image != null ? i.Image.Path : "",
                CreatedAt = i.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
                UpdatedAt = i.UpdatedAt.ToString("dd.MM.yyyy HH:mm")
            })
            .ToListAsync();

        return new PagedResult<ItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetItem(int id)
    {
        var item = await _context.Items
            .Where(i => i.Id == id)
            .Select(i => new ItemDto
            {
                Id = i.Id,
                Model = i.Model,
                Price = i.Price,
                ManufacturerName = i.Manufacturer != null ? i.Manufacturer.Name : "—",
                ApplianceName = i.Appliance != null ? i.Appliance.Name : "—",
                CountryName = i.Country != null ? i.Country.Name : "—",
                ImagePath = i.Image != null ? i.Image.Path : "",
                CreatedAt = i.CreatedAt.ToString("dd.MM.yyyy HH:mm")
            })
            .FirstOrDefaultAsync();

        if (item == null) return NotFound();
        return item;
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostItem([FromBody] ItemMutateDto dto)
    {
        var item = new DatabaseItem
        {
            AppliancesID = dto.AppliancesID,
            ManufacturerID = dto.ManufacturerID,
            CountryID = dto.CountryID,
            ImageID = dto.ImageID,
            Model = dto.Model,
            Price = dto.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("DataChanged");

        return CreatedAtAction(nameof(GetItem), new { id = item.Id }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutItem(int id, [FromBody] ItemMutateDto dto)
    {
        var existingItem = await _context.Items.FindAsync(id);
        if (existingItem == null) return NotFound($"Товар с ID {id} не найден");

        existingItem.ManufacturerID = dto.ManufacturerID;
        existingItem.AppliancesID = dto.AppliancesID;
        existingItem.CountryID = dto.CountryID;
        existingItem.ImageID = dto.ImageID;
        existingItem.Price = dto.Price;
        existingItem.Model = dto.Model;
        existingItem.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("DataChanged");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _context.Items.FindAsync(id);
        if (item == null) return NotFound();

        int fileId = item.ImageID;

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        
        if (fileId > 0)
        {
            await _fileService.DeleteFileAsync(fileId);
        }

        await _hubContext.Clients.All.SendAsync("DataChanged");
        return NoContent();
    }
}