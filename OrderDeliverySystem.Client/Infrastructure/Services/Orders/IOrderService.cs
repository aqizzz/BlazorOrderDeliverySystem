using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Orders
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetUserRecentOrders();
        Task<List<OrderDTO>> GetUserHistoryOrders();
        Task<CreateOrderDTO> GetPlacedOrder();
        Task<Result> CreateOrder(CreateOrderDTO model);
        Task<Result> UpdateOrder(CreateOrderDTO model);
        Task<Result> CancelOrder(int orderId);
      
    }
}
