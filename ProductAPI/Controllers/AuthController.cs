using Microsoft.AspNetCore.Mvc;
using ProductAPI.DataAccess.Services.IServices;
using ProductAPI.Models.DTO;
using ProductAPI.Models.Models;

namespace ProductAPI.Controllers
{
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
}
