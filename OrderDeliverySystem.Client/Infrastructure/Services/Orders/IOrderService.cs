using OrderDeliverySystem.Client.Pages.Merchant;
using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.DTOs;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO.OrderDeliverySystem.Share.DTOs.CartDTO;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Orders
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetOrdersByRole(string role,  bool recent);
        Task<List<OrderDTO>> GetOrdersTableByRole(string role,  bool recent);
        Task<Result> CreateOrder(CreateOrderDTO order);
        Task<Result> UpdateOrder(UpdateOrderDTO order);
        Task<GetOrderResponseDTO> GetOrderByCart();
        Task<Result> CancelOrder(int orderId);
    }
}
