using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Замовлення")]
        public int OrderId { get; set; }

        [Required]
        [Display(Name = "Товар")]
        public int ProductId { get; set; }

        // навігаційні властивості
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

    }
}

