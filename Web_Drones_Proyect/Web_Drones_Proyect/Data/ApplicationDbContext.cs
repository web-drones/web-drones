using Microsoft.EntityFrameworkCore;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =====================
        // DbSets existentes
        // =====================

        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categorias { get; set; }
        public DbSet<Drone> Drones { get; set; }
        public DbSet<User> Usuarios { get; set; }
        public DbSet<Sale> Ventas { get; set; }
        public DbSet<DetailsSale> VentaDetalle { get; set; }
        public DbSet<DronRent> Rentas { get; set; }
        public DbSet<Service> Servicios { get; set; }
        public DbSet<TypeDrones> TiposDron { get; set; }
        public DbSet<ImagesDrones> DronImagenes { get; set; }
        public DbSet<ServiceRequest> SolicitudesServicio { get; set; }

        // =====================
        // NUEVO: Carrito persistente
        // =====================

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================
            // Tablas
            // =====================

            modelBuilder.Entity<TypeDrones>().ToTable("TiposDron");
            modelBuilder.Entity<Category>().ToTable("Categorias");
            modelBuilder.Entity<User>().ToTable("Usuarios");
            modelBuilder.Entity<DetailsSale>().ToTable("VentaDetalle");
            modelBuilder.Entity<DronRent>().ToTable("Rentas");
            modelBuilder.Entity<ImagesDrones>().ToTable("DronImagenes");
            modelBuilder.Entity<Service>().ToTable("Servicios");
            modelBuilder.Entity<ServiceRequest>().ToTable("SolicitudesServicio");

            modelBuilder.Entity<Cart>().ToTable("Cart");
            modelBuilder.Entity<CartItem>().ToTable("CartItem");

            // =====================
            // Relaciones Drone
            // =====================

            modelBuilder.Entity<Drone>()
                .HasOne(d => d.TypeDrone)
                .WithMany()
                .HasForeignKey(d => d.TypeDroneID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Drone>()
                .HasOne(d => d.Category)
                .WithMany()
                .HasForeignKey(d => d.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================
            // Relaciones DetailsSale
            // =====================

            modelBuilder.Entity<DetailsSale>()
                .HasOne(ds => ds.Sale)
                .WithMany(s => s.DetailsSales)
                .HasForeignKey(ds => ds.SaleID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetailsSale>()
                .HasOne(ds => ds.Drone)
                .WithMany(d => d.SaleDetails)
                .HasForeignKey(ds => ds.DronID)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================
            // Relaciones DronRent
            // =====================

            modelBuilder.Entity<DronRent>()
                .HasOne(dr => dr.User)
                .WithMany()
                .HasForeignKey(dr => dr.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DronRent>()
                .HasOne(dr => dr.Drone)
                .WithMany(d => d.Rents)
                .HasForeignKey(dr => dr.DronID)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================
            // Relaciones ImagesDrones
            // =====================

            modelBuilder.Entity<ImagesDrones>()
                .HasOne(img => img.Drone)
                .WithMany(d => d.Images)
                .HasForeignKey(img => img.DronID)
                .OnDelete(DeleteBehavior.Cascade);

            // =====================
            // Relaciones ServiceRequest
            // =====================

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.User)
                .WithMany()
                .HasForeignKey(sr => sr.UserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Service)
                .WithMany()
                .HasForeignKey(sr => sr.ServiceID)
                .OnDelete(DeleteBehavior.Restrict);

            // =====================
            // Enums (opcional pero recomendado)
            // =====================

            modelBuilder.Entity<Cart>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.Status)
                .HasConversion<string>();

            modelBuilder.Entity<CartItem>()
                .Property(ci => ci.AvailabilityMode)
                .HasConversion<int>();

            modelBuilder.Entity<Drone>()
                .Property(d => d.Availability)
                .HasConversion<int>();
        }
    }
}