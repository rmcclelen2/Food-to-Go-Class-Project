using FA21.P05.Web.Features.Identity;
using FA21.P05.Web.Features.Orders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FA21.P05.Web.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var userRole = modelBuilder.Entity<UserRole>();
            userRole.HasKey(x => new {x.UserId, x.RoleId});

            userRole.HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId);

            userRole.HasOne(x => x.User)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.UserId);

            // this finds the "MenuItemConfiguration" class
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

            // you can also just manually configure these
            modelBuilder.Entity<Order>();
            modelBuilder.Entity<OrderItem>();
        }
    }
}