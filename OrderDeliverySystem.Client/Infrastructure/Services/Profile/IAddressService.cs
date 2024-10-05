using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Profile
{
    public interface IAddressService
    {
        Task<List<AddressUpdateDTO>> GetAddressList();
        Task<AddressUpdateDTO> GetAddress(int addressId);
        Task<Result> UpdateAddress(AddressUpdateDTO addressDto);
        Task<Result> CreateAddress(AddressUpdateDTO addressDto);
    }
}
