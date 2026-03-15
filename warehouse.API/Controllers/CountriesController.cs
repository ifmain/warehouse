using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using warehouse.API.Data;
using warehouse.Shared.Models;

using Microsoft.AspNetCore.SignalR;
using warehouse.API.Hubs;

namespace warehouse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<UpdateHub> _hubContext;

    public CountriesController(AppDbContext context, IHubContext<UpdateHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DatabaseCountry>>> Get() => 
        await _context.Countries.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<DatabaseCountry>> Get(int id)
    {
        var country = await _context.Countries.FindAsync(id);
        return country == null ? NotFound() : country;
    }

    [HttpPost]
    public async Task<ActionResult<DatabaseCountry>> Post(DatabaseCountry country)
    {
        country.CreatedAt = DateTime.UtcNow;
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();
        
        await _hubContext.Clients.All.SendAsync("DataChanged");
        return CreatedAtAction(nameof(Get), new { id = country.Id }, country);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country == null) return NotFound();
        _context.Countries.Remove(country);
        await _context.SaveChangesAsync();
        
        await _hubContext.Clients.All.SendAsync("DataChanged");
        return NoContent();
    }
}