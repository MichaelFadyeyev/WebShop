using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models;
using Microsoft.AspNetCore.Authorization;
using WebShop.Controllers;

namespace WebShop.Controllers.Shared
{
    public class StatInfo
    {
        public int Count { get; set; }
        public decimal Amount { get; set; }

        public StatInfo GetStatInfo(ApplicationDbContext context, string userName)
        {
            var currentUser = context.ApplicationUsers
                .Where(u => u.UserName == userName).FirstOrDefault();
            if (currentUser == null)
            {
                return new StatInfo() { Count = 0, Amount = 0.00M };
            }
            else
            {
                int count = default;
                decimal amount = 0.00M;
                var currentUserCartItems = context.CartItems
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.Product).Where(x => x.ApplicationUser.UserName == userName).ToList();
                count = currentUserCartItems.Count;
                foreach (var cartItem in currentUserCartItems)
                {
                    amount += cartItem.Product.Price * cartItem.Quantity;
                }

                return new StatInfo () { Count = count, Amount = amount }; ;
            }
        }

    }
}
