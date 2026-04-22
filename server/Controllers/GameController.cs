using Microsoft.AspNetCore.Mvc;

namespace CheckerboardGameApp.Controllers;

[Route("v1/api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    [HttpGet]
    public IActionResult GetGame()
    {
        return Ok("oke");
    }

    [HttpGet("start")]
    public IActionResult Start()
    {
        return Ok("startooo gammuuu");
    }

    [HttpPost("{id}")]
    public IActionResult PostGame(int id)
    {
        var res = new { Id = id, userName = "Checkers" };
        return Ok(res);
    }
}
