using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICPOSレジ_2C29KS.Models
{
   public class TransactionDetails
    {
        public int DetailID { get; set; }
        public int ReceiptNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Subtotal { get; set; }
    }
}
