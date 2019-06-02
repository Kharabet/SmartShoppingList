using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ShoppingListArduino.Models
{
    public class ApplicationUser : IdentityUser
    {
        public List<UserProduct> UserProducts { get; set; }
    }
}