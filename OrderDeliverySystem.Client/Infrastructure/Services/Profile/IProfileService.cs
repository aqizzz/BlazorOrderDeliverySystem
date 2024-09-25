using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Profile
{
    public interface IProfileService
    {
        Task<Result<UserProfileDTO>> GetCustomer(string id);
        Task<Result> UpdateCustomer(UserProfileDTO model);
    }
}
