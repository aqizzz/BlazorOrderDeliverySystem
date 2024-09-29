using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO.OrderDeliverySystem.Share.DTOs.CartDTO;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Orders
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetOrdersByRole(string role, int id, bool recent);
        Task<List<OrderDTO>> GetOrdersTableByRole(string role, int id, bool recent);

        Task<GetOrderResponseDTO> GetPlacedOrder(int cartId);
        Task<Result> CreateOrder(List<GetOrderItemResponseDTO> cartItems);
        Task<Result> UpdateOrder(OrderDTO model);
        Task<Result> CancelOrder(int orderId);
        Task<GetOrderResponseDTO> GetOrderByCart();
    }
   
}
