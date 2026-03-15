using Microsoft.EntityFrameworkCore;
using warehouse.API.Data;

namespace warehouse.API.Services;

public class FileService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public FileService(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    public async Task DeleteFileAsync(int fileId)
    {
        var fileRecord = await _context.Files.FindAsync(fileId);
        if (fileRecord == null) return;

        string root = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        string cleanRelativePath = fileRecord.Path.TrimStart('/');
        string fullPath = Path.Combine(root, cleanRelativePath);

        if (System.IO.File.Exists(fullPath))
        {
            System.IO.File.Delete(fullPath);
        }

        _context.Files.Remove(fileRecord);
        await _context.SaveChangesAsync();
    }
}