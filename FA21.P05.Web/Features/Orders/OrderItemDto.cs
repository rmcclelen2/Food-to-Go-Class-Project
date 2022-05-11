namespace FA21.P05.Web.Features.Orders
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public decimal LineItemPrice { get; set; }
        public int LineItemQuantity { get; set; }
        public decimal LineItemTotal { get; set; }
    }
}