using Microsoft.AspNetCore.Mvc;

namespace ConfigurationApi.Controllers;
[ApiController]
[Route("configuration")]
public class ConfigurationController : ControllerBase
{
    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey([FromRoute]string key) 
    { 
        await Task.CompletedTask;
        return Ok(new {message = $"get oke: {key}"});
    }
}