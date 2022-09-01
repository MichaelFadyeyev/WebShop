using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Товар")]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Кількість")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Користувач")]
        public string UserId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
