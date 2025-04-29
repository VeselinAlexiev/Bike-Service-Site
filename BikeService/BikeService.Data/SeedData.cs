using BikeService.Data;
using BikeService.Data.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        // --- ROLES ---
        var roles = new[] { "Admin", "User", "Employee" };
        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role).Result)
            {
                roleManager.CreateAsync(new IdentityRole(role)).Wait();
            }
        }

        // --- USERS ---
        if (!userManager.Users.Any())
        {
            var admin = new User { UserName = "admin@bikeservice.com", Email = "admin@bikeservice.com" };
            userManager.CreateAsync(admin, "Admin123!").Wait();
            userManager.AddToRoleAsync(admin, "Admin").Wait();

            var employee1 = new User { UserName = "employee1@bikeservice.com", Email = "employee1@bikeservice.com" };
            userManager.CreateAsync(employee1, "Employee123!").Wait();
            userManager.AddToRoleAsync(employee1, "Employee").Wait();

            var employee2 = new User { UserName = "employee2@bikeservice.com", Email = "employee2@bikeservice.com" };
            userManager.CreateAsync(employee2, "Employee123!").Wait();
            userManager.AddToRoleAsync(employee2, "Employee").Wait();

            var user1 = new User { UserName = "user1@bikeservice.com", Email = "user1@bikeservice.com" };
            userManager.CreateAsync(user1, "User123!").Wait();
            userManager.AddToRoleAsync(user1, "User").Wait();

            var user2 = new User { UserName = "user2@bikeservice.com", Email = "user2@bikeservice.com" };
            userManager.CreateAsync(user2, "User123!").Wait();
            userManager.AddToRoleAsync(user2, "User").Wait();

            context.SaveChanges();
        }

        var employees = userManager.GetUsersInRoleAsync("Employee").Result;
        var users = userManager.GetUsersInRoleAsync("User").Result;

        // --- WORKSHOPS ---
        if (!context.Workshops.Any())
        {
            var workshops = new List<Workshop>
            {
                new Workshop { Name = "Sofia Center Workshop", Location = "1000 Sofia, Vitosha Blvd 1", Latitude = 42.695, Longitude = 23.325 },
                new Workshop { Name = "Plovdiv Bike Garage", Location = "4000 Plovdiv, Kapana", Latitude = 42.15, Longitude = 24.75 }
            };

            context.Workshops.AddRange(workshops);
            context.SaveChanges();
        }

        // Link employees to workshops
        var ws1 = context.Workshops.First();
        var ws2 = context.Workshops.Skip(1).First();
        employees.First().WorkshopId = ws1.Id;
        employees.Last().WorkshopId = ws2.Id;
        context.SaveChanges();

        // --- SERVICE TYPES ---
        if (!context.ServiceTypes.Any())
        {
            context.ServiceTypes.AddRange(
                new ServiceType { Title = "Basic Inspection", Description = "Check brakes, gears, tires." },
                new ServiceType { Title = "Chain Replacement", Description = "Remove and replace chain." },
                new ServiceType { Title = "Brake Adjustment", Description = "Fix loose brakes." }
            );
            context.SaveChanges();
        }

        // --- WORKSHOP SERVICES ---
        if (!context.WorkshopServices.Any())
        {
            var services = context.ServiceTypes.ToList();
            context.WorkshopServices.AddRange(
                new WorkshopService { WorkshopId = ws1.Id, ServiceTypeId = services[0].Id, Price = 25, TimeRequired = TimeSpan.FromMinutes(30) },
                new WorkshopService { WorkshopId = ws2.Id, ServiceTypeId = services[1].Id, Price = 40, TimeRequired = TimeSpan.FromMinutes(60) },
                new WorkshopService { WorkshopId = ws1.Id, ServiceTypeId = services[2].Id, Price = 35, TimeRequired = TimeSpan.FromMinutes(45) }
            );
            context.SaveChanges();
        }

        // --- BICYCLES ---
        if (!context.Bicycles.Any())
        {
            context.Bicycles.AddRange(
                new Bicycle { Model = "Mountain King", Brand = "BikePro", Type = "Mountain", Year = 2023, Price = 899, EcoFriendly = true, BatteryCapacity = 450, EnergySource = "Electric", Material = "Aluminum", Description = "All-terrain beast." },
                new Bicycle { Model = "Speedster", Brand = "SpeedX", Type = "Road", Year = 2022, Price = 1199, EcoFriendly = false, Material = "Carbon", Description = "Speed focused racer." },
                new Bicycle { Model = "Urban Commuter", Brand = "EcoMove", Type = "City", Year = 2024, Price = 699, EcoFriendly = true, BatteryCapacity = 300, EnergySource = "Electric", Material = "Steel", Description = "Perfect for daily rides." }
            );
            context.SaveChanges();
        }

        // --- SPARE PARTS ---
        if (!context.SpareParts.Any())
        {
            context.SpareParts.AddRange(
                new SparePart { Name = "Disc Brakes", Description = "Hydraulic disc brakes.", Price = 45, Compatibility = "Mountain, Road", StockQuantity = 100, EcoFriendly = false },
                new SparePart { Name = "Chain", Description = "Universal chain.", Price = 25, Compatibility = "All", StockQuantity = 150, EcoFriendly = false },
                new SparePart { Name = "Battery Pack", Description = "Lithium battery.", Price = 120, Compatibility = "Electric", StockQuantity = 50, EcoFriendly = true }
            );
            context.SaveChanges();
        }

        // --- BICYCLE-SPAREPART LINKS ---
        var bikes = context.Bicycles.ToList();
        var parts = context.SpareParts.ToList();
        if (!context.BicycleSpareParts.Any())
        {
            context.BicycleSpareParts.AddRange(
                new BicycleSparePart { BicycleId = bikes[0].Id, SparePartId = parts[0].Id },
                new BicycleSparePart { BicycleId = bikes[0].Id, SparePartId = parts[2].Id },
                new BicycleSparePart { BicycleId = bikes[1].Id, SparePartId = parts[0].Id },
                new BicycleSparePart { BicycleId = bikes[2].Id, SparePartId = parts[2].Id }
            );
            context.SaveChanges();
        }

        // --- CARTS ---
        foreach (var user in users)
        {
            if (!context.Carts.Any(c => c.UserId == user.Id))
            {
                var cart = new Cart { UserId = user.Id };
                context.Carts.Add(cart);
                context.SaveChanges();

                context.CartItems.Add(new CartItem { CartId = cart.Id, BicycleId = bikes[0].Id, Quantity = 1, Price = bikes[0].Price });
                context.CartItems.Add(new CartItem { CartId = cart.Id, PartId = parts[1].Id, Quantity = 2, Price = parts[1].Price });
                context.SaveChanges();
            }
        }

        // --- APPOINTMENT ---
        if (!context.Appointments.Any())
        {
            var appointment = new Appointment
            {
                UserId = users.First().Id,
                WorkshopId = ws1.Id,
                AppointmentDate = DateTime.Now.AddDays(1),
                Status = "Scheduled",
                Notes = "Fix brakes and inspect chain."
            };
            context.Appointments.Add(appointment);
            context.SaveChanges();

            context.AppointmentBicycles.Add(new AppointmentBicycle { AppointmentId = appointment.Id, BicycleId = bikes[0].Id });
            context.SaveChanges();
        }

        // --- ORDER ---
        if (!context.Orders.Any())
        {
            var order = new Order
            {
                UserId = users.First().Id,
                Address = "Sofia, Tsarigradsko shose 45",
                Phone = "0888123456",
                PaymentMethod = "Cash on Delivery",
                TotalAmount = bikes[1].Price + parts[0].Price * 2
            };
            context.Orders.Add(order);
            context.SaveChanges();

            context.OrderBicycles.Add(new OrderBicycle { OrderId = order.Id, BicycleId = bikes[1].Id, Quantity = 1 });
            context.OrderSpareParts.Add(new OrderSparePart { OrderId = order.Id, PartId = parts[0].Id, Quantity = 2 });
            context.SaveChanges();
        }
    }
}