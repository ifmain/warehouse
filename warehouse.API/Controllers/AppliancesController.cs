using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using warehouse.API.Data;
using warehouse.Shared.Models;

using Microsoft.AspNetCore.SignalR;
using warehouse.API.Hubs;

namespace warehouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppliancesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<UpdateHub> _hubContext; // Для работы SignalR

    public AppliancesController(AppDbContext context, IHubContext<UpdateHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DatabaseAppliances>>> GetAppliances()
    {
        return await _context.Appliances.ToListAsync();
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<DatabaseAppliances>> GetAppliance(int id)
    {
        var appliance = await _context.Appliances.FindAsync(id);
        if (appliance == null) return NotFound();
        return appliance;
    }

    
    [HttpPost]
    public async Task<ActionResult<DatabaseAppliances>> PostAppliance(DatabaseAppliances appliance)
    {
        appliance.CreatedAt = DateTime.UtcNow;
        appliance.UpdatedAt = DateTime.UtcNow;

        _context.Appliances.Add(appliance);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("DataChanged");
        return CreatedAtAction(nameof(GetAppliance), new { id = appliance.Id }, appliance);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAppliance(int id, DatabaseAppliances appliance)
    {
        if (id != appliance.Id) return BadRequest();

        var existing = await _context.Appliances.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = appliance.Name;
        existing.Description = appliance.Description;
        existing.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Appliances.Any(e => e.Id == id)) return NotFound();
            throw;
        }

        await _hubContext.Clients.All.SendAsync("DataChanged");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppliance(int id)
    {
        var appliance = await _context.Appliances.FindAsync(id);
        if (appliance == null) return NotFound();

        _context.Appliances.Remove(appliance);
        await _context.SaveChangesAsync();
        
        await _hubContext.Clients.All.SendAsync("DataChanged");
        return NoContent();
    }
}