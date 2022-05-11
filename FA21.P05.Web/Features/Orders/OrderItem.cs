using FA21.P05.Web.Features.MenuItems;

namespace FA21.P05.Web.Features.Orders
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int LineItemQuantity { get; set; }
        public decimal LineItemPrice { get; set; }
        public decimal LineItemTotal { get; set; }

        public int MenuItemId { get; set; }
        public virtual MenuItem MenuItem { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
    }
}