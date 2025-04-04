using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_MVC_CF_Special.Models.ViewModels
{
    public class OrderVM
    {
        public int OrderDetailID { get; set; }
        public int ProductID { get; set; }
        public int OrderID { get; set; }

        public int Quantity { get; set; }

        public decimal Rate { get; set; }


        public DateTime OrderDate { get; set; }

        public string Description { get; set; }

        public string AddressProofImage { get; set; }

        public Nullable<bool> Terms { get; set; }
    }
}