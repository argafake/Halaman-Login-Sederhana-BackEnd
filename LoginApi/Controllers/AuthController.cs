// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace LoginApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly string _connectionString;

    public AuthController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "SELECT password_hash FROM users WHERE username = @username", conn);
        cmd.Parameters.AddWithValue("username", request.Username);

        var result = cmd.ExecuteScalar();

        if (result == null)
            return BadRequest("User not found");

        // ⚠️ Ini hanya untuk demo! Jangan bandingkan password plain di proyek nyata!
        if (result.ToString() == request.Password)
            return Ok(new { message = "Login success!" });
        else
            return BadRequest("Wrong password");
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}