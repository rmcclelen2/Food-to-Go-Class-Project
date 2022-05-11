using System.Collections.Generic;

namespace FA21.P05.Web.Features.Orders
{
    public class CreateOrderDto
    {
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
        public string Address { get; set; }
        public string CustomerFirst { get; set; }
        public string CustomerLast { get; set; }
    }
}