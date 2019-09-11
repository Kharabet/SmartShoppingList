using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListArduino.Models
{
    public class UserProductRfid
    {
        public int Id { get; set; }

        public int UserProductId { get; set; }
        public UserProduct UserProduct { get; set; }

        public string Rfid { get; set; }
    }
}
