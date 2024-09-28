using OrderDeliverySystem.Share.DTOs.CartDTO;


namespace OrderDeliverySystem.Client.Infrastructure.Services.Cart
{
    public interface ICartService
    {
        Task<GetCartReponseDTO> GetCartItems();
        Task<HttpResponseMessage> UpdateCartItems(AddUpdateCartItemsRequestDTO updateDto);
        Task<HttpResponseMessage> RemoveCartItems(int itemId);
        Task<HttpResponseMessage> AddToCartItems(List<AddUpdateCartItemsRequestDTO> cartItems);
        Task<HttpResponseMessage> ClearCartItems();
    }
}
