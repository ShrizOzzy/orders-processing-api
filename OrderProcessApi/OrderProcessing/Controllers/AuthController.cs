using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcess.Core.Models.LoginDto;
using OrderProcess.Application.Services;

namespace OrderProcessApi.OrderProcessing.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequestDto request)
    {
        if (!IsValidUser(request.Username, request.Password))
            return Unauthorized();

        var tokenResponse = tokenService.GenerateToken(request.Username);
        return Ok(tokenResponse);
    }

    private static bool IsValidUser(string username, string password)
    {
        return username == "admin" && password == "password";
    }

}