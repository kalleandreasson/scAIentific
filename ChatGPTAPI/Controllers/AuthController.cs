using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ChatGPTAPI.Models;
using ChatGPTAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace ChatGPTAPI.Controllers;


[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{

    private readonly string _jwtSecret;
    private readonly string _jwtValidIssuer;
    private readonly ILogger<ChatController> _logger;
    private readonly MongoDBService _mongoDBService;


    public AuthController(IConfiguration configuration, MongoDBService mongoDBService)
    {
        _jwtSecret = configuration["JWT:SecretPriv"] ?? string.Empty;
        _jwtValidIssuer = configuration["JWT:ValidIssuer"] ?? string.Empty;
        _mongoDBService = mongoDBService;
    }

    /// <summary>
    /// Allows you to authorize to use fishing report api
    /// </summary>
    /// <param name="user">user that wants to authorize</param>
    /// <returns></returns>
    /// <response code="200">Returns the the delivery site</response>
    /// <response code="400">Bad request</response>
    /// <response code="401">If enduser does not has permission</response>
    /// <response code="500">Internal server error</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("login")]

    //Check if production or development and use public/private key
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if (loginRequest.Username == null || loginRequest.Password == null)
        {
            return BadRequest("Invalid user request!!!");
        }
        if (await _mongoDBService.CheckUserCredentials(loginRequest.Username, loginRequest.Username))
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            // Create the claims for the token
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, loginRequest.Username) // Add the username as a claim
        // You can add more claims if needed
    };

            var tokeOptions = new JwtSecurityToken(
                issuer: _jwtValidIssuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return Ok(new JWTTokenResponse
            {
                Token = tokenString
            });
        }
        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        var user = await _mongoDBService.saveUser(registerRequest.Username, registerRequest.Password, registerRequest.Email);
        if (user == null)
        {
            return StatusCode(409);
        }
        return StatusCode(201);
    }
}