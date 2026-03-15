using Microsoft.AspNetCore.Mvc;
using warehouse.Shared;

[ApiController]
[Route("api/clicker")]
public class WarehouseController : ControllerBase
{
    private static int _currentValue = 0;

    [HttpGet]
    public WarehouseStats Get() => new(_currentValue);

    [HttpPost]
    public IActionResult Post([FromBody] WarehouseStats stats)
    {
        _currentValue = stats.Value;
        return Ok();
    }
}