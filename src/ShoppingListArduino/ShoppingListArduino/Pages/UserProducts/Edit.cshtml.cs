using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingListArduino.Data;
using ShoppingListArduino.Models;

namespace ShoppingListArduino.Pages.UserProducts
{
    public class EditModel : PageModel
    {
        private readonly ShoppingListArduino.Data.ApplicationDbContext _context;

        public EditModel(ShoppingListArduino.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserProduct UserProduct { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            UserProduct = await _context.UserProduct
                .Include(u => u.Product)
                .Include(u => u.User).FirstOrDefaultAsync(m => m.UserId == id);

            if (UserProduct == null)
            {
                return NotFound();
            }
           ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
           ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(UserProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProductExists(UserProduct.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./UserProducts/Index");
        }

        private bool UserProductExists(string id)
        {
            return _context.UserProduct.Any(e => e.UserId == id);
        }
    }
}
