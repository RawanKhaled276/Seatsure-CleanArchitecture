using Seatsure.Domain;
namespace Seatsure.Application.Security;
public interface IJwtService
{
    (string Token,DateTime ExpiresAtUtc)GenerateToken(Guid userId,string email, UserRole role);
}