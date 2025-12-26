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
            "SELECT password_hash, role FROM users WHERE username = @username", conn);
        cmd.Parameters.AddWithValue("username", request.Username);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return BadRequest(new { message = "User not found" });

        var storedPassword = reader.GetString(0);
        var role = reader.GetString(1);

        // ⚠️ Demo only: compare plain text password
        if (storedPassword != request.Password)
            return BadRequest(new { message = "Wrong password" });

        return Ok(new
        {
            message = "Login success!",
            username = request.Username,
            role = role  // Kirim role ke frontend!
        });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}