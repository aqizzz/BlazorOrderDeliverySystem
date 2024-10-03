using OrderDeliverySystem.Share.Data.Models;
using OrderDeliverySystem.Share.DTOs.CartDTO;
using OrderDeliverySystem.Share.DTOs.PlacedOrderDTO;
using System;
using static OrderDeliverySystem.Share.Data.Constants;

namespace OrderDeliverySystem.Share.DTOs
{
    public class CreateOrderDTO
    {
        public decimal Tax { get; set; }
        public decimal DeliveryFee { get; set; }
        public List<OrderByMerchanDTO>? Orders { get; set; }

    }
}
