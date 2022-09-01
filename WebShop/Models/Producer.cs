using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebShop.Models
{
    public class Producer
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Виробник")]
        public string ProducerName { get; set; }

        // навігаційні властивості
        public virtual List<Product> Products { get; set; }

    }
}
