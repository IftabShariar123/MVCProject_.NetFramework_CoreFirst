using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project_MVC_CF_Special.Models
{
    public class OrderMaster
    {
        public OrderMaster()
        {
            OrderDetails = new List<OrderDetail>(); // This prevents null reference
        }


        [Key]
        public int OrderID { get; set; }

        public DateTime OrderDate { get; set; }

        public string Description { get; set; }
        public string EmailAddress { get; set; }

        [Display(Name = "Image")]
        public string AddressProofImage { get; set; }

        public Nullable<bool> Terms { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}