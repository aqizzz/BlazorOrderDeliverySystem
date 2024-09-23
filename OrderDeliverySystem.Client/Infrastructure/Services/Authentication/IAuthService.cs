using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Authentication
{
    public interface IAuthService
    {
        Task<Result> Register(CustomerRegisterDTO model);
        Task<Result> Login(LoginRequestDTO model);
        Task<Result> ChangePassword(ChangePasswordRequestDto model);
        Task Logout();
    }
}
