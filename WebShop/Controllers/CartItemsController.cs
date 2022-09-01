using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebShop.Controllers
{
    public class StatInfo
    {
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }


    [Authorize]
    public class CartItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //*
        [HttpPost]
        public StatInfo AddProductToCart(int productId)
        {

            var currentUser = _context.ApplicationUsers
                .Single(u => u.UserName == User.Identity.Name);
            _context.CartItems.Add(new CartItem()
            {
                UserId = currentUser.Id,
                ProductId = productId
            });
            _context.SaveChanges();

            return GetStatInfo();
        }

        [HttpPost]
        [AllowAnonymous]
        public StatInfo GetStatInfo()
        {
            var currentUser = _context.ApplicationUsers
                .Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            if (currentUser == null)
            {
                return new StatInfo() { Count = 0, Amount = 0.00M };
            }
            else
            {
                int count = default;
                decimal amount = 0.00M;
                var currentUserCartItems = _context.CartItems
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.Product).Where(x => x.ApplicationUser.UserName == User.Identity.Name).ToList();
                count = currentUserCartItems.Count;
                foreach (var cartItem in currentUserCartItems)
                {
                    amount += cartItem.Product.Price;
                }
                StatInfo si = new() { Count = count, Amount = amount };
                return si;
            }
        }
        //->

        // GET: CartItems
        public async Task<IActionResult> Index()
        {
            var currentUserCartItems = _context.CartItems
                .Include(x => x.ApplicationUser)
                .Include(x => x.Product).Where(x => x.ApplicationUser.UserName == User.Identity.Name);
            return View(await currentUserCartItems.ToListAsync());
        }

        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.ApplicationUser)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Image");
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,UserId")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", cartItem.UserId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Image", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems.FindAsync(id);
            cartItem.Product = _context.Products.Single(p => p.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", cartItem.UserId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Image", cartItem.ProductId);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,UserId")] CartItem cartItem)
        {
            if (id != cartItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", cartItem.UserId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Image", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: CartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.ApplicationUser)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.Id == id);
        }
    }
}
