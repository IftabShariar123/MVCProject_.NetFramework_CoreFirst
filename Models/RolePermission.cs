using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_MVC_CF_Special.Models
{
    public class RolePermission
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Permission { get; set; }
    }
}