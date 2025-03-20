using BikeService.Data;
using BikeService.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        // Seed the roles
        if (!roleManager.Roles.Any())
        {
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                var roleResult = roleManager.CreateAsync(new IdentityRole(role)).Result;
            }
        }

        // Seed the users
        if (!userManager.Users.Any())
        {
            var adminUser = new User
            {
                UserName = "admin@bikeservice.com",
                Email = "admin@bikeservice.com"
            };

            var userResult = userManager.CreateAsync(adminUser, "Admin123!").Result;
            if (userResult.Succeeded)
            {
                userManager.AddToRoleAsync(adminUser, "Admin").Wait();
            }

            var regularUser = new User
            {
                UserName = "user@bikeservice.com",
                Email = "user@bikeservice.com"
            };

            userResult = userManager.CreateAsync(regularUser, "User123!").Result;
            if (userResult.Succeeded)
            {
                userManager.AddToRoleAsync(regularUser, "User").Wait();
            }
        }

        // Seed Bicycles
        if (!context.Bicycles.Any())
        {
            var bicycles = new[]
            {
                new Bicycle
                {
                    Model = "Mountain Bike X",
                    Brand = "BikePro",
                    Year = 2022,
                    Type = "Mountain",
                    Price = 1200.00M,
                    Description = "A sturdy mountain bike.",
                    ImageUrl = "https://example.com/mountain-bike.jpg",
                    EcoFriendly = true,
                    BatteryCapacity = 500.00M,
                    EnergySource = "Electric",
                    Material = "Aluminum"
                },
                new Bicycle
                {
                    Model = "Road Bike Z",
                    Brand = "SpeedMaster",
                    Year = 2023,
                    Type = "Road",
                    Price = 1500.00M,
                    Description = "A high-performance road bike.",
                    ImageUrl = "https://example.com/road-bike.jpg",
                    EcoFriendly = false,
                    BatteryCapacity = null,
                    EnergySource = null,
                    Material = "Carbon Fiber"
                }
            };

            context.Bicycles.AddRange(bicycles);
            context.SaveChanges();
        }

        // Seed Services
        if (!context.Services.Any())
        {
            var services = new[]
            {
                new Service
                {
                    Name = "Bike Repair",
                    Description = "Fixes and repairs bikes.",
                    Price = 50.00M,
                    TimeRequired = TimeSpan.FromHours(2),
                    Location = "Service Center 1"
                },
                new Service
                {
                    Name = "Bike Customization",
                    Description = "Customize your bike to your liking.",
                    Price = 100.00M,
                    TimeRequired = TimeSpan.FromHours(3),
                    Location = "Service Center 2"
                }
            };

            context.Services.AddRange(services);
            context.SaveChanges();
        }

        // Seed Spare Parts
        if (!context.SpareParts.Any())
        {
            var spareParts = new[]
            {
                new SparePart
                {
                    Name = "Brake Pad",
                    Description = "Essential for braking.",
                    Price = 25.00M,
                    Compatibility = "Universal",
                    StockQuantity = 50,
                    ImageUrl = "https://example.com/brake-pad.jpg",
                    EcoFriendly = true
                },
                new SparePart
                {
                    Name = "Bike Chain",
                    Description = "High-quality bike chain.",
                    Price = 40.00M,
                    Compatibility = "Road Bike, Mountain Bike",
                    StockQuantity = 100,
                    ImageUrl = "https://example.com/bike-chain.jpg",
                    EcoFriendly = false
                }
            };

            context.SpareParts.AddRange(spareParts);
            context.SaveChanges();
        }

        // Seed Appointments
        if (!context.Appointments.Any())
        {
            var appointments = new[]
            {
        new Appointment
        {
            UserId = userManager.Users.OrderBy(u => u.Id).First().Id, // Assign the admin user
            ServiceId = context.Services.OrderBy(s => s.Id).First().Id, // First service
            AppointmentDate = DateTime.Now.AddDays(1),
            Status = "Scheduled"
        },
        new Appointment
        {
            UserId = userManager.Users.OrderBy(u => u.Id).Last().Id, // Assign the regular user
            ServiceId = context.Services.OrderBy(s => s.Id).Last().Id, // Last service
            AppointmentDate = DateTime.Now.AddDays(2),
            Status = "Scheduled"
        }
    };

            context.Appointments.AddRange(appointments);
            context.SaveChanges();
        }

        // Seed Orders
        if (!context.Orders.Any())
        {
            var orders = new[]
            {
        new Order
        {
            UserId = userManager.Users.OrderBy(u => u.Id).First().Id, // Admin user
            OrderDate = DateTime.Now,
            TotalAmount = 200.00M,
            Status = "Completed"
        },
        new Order
        {
            UserId = userManager.Users.OrderBy(u => u.Id).Last().Id, // Regular user
            OrderDate = DateTime.Now,
            TotalAmount = 100.00M,
            Status = "Pending"
        }
    };

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }

        // Seed Cart
        if (!context.Carts.Any())
        {
            var carts = new[]
            {
        new Cart
        {
            UserId = userManager.Users.First().Id // Admin user
        },
        new Cart
        {
            UserId = userManager.Users.OrderBy(u => u.Id).Last().Id // Regular user, ensure deterministic order
        }
    };

            context.Carts.AddRange(carts);
            context.SaveChanges();
        }
    }
}