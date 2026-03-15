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
public class FilemanagerController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IHubContext<UpdateHub> _hubContext;
    private readonly FileService _fileService;

    public FilemanagerController(
        AppDbContext context, 
        IWebHostEnvironment env, 
        IHubContext<UpdateHub> hubContext, 
        FileService fileService)
    {
        _context = context;
        _env = env;
        _hubContext = hubContext;
        _fileService = fileService;
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<DatabaseFilemanager>>> GetFiles()
    {
        return await _context.Files.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DatabaseFilemanager>> GetItem(int id)
    {
        var item = await _context.Files.FindAsync(id);
        if (item == null) return NotFound();
        return item;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<DatabaseFilemanager>> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Файл не выбран");

        const long maxFileSizeBytes = 5 * 1024 * 1024;
        if (file.Length > maxFileSizeBytes)
        {
            return BadRequest("Файл слишком большой. Максимальный размер: 5 МБ.");
        }
        
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            return BadRequest("Недопустимый формат файла. Разрешены только: jpg, png, webp, gif.");
        }

        var rootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var uploadsFolder = Path.Combine(rootPath, "uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var dbFile = new DatabaseFilemanager
        {
            Path = $"/uploads/{fileName}",
            CreatedAt = DateTime.UtcNow
        };

        _context.Files.Add(dbFile);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("DataChanged");
        return Ok(dbFile);
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        await _fileService.DeleteFileAsync(id);
        await _hubContext.Clients.All.SendAsync("DataChanged");
        return NoContent();
    }
}