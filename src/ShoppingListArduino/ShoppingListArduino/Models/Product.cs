using System.Collections.Generic;

namespace ShoppingListArduino.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }

        public List<UserProduct> UserProducts { get; set; }
    }
}