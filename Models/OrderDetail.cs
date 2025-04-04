using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project_MVC_CF_Special.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }
        public int ProductID { get; set; }
        public int OrderID { get; set; }
        public OrderMaster OrderMaster { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public Product product { get; set; }
    }
}