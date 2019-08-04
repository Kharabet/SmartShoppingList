using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShoppingListArduino.Data;
using ShoppingListArduino.Models;

namespace ShoppingListArduino.Pages.UserProducts
{
    public class IndexModel : PageModel
    {
        private readonly ShoppingListArduino.Data.ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public IndexModel(ShoppingListArduino.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<UserProduct> UserProduct { get;set; }

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["UserId"] = user.Id;
            ViewData["ProductsDb"] = JsonConvert.SerializeObject(_context.Products.ToList().Select(x => x.Title));
            UserProduct = await _context.UserProduct
                .Include(u => u.Product)
                .Where(u => u.UserId == user.Id).ToListAsync();
        }
    }
}
