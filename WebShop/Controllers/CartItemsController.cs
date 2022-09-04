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
using WebShop.Controllers.Shared;

namespace WebShop.Controllers
{
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
            var currentUserCartItems = _context.CartItems
                .Include(x => x.ApplicationUser)
                .Include(x => x.Product).Where(x => x.ApplicationUser.UserName == User.Identity.Name).ToList();
            var existingItem = currentUserCartItems.Where(c => c.ProductId == productId).FirstOrDefault();
            var userId = _context.Users.Where(u => u.UserName == User.Identity.Name).First().Id;
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                _context.CartItems.Add(new CartItem()
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1
                });
            }
            _context.SaveChanges();

            return new StatInfo().GetStatInfo(_context, User.Identity.Name);
        }

        //[HttpPost]
        //[AllowAnonymous]

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
            cartItem.Product = _context.Products.Where(p => p.Id == cartItem.ProductId).FirstOrDefault();
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,UserId")] CartItem cartItem, int quantity)
        {
            if (id != cartItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (quantity > 0) cartItem.Quantity = quantity;
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

        //POST: CartItems/Clear/
        [HttpPost, ActionName("Clear")]
        public async Task<IActionResult> Clear()
        {
            var userId = _context.Users.Where(u => u.UserName == User.Identity.Name).First().Id;
            var cardItems = _context.CartItems.ToList();

            foreach(var ci in cardItems)
            {
                if (ci.UserId == userId) _context.CartItems.Remove(ci);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Products");
        }
    }
}
