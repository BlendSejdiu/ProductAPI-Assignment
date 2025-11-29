using Microsoft.AspNetCore.Mvc;
using ProductAPI.DataAccess.Services.IServices;
using ProductAPI.Models.DTO;
using ProductAPI.Models.Models;

namespace ProductAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    #region CTOR
    private readonly IAuthService authService;
    private readonly ILogger<AuthController> _logger;
    public AuthController(IAuthService auth, ILogger<AuthController> logger)
    {
        authService = auth;
        _logger = logger;
    }
    #endregion

    #region Endpoints

    #region Register
    /// <summary>
    /// Registers a new user with the provided details.
    /// </summary>
    /// <param name="request">The user registration information.</param>
    /// <returns>The newly created user, or a 400 Bad Request if registration fails.</returns>
    /// <response code="200">Returns the created user.</response>
    /// <response code="400">If registration fails.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("Register")]
    public async Task<ActionResult<User>> Register(UserDTO request)
    {
        try
        {
            var result = await authService.RegisterAsync(request);
            if (result is null)
                return BadRequest("Something went wrong");

            _logger.LogInformation("New user registered successfully - UserId: {UserId}, Email: {Email}", result.Id, result.Email);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return StatusCode(500, "Internal server error");
        }
    }
    #endregion

    #region Login
    /// <summary>
    /// Authenticates a user and returns a JWT token pair.
    /// </summary>
    /// <param name="request">The user login credentials.</param>
    /// <returns>A token response containing access and refresh tokens, or a 400 Bad Request if login fails.</returns>
    /// <response code="200">Returns a token response with access and refresh tokens.</response>
    /// <response code="400">If login credentials are invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("Login")]
    public async Task<ActionResult<TokenResponseDTO>> Login(UserDTO request)
    {
        try
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
                return BadRequest("Something went wrong, check your password or email.");

            _logger.LogInformation("Successful login for email: {Email}", request.Email);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return StatusCode(500, "Internal server error");
        }
    }
    #endregion

    #region Refresh Token
    /// <summary>
    /// Refreshes an access token using a valid refresh token.
    /// </summary>
    /// <param name="request">The refresh token request containing user ID and refresh token.</param>
    /// <returns>A new token response with updated access and refresh tokens, or 401 Unauthorized if the token is invalid.</returns>
    /// <response code="200">Returns a new token response.</response>
    /// <response code="401">If the refresh token is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request)
    {
        try
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid token");

            _logger.LogInformation("Token refreshed successfully for UserId: {UserId}", request.UserId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh for UserId: {UserId}", request.UserId);
            return StatusCode(500, "Internal server error");
        }
    }
    #endregion

    #endregion
}
