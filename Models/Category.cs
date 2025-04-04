using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Project_MVC_CF_Special.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}