using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FA21.P05.Web.Data;
using FA21.P05.Web.Features.Identity;
using FA21.P05.Web.Features.MenuItems;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FA21.P05.Web.HostedServices
{
    public class SeedDataHostedService : IHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public SeedDataHostedService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            await using var dataContext = scope.ServiceProvider.GetService<DataContext>() ?? throw new Exception("Missing DataContext");
            using var userManager = scope.ServiceProvider.GetService<UserManager<User>>() ?? throw new Exception("Missing UserManager<User>");
            using var roleManager = scope.ServiceProvider.GetService<RoleManager<Role>>() ?? throw new Exception("Missing RoleManager<Role>");

            var menuItems = dataContext.Set<MenuItem>();
            if (!await menuItems.AnyAsync(cancellationToken))
            {
                menuItems.AddRange(
                    new MenuItem
                    {
                        Name = "Pizza",
                        Description = "Pick your own toppings",
                        Price = 5.99m,
                        IsSpecial = true,
                        Image = "https://i.imgur.com/M5FdHIr.jpg"

                    },
                    new MenuItem
                    {
                        Name = "Salad",
                        Description = "Fresh greens",
                        Price = 10.99m,
                        IsSide = true,
                        Image = "https://i.imgur.com/PXAINF5.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Cereal",
                        Description = "Comes with optional Malk",
                        Price = 27.36m,
                        IsSide = true,
                        Image = "https://i.imgur.com/Lp4Bli6.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Detroit-Style Pizza",
                        Description = "A deep dish pizza with pepperoni, bacon bits, sausage, and a three-cheese blend",
                        Price = 8.99m,
                        IsSpecial = true,
                        Image = "https://i.imgur.com/vqs6UWT.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Philly Cheesesteak",
                        Description = "A classic sandwhich served on a sliced roll with cheese and onions",
                        Price = 9.99m,
                        IsSpecial = true,
                        Image = "https://i.imgur.com/8F9sBXP.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Chili Cheese Coney",
                        Description = "A foot-long Coney hotdog with fresh chili and shredded cheese",
                        Price = 6.99m,
                        IsSpecial = true,
                        Image = "https://i.imgur.com/3IrOxYE.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Chicken Alfredo",
                        Description = "A classic pasta dish served with grilled chicken and creamy alfredo sauce",
                        Price = 11.49m,
                        IsSpecial = true,
                        Image = "https://i.imgur.com/w4v4X5f.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Buffalo Wings",
                        Description = "Crispy baked wings topped with our signature buttery buffalo sauce",
                        Price = 14.99m,
                        IsSpecial = true,
                        Image = "https://i.imgur.com/vUGgl0E.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Spaghetti and Meatballs",
                        Description = "A classic Italian pasta dish topped with marinara and parmesan cheese",
                        Price = 11.99m,
                        IsSpecial = true,
                        Image = "https://i.imgur.com/Ae0hRrF.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Double Bacon Cheeseburger",
                        Description = "A juicy double cheeseburger with Applewood smoked bacon, lettuce, mayo, and American cheese",
                        Price = 7.99m,
                        IsEntree = true,
                        Image = "https://i.imgur.com/mi5HnlF.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Cheeseburger",
                        Description = "A basic cheeseburger with lettuce, mayo, american cheese, tomatoes, pickles, and onions",
                        Price = 4.99m,
                        IsEntree = true,
                        Image = "https://i.imgur.com/C9VgRJj.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Lemonade",
                        Description = "Fresh lemonade",
                        Price = 1.49m,
                        IsDrink = true,
                        Image = "https://i.imgur.com/iTb4JYt.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Coca-Cola",
                        Description = "Canned",
                        Price = 1.49m,
                        IsDrink = true,
                        Image = "https://i.imgur.com/QSJTlK9.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Sprite",
                        Description = "Canned",
                        Price = 1.49m,
                        IsDrink = true,
                        Image = "https://i.imgur.com/QQ3k9BH.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Sweet Tea",
                        Description = "Freshly brewed",
                        Price = 1.49m,
                        IsDrink = true,
                        Image = "https://i.imgur.com/LIB3V3N.jpg"
                    },
                    new MenuItem
                    {
                        Name = "French Fries",
                        Description = "Hand-cut fries",
                        Price = 2.99m,
                        IsSide = true,
                        Image = "https://i.imgur.com/gePPEDW.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Mozzarella Sticks",
                        Description = "Crispy and gooey",
                        Price = 4.99m,
                        IsSide = true,
                        Image = "https://i.imgur.com/4sv9G5c.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Chicken Strips",
                        Description = "Fresh, never frozen, hand battered chicken strips",
                        Price = 6.99m,
                        IsSide = true,
                        Image = "https://i.imgur.com/MiZKTyD.jpg"
                    }
                );
            }

            var anyRoles = await roleManager.Roles.AnyAsync(cancellationToken);
            var newRolesExist = await roleManager.RoleExistsAsync(RoleNames.Server);
            if (!anyRoles || !newRolesExist)
            {
                await roleManager.CreateAsync(new Role
                {
                    Name = RoleNames.Admin
                });
                await roleManager.CreateAsync(new Role
                {
                    Name = RoleNames.Staff
                });
                await roleManager.CreateAsync(new Role
                {
                    Name = RoleNames.KitchenStaff
                });
                await roleManager.CreateAsync(new Role
                {
                    Name = RoleNames.GreetingStaff
                });
                await roleManager.CreateAsync(new Role
                {
                    Name = RoleNames.Server
                });

            }

            var anyUsers = await userManager.Users.AnyAsync(cancellationToken);
            if (!anyUsers)
            {
                const string defaultPassword = "Password123!";
                var adminUser = new User
                {
                    Name = "Ghassan",
                    Schedule = new List<SchedulerDto>()
                {
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"}
                },
                UserName = "galkadi"
                };
                await userManager.CreateAsync(adminUser, defaultPassword);
                await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);

                var normalUser = new User
                {
                    Name = "Bob",
                    Schedule = new List<SchedulerDto>()
                {
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"},
                    new SchedulerDto(){DailySchedule="OFF"}
                },
                    UserName = "bob"
                };
                await userManager.CreateAsync(normalUser, defaultPassword);
                await userManager.AddToRoleAsync(normalUser, RoleNames.Staff);
            }

            await dataContext.SaveChangesAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}