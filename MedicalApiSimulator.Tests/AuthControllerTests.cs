using Xunit;
using MedicalApiSimulator.Controllers;
using MedicalApiSimulator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Security.Principal;

public class AuthControllerTests
{
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Jwt:Key", "012345678901234567890123345754656456"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _controller = new AuthController(configuration);
    }

    [Theory]
    [InlineData("doctor", "med123", "Admin")]
    [InlineData("user", "userpass", "User")]
    public void Login_ReturnsToken_WhenCredentialsAreValid(string username, string password, string expectedRole)
    {
        var loginModel = new LoginModel { Username = username, Password = password };

        var result = _controller.Login(loginModel) as OkObjectResult;

        Assert.NotNull(result);
        var token = result.Value.GetType().GetProperty("token")?.GetValue(result.Value) ??
                    result.Value.GetType().GetProperty("Token")?.GetValue(result.Value);
        Assert.NotNull(token);
        Assert.IsType<string>(token);
    }

    [Fact]
    public void Login_ReturnsUnauthorized_WhenCredentialsInvalid()
    {
        var loginModel = new LoginModel { Username = "baduser", Password = "badpass" };

        var result = _controller.Login(loginModel);

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void SecureEndpoint_ReturnsOk_WhenUserIsAuthorized()
    {
        // Arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.Name, "doctor"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };

        // Act
        var result = _controller.GetSecureVitals();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("You are authorized!", okResult.Value);
    }
}
