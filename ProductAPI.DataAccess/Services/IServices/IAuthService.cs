using ProductAPI.Models.DTO;
using ProductAPI.Models.Models;

namespace ProductAPI.DataAccess.Services.IServices
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDTO request);
        Task<TokenResponseDTO?> LoginAsync(UserDTO request);
        Task<TokenResponseDTO?> RefreshTokensAsync(RefreshTokenRequestDTO request);
    }
}
