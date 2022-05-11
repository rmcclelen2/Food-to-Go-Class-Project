using System;
using System.Collections.Generic;

namespace FA21.P05.Web.Features.Orders
{
    public class Order
    {
        public int Id { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTimeOffset? Placed { get; set; }
        public DateTimeOffset? Started { get; set; }
        public DateTimeOffset? Finished { get; set; }
        public DateTimeOffset? CustomerRecieved { get; set; }
        public DateTimeOffset? Canceled { get; set; }
        public string Address { get; set; }
        public string CustomerFirst { get; set; }
        public string CustomerLast { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}