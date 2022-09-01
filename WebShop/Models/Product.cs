using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Назва")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Ціна")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        //[Required]
        [Display(Name = "Зображення")]
        public string Image { get; set; }

        [Required]
        [Display(Name = "У наявності")]
        public bool IsAvailable { get; set; }

        [Required]
        [Display(Name = "Категорія")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Виробник")]
        public int ProducerId { get; set; }

        // навігаційні властивості
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [ForeignKey("ProducerId")]
        public virtual Producer Producer { get; set; }

        public virtual List<OrderItem> Items { get; set; }

        public virtual List<CartItem> CardItems { get; set; }

    }
}
