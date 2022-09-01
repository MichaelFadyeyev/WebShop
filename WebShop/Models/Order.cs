using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "№ замовлення")]
        public string OrderNo { get; set; }
        [Required]
        [Display(Name = "Ім'я замовника")]
        public string PersonName { get; set; }

        [Required]
        [Display(Name = "Телефон замовника")]
        public string PhoneNo { get; set; }
        [Required]
        [Display(Name = "E-Mail замовника")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Адреса замовника")]
        public string Address { get; set; }

        [Required]
        //[DataType(DataType.Date)] // не використовую розділення на дату і час
        [Display(Name = "Дата замовлення")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Display(Name = "Замовник")]
        public string UserId { get; set; }

        // навігаційні властивості
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public virtual List<OrderItem> Items { get; set; }
    }
}
