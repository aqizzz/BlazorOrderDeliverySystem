using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Profile
{
    public interface IAddressService
    {
        Task<List<AddressCreateDTO>> GetAddressList();
        Task<AddressCreateDTO> GetAddress(int addressId);
        Task<Result> UpdateAddress(AddressCreateDTO addressDto);
        Task<Result> CreateAddress(AddressCreateDTO addressDto);
    }
}
