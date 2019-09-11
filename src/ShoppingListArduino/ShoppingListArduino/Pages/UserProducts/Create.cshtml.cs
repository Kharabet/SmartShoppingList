using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingListArduino.Data;
using ShoppingListArduino.Models;

namespace ShoppingListArduino.Pages.UserProducts
{
    public class CreateModel : PageModel
    {
        private readonly ShoppingListArduino.Data.ApplicationDbContext _context;

        public CreateModel(ShoppingListArduino.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public UserProduct UserProduct { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userProduct = _context.UserProducts.FirstOrDefault(p => p.UserId == UserProduct.UserId && p.ProductId == UserProduct.ProductId);
            if (userProduct == null)
            {
                _context.UserProducts.Add(UserProduct);

            }
            else
            {
                userProduct.Quantity += UserProduct.Quantity;
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}