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
    public class IndexModel : PageModel
    {
        private readonly ShoppingListArduino.Data.ApplicationDbContext _context;

        public IndexModel(ShoppingListArduino.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<UserProduct> UserProduct { get;set; }

        public async Task OnGetAsync()
        {
            UserProduct = await _context.UserProduct
                .Include(u => u.Product)
                .Include(u => u.User).ToListAsync();
        }
    }
}
