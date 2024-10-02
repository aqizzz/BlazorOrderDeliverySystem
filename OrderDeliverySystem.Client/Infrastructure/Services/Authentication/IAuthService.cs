using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Authentication
{
    public interface IAuthService
    {
        Task<Result> Register(CustomerRegisterDTO model);
        Task<Result> WorkerRegister(WorkerRegisterDTO model);
        Task<Result> MerchantRegister(MerchantRegisterDTO model);
        Task<Result> Login(LoginRequestDTO model);
        Task<Result> ChangePassword(ChangePasswordRequestDto model);
        Task Logout();

        Task<Result> DeleteUser(int userId);
    }
}
