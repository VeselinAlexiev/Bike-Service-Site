using BikeService.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Bicycle> Bicycles { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderSparePart> OrderSpareParts { get; set; }
    public DbSet<SparePart> SpareParts { get; set; }
    public DbSet<AppointmentBicycle> AppointmentBicycles { get; set; }
    public DbSet<BicycleSparePart> BicycleSpareParts { get; set; }
    public DbSet<OrderBicycle> OrderBicycles { get; set; }
    public DbSet<Workshop> Workshops { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }
    public DbSet<WorkshopService> WorkshopServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.SparePart)
            .WithMany()
            .HasForeignKey(ci => ci.PartId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.User)
            .WithMany(u => u.Appointments)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderSparePart>()
            .HasKey(osp => new { osp.OrderId, osp.PartId });

        modelBuilder.Entity<OrderSparePart>()
            .HasOne(osp => osp.Order)
            .WithMany(o => o.OrderSpareParts)
            .HasForeignKey(osp => osp.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderSparePart>()
            .HasOne(osp => osp.SparePart)
            .WithMany(sp => sp.OrderSpareParts)
            .HasForeignKey(osp => osp.PartId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AppointmentBicycle>()
            .HasKey(ab => new { ab.AppointmentId, ab.BicycleId });

        modelBuilder.Entity<AppointmentBicycle>()
            .HasOne(ab => ab.Appointment)
            .WithMany(a => a.AppointmentBicycles)
            .HasForeignKey(ab => ab.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AppointmentBicycle>()
            .HasOne(ab => ab.Bicycle)
            .WithMany(b => b.AppointmentBicycles)
            .HasForeignKey(ab => ab.BicycleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BicycleSparePart>()
            .HasOne(bsp => bsp.Bicycle)
            .WithMany(b => b.BicycleSpareParts)
            .HasForeignKey(bsp => bsp.BicycleId);

        modelBuilder.Entity<BicycleSparePart>()
            .HasOne(bsp => bsp.SparePart)
            .WithMany(sp => sp.BicycleSpareParts)
            .HasForeignKey(bsp => bsp.SparePartId);

        modelBuilder.Entity<Bicycle>()
            .Property(b => b.EcoFriendly)
            .HasDefaultValue(false);

        modelBuilder.Entity<Appointment>()
            .Property(a => a.Status)
            .HasDefaultValue("Scheduled");

        modelBuilder.Entity<Order>()
            .Property(o => o.OrderDate)
            .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasDefaultValue("Pending");

        modelBuilder.Entity<Bicycle>()
            .Property(b => b.EnergySource)
            .HasDefaultValue("Unknown");

        modelBuilder.Entity<WorkshopService>()
            .HasKey(ws => new { ws.WorkshopId, ws.ServiceTypeId });

        modelBuilder.Entity<WorkshopService>()
            .HasOne(ws => ws.Workshop)
            .WithMany(w => w.WorkshopServices)
            .HasForeignKey(ws => ws.WorkshopId);

        modelBuilder.Entity<WorkshopService>()
            .HasOne(ws => ws.ServiceType)
            .WithMany(st => st.WorkshopServices)
            .HasForeignKey(ws => ws.ServiceTypeId);

        modelBuilder.Entity<OrderBicycle>()
            .HasKey(ob => new { ob.OrderId, ob.BicycleId });

        modelBuilder.Entity<OrderBicycle>()
            .HasOne(ob => ob.Order)
            .WithMany(o => o.OrderBicycles)
            .HasForeignKey(ob => ob.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderBicycle>()
            .HasOne(ob => ob.Bicycle)
            .WithMany()
            .HasForeignKey(ob => ob.BicycleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}