using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Категорія")]
        public string CategoryName { get; set; }

        // навігаційні властивості
        public virtual List<Product> Products { get; set; }


    }
}
