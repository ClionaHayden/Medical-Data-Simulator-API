using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Controller responsible for user authentication and JWT token generation.
/// Provides endpoints for user login and access to a secured test route.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Authenticates a user and generates a JWT token with appropriate role claims.
    /// For demo purposes, uses hardcoded usernames and passwords.
    /// </summary>
    /// <param name="model">The login credentials.</param>
    /// <returns>JWT token if credentials are valid; Unauthorized otherwise.</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        // For demo: replace with real DB user validation
        if (model.Username == "doctor" && model.Password == "med123")
        {
            var role = "Admin";
            var token = GenerateJwtToken(model.Username, role);
            return Ok(new { token });
        }
        else if (model.Username == "user" && model.Password == "userpass")
        {
            var role = "User";
            var token = GenerateJwtToken(model.Username, role);
            return Ok(new { Token = token });
        }

        return Unauthorized("Invalid credentials");
    }

    /// <summary>
    /// Generates a JWT token containing username and role claims.
    /// Token expires in 1 hour.
    /// </summary>
    /// <param name="username">Username of the authenticated user.</param>
    /// <param name="role">Role assigned to the user.</param>
    /// <returns>Signed JWT token string.</returns>
    private string GenerateJwtToken(string username, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("012345678901234567890123345754656456");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Audience = _config["Jwt:Audience"],    // add this
            Issuer = _config["Jwt:Issuer"],        // and this
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// A sample protected endpoint to test authorization.
    /// Requires a valid JWT token.
    /// </summary>
    /// <returns>A success message if authorized.</returns>
    [Authorize]
    [HttpGet("secure-vitals")]
    public IActionResult GetSecureVitals()
    {
        return Ok("You are authorized!");
    }
}
