using OrderDeliverySystem.Share.Data;
using OrderDeliverySystem.Share.DTOs;

namespace OrderDeliverySystem.Client.Infrastructure.Services.Orders
{
    public interface IOrderService
    {
        Task<List<OrderDTO>> GetOrdersByRole(string role, int id, bool recent);
        Task<List<OrderDTO>> GetOrdersTableByRole(string role, int id, bool recent);
        Task<CreateOrderDTO> GetPlacedOrder();
        Task<Result> CreateOrder(CreateOrderDTO model);
        Task<Result> UpdateOrder(OrderDTO model);
        Task<Result> CancelOrder(int orderId);
      
    }
}
