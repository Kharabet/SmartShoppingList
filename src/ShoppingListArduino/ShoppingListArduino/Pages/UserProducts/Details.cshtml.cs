using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingListArduino.Data;
using ShoppingListArduino.Models;

namespace ShoppingListArduino.Pages.UserProducts
{
    public class DetailsModel : PageModel
    {
        private readonly ShoppingListArduino.Data.ApplicationDbContext _context;

        public DetailsModel(ShoppingListArduino.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
            return Page();
        }
    }
}
