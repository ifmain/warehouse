using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using warehouse.API.Data;
using warehouse.Shared.Models;

using Microsoft.AspNetCore.SignalR;
using warehouse.API.Hubs;

namespace warehouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManufacturersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<UpdateHub> _hubContext;

    public ManufacturersController(AppDbContext context, IHubContext<UpdateHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DatabaseManufacturer>>> Get() => 
        await _context.Manufacturers
            .Include(m => m.Country)
            .ToListAsync();
    
    [HttpGet("{id}")]
    public async Task<ActionResult<DatabaseManufacturer>> Get(int id)
    {
        var manufacturer = await _context.Manufacturers
            .Include(m => m.Country)
            .FirstOrDefaultAsync(m => m.Id == id);

        return manufacturer == null ? NotFound() : manufacturer;
    }

    [HttpPost]
    public async Task<ActionResult<DatabaseManufacturer>> Post(DatabaseManufacturer manufacturer)
    {
        manufacturer.CreatedAt = DateTime.UtcNow;
        manufacturer.UpdatedAt = DateTime.UtcNow;

        _context.Manufacturers.Add(manufacturer);
        await _context.SaveChangesAsync();

        await _context.Entry(manufacturer).Reference(m => m.Country).LoadAsync();

        await _hubContext.Clients.All.SendAsync("DataChanged");
        return CreatedAtAction(nameof(Get), new { id = manufacturer.Id }, manufacturer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, DatabaseManufacturer manufacturer)
    {
        if (id != manufacturer.Id) return BadRequest("ID в URL и объекте не совпадают");

        var existing = await _context.Manufacturers.FindAsync(id);
        if (existing == null) return NotFound();

        if (!_context.Countries.Any(c => c.Id == manufacturer.CountryID))
        {
            return BadRequest("Указанная страна не найдена в справочнике");
        }

        existing.Name = manufacturer.Name;
        existing.Description = manufacturer.Description;
        existing.CountryID = manufacturer.CountryID;
        existing.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Manufacturers.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        await _hubContext.Clients.All.SendAsync("DataChanged");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var manufacturer = await _context.Manufacturers.FindAsync(id);
        if (manufacturer == null) return NotFound();

        _context.Manufacturers.Remove(manufacturer);
        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("DataChanged");
        return NoContent();
    }
}