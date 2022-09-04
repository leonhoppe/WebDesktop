using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers; 

[ApiController]
public class TestController : ControllerBase {

    private DatabaseContext _context;

    public TestController(DatabaseContext context) {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult InitializeDb() {
        _context.ExecuteTableCreation();
        return Ok("OK");
    }
    
}