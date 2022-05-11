using System.ComponentModel.DataAnnotations;

namespace FA21.P05.Web.Features.MenuItems
{
    public class MenuItemDto
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public bool IsSpecial { get; set; }
        public bool IsEntree { get; set; }
        public bool IsSide { get; set; }
        public bool IsDrink { get; set; }
        public string Image { get; set; }
    }
}