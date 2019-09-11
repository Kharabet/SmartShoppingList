using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingListArduino.Data;
using ShoppingListArduino.Models;

namespace ShoppingListArduino.Pages.UserProducts
{
    public class DeleteModel : PageModel
    {
        private readonly ShoppingListArduino.Data.ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteModel(ShoppingListArduino.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public UserProduct UserProduct { get; set; }

        public async Task<IActionResult> OnGetAsync(int productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            UserProduct = _context.UserProducts
                .Include(u => u.Product)
                .FirstOrDefault(u => u.UserId == user.Id && u.ProductId == productId);


            if (UserProduct == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);


            UserProduct = await _context.UserProducts.FirstOrDefaultAsync(x => x.UserId == user.Id && x.ProductId == productId);

            if (UserProduct != null)
            {
                _context.UserProducts.Remove(UserProduct);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
