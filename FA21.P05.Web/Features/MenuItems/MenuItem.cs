using System.Collections.Generic;
using FA21.P05.Web.Features.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FA21.P05.Web.Features.MenuItems
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsSpecial { get; set; }
        public bool IsEntree { get; set; }
        public bool IsSide { get; set; }
        public bool IsDrink { get; set; }
        public string Image { get; set; }
        public virtual ICollection<OrderItem> InOrders { get; set; } = new List<OrderItem>();
    }

    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder
                .Property(x => x.Name)
                .HasMaxLength(120);
        }
    }
}