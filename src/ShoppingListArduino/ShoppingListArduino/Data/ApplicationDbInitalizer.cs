using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ShoppingListArduino.Models;

namespace ShoppingListArduino.Data
{
    public static class ApplicationDbInitializer
    {
        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            if (userManager.FindByEmailAsync("test@mail.com").Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "test@mail.com",
                    Email = "test@mail.com"
                };

                IdentityResult result = userManager.CreateAsync(user, "123321").Result;

            }
        }
    }
}