using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace WebShop.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        // навігаційні властивості
        public virtual List<Order> Orders { get; set; }
        public virtual List<CartItem> CardItems { get; set; }


    }
}
