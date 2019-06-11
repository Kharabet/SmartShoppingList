using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShoppingListArduino.Data;
using ShoppingListArduino.Models;

namespace ShoppingListArduino.Pages.Products
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
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();

            return RedirectToPage("./UserProducts/Index");
        }
    }
}