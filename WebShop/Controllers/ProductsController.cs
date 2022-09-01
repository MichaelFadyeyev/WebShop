using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebShop.Data;
using WebShop.Models;
using WebShop.ViewModels;

namespace WebShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private readonly string[] allowedExt = new string[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
        private string defaultImagePath = "/Images/default_image.jpg";

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Products
        public async Task<IActionResult> Index(int? cid, int? pid, int page = 1)
        {
            //*1
            const int pageSize = 4;

            //*2
            IQueryable<Product> products = _context.Products.Include(p => p.Category).Include(p => p.Producer);
            if (cid != null && cid != 0)
            {
                products = products.Where(p => p.CategoryId == cid);
            }
            if (pid != null && pid != 0)
            {
                products = products.Where(p => p.ProducerId == pid);
            }

            //* 3
            List<Category> categories = _context.Categories.ToList();
            List<Producer> producers = _context.Producers.ToList();

            //* 4
            categories.Insert(0, new Category() { Id = 0, CategoryName = "Всі категорії" });
            producers.Insert(0, new Producer() { Id = 0, ProducerName = "Всі виробники" });

            //* 5
            int count = await products.CountAsync();
            var items = await products.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();


            //* 6
            PageViewModel pageViewModel = new(count, page, pageSize);

            //* 7
            FilterViewModel viewModel = new()
            {
                Products = items,
                Categories = new SelectList(categories, "Id", "CategoryName", cid),
                Producers = new SelectList(producers, "Id", "ProducerName", pid),
                PageViewModel = pageViewModel
            };

            //* 8
            return View(viewModel);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Producer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName");
            ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "ProducerName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,IsAvailable,Image,CategoryId,ProducerId")] Product product, IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            //if (true)
            {
                try
                {
                    if (uploadFile != null)
                    {
                        var name = uploadFile.FileName;
                        string ext = Path.GetExtension(uploadFile.FileName);
                        if (allowedExt.Contains(ext))
                        {
                            string path = $"/Images/{name}";
                            string serverPath = _env.WebRootPath + path;
                            using (FileStream fs = new(serverPath, FileMode.Create, FileAccess.Write))
                                await uploadFile.CopyToAsync(fs);

                            product.Image = path;
                        }
                        else product.Image = defaultImagePath;
                    } 
                    else product.Image = defaultImagePath;
                }
                catch (Exception)
                {
                    throw;
                }

                //...
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "ProducerName", product.ProducerId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "ProducerName", product.ProducerId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,IsAvailable,Image,CategoryId,ProducerId")] Product product, IFormFile uploadFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadFile != null)
                    {
                        string name = uploadFile.FileName;
                        string ext = Path.GetExtension(uploadFile.FileName);
                        if (allowedExt.Contains(ext))
                        {
                            string path = $"/Images/{name}";
                            string serverPath = _env.WebRootPath + path;
                            using (FileStream fs = new FileStream(serverPath, FileMode.Create, FileAccess.Write))
                                await uploadFile.CopyToAsync(fs);
                            product.Image = path;

                        }
                    }
                    _context.Products.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "CategoryName", product.CategoryId);
            ViewData["ProducerId"] = new SelectList(_context.Producers, "Id", "ProducerName", product.ProducerId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Producer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        //*
        //->
    }
}
