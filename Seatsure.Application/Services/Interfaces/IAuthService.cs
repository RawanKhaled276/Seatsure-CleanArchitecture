using Seatsure.Application.DTOs.Auth;
namespace Seatsure.Application.Services.Interfaces;

public interface IAuthService
{

    Task<UserDto> RegisterAsync(RegisterRequest request);

    Task<AuthResult> LoginAsync(LoginRequest request);


}