using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Profile
{
    public interface IProfileService
    {
        Task<CustomerProfileDTO> GetCustomerProfile();
        Task<CustomerProfileDTO> GetCustomerProfile(int userId);
        Task<WorkerProfileDTO> GetWorkerProfile();
        Task<WorkerProfileDTO> GetWorkerProfile(int userId);
        Task<MerchantProfileDTO> GetMerchantProfile();
        Task<MerchantProfileDTO> GetMerchantProfileByItemId(int itemId);
        Task<MerchantProfileDTO> GetMerchantProfile(int userId);
        Task<Result> UpdateCustomerProfile(CustomerProfileDTO model);
        Task<Result> UpdateWorkerProfile(WorkerProfileDTO model);
        Task<Result> UpdateMerchantProfile(MerchantProfileDTO model);
        Task<List<UserProfileDTO>> GetUsersList();

        Task<bool> GetWorkerAvailability();
        Task<Result> UpdateWorkerAvailability(AvailabilityUpdateDTO availability);
    }
}
