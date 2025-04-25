using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Project_MVC_CF_Special.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }




        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderMaster> OrderMasters { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            Configuration.ProxyCreationEnabled = false;
        }


        public int CreateCategory(string categoryName)
        {
            return Database.SqlQuery<int>(
                "EXEC sp_CreateCategory @CategoryName",
                new SqlParameter("@CategoryName", categoryName)
            ).FirstOrDefault();
        }

        public int UpdateCategory(int categoryId, string categoryName)
        {
            return Database.ExecuteSqlCommand(
                "EXEC sp_UpdateCategory @CategoryID, @CategoryName",
                new SqlParameter("@CategoryID", categoryId),
                new SqlParameter("@CategoryName", categoryName)
            );
        }

        public int DeleteCategory(int categoryId)
        {
            return Database.ExecuteSqlCommand(
                "EXEC sp_DeleteCategory @CategoryID",
                new SqlParameter("@CategoryID", categoryId)
            );
        }

        public List<Category> GetAllCategories()
        {
            return Database.SqlQuery<Category>("EXEC sp_GetAllCategories").ToList();
        }

        public Category GetCategoryById(int categoryId)
        {
            return Database.SqlQuery<Category>(
                "EXEC sp_GetCategoryById @CategoryID",
                new SqlParameter("@CategoryID", categoryId)
            ).FirstOrDefault();
        }
    }
}