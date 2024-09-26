using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Profile
{
    public interface IProfileService
    {
        Task<UserProfileDTO> GetCustomer();
        Task<UserProfileDTO> GetCustomer(int userId);
        Task<Result> UpdateCustomer(UserProfileDTO model);
    }
}
