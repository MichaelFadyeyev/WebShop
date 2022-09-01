using System.Collections.Generic;
using WebShop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebShop.ViewModels
{
    public class FilterViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public SelectList Categories { get; set; }
        public SelectList Producers { get; set; }
        public PageViewModel PageViewModel { get; set; }    
    }
}
