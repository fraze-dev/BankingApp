using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly Bank _bank;

    public AuthController(Bank bank)
    {
        _bank = bank;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and password are required.");
        }

        User user = _bank.Authenticate(request.Username, request.Password);

        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var response = new LoginResponse
        {
            Username = user.Username,
            Role = user.GetRole()
        };

        return Ok(response);
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Username { get; set; }
    public string Role { get; set; }
}