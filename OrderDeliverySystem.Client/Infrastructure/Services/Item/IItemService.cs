/*using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Item
{
    public interface IItemService
    {  
            Task<GetCartReponseDTO> GetCartItems();
            Task<HttpResponseMessage> UpdateCartItems(AddUpdateCartItemsRequestDTO updateDto);
            Task<HttpResponseMessage> RemoveCartItems(int itemId);
            Task<HttpResponseMessage> AddToCartItems(List<AddUpdateCartItemsRequestDTO> cartItems);
            Task<HttpResponseMessage> ClearCartItems();
            Task<HttpResponseMessage> GetTotalCartQuantity();

            event Action OnCartChanged;
        
    }



}
*/