using CarManagetment.DTOs;
using CarManagetment.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CarManagetment.Data
{
    public class CarDBContext : DbContext
    {
        public CarDBContext(DbContextOptions <CarDBContext>options) : base(options)
        {}
        public DbSet<Users> Users { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Car> Car { get; set; }
        public DbSet<Garages> garages { get; set; }
        public DbSet<Servicess> Servicess { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<Technician> technician { get; set; }
        public DbSet<TimeSlot> TimeSlot { get; set; }
        public DbSet<Booking> Booking { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasKey(u => u.user_id);
            modelBuilder.Entity<Customer>()
                .HasKey(c => c.CustomerId);
            modelBuilder.Entity<CarDTO>()
                .HasKey(c => c.CarId);
            modelBuilder.Entity<Garages>()
                .HasKey(g => g.GarageId);

            //Service
            modelBuilder.Entity<Servicess>(entity =>
            {
                entity.HasKey(s => s.ServiceId);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });

            //Invoice
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(i => i.InvoiceId);
                entity.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(i => i.VAT).HasColumnType("decimal(5,2)");
                entity.Property(i=>i.PaymentStatus).HasMaxLength(20).HasDefaultValue("Unpaid");
            
                entity.HasOne(i => i.Booking)
                      .WithMany()
                      .HasForeignKey(i => i.Booking_id)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            //InvoiceDetails
            modelBuilder.Entity<InvoiceDetail>(entity =>
            {
                entity.HasKey(id => id.InvoiceDetailId);
                entity.Property(id => id.UnitPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(d => d.Invoice)
                      .WithMany(i => i.InvoiceDetails)
                      .HasForeignKey(id => id.InvoiceId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                
                entity.HasOne(id => id.Service)
                      .WithMany()
                      .HasForeignKey(id => id.ServiceId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Technician>()
                .HasKey(t => t.TechnicianId);
            modelBuilder.Entity<TimeSlot>()
                .HasKey(ts => ts.TimeSlot_Id);
            modelBuilder.Entity<Booking>()
                .HasKey(b => b.Booking_id);
                
        }
    }
}
