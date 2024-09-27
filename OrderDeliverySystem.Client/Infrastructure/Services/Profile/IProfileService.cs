using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Profile
{
    public interface IProfileService
    {
        Task<UserProfileDTO> GetCustomerProfile();
        Task<UserProfileDTO> GetCustomerProfile(int userId);
        Task<WorkerProfileDTO> GetWorkerProfile();
        Task<WorkerProfileDTO> GetWorkerProfile(int userId);
        Task<MerchantProfileDTO> GetMerchantProfile();
        Task<MerchantProfileDTO> GetMerchantProfile(int userId);
        Task<Result> UpdateCustomerProfile(UserProfileDTO model);
        Task<Result> UpdateWorkerProfile(WorkerProfileDTO model);
        Task<Result> UpdateMerchantProfile(MerchantProfileDTO model);
    }
}
