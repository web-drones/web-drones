using Microsoft.EntityFrameworkCore;
using Web_Drones_Proyect.Data;
using Web_Drones_Proyect.Enums;
using Web_Drones_Proyect.Helpers;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Services
{
    public class CartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        //  Obtener o crear carrito activo
        public async Task<Cart> GetOrCreateActiveCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c =>
                    c.UserID == userId &&
                    c.Status == CartStatus.Active
                );

            if (cart != null)
                return cart;

            cart = new Cart
            {
                UserID = userId,
                Status = CartStatus.Active,
                CreatedAt = DateTime.Now
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return cart;
        }

        // =========================
        // AGREGAR ITEM (VENTA / RENTA)
        // =========================
        public async Task<AddToCartResult> AddItemAsync(
            int userId,
            int dronId,
            bool isRent,
            int quantity = 1)
        {
            var cart = await GetOrCreateActiveCartAsync(userId);

            var drone = await _context.Drones.FindAsync(dronId);
            if (drone == null)
                throw new Exception("Drone no encontrado");

            var availability = DronePricingHelper.GetAvailability(drone);

            //  Stock
            if (drone.Stock <= 0)
                return AddToCartResult.OutOfStock;

            // 🔥 reglas basadas en disponibilidad (SIN PRECIOS)
            if (availability == DroneAvailability.Sale && isRent)
                return AddToCartResult.NotAvailable;

            if (availability == DroneAvailability.Rent && !isRent)
                isRent = true;

            decimal unitPrice;

            if (isRent && drone.PriceRent.HasValue)
                unitPrice = drone.PriceRent.Value;
            else if (!isRent && drone.PriceSale.HasValue)
                unitPrice = drone.PriceSale.Value;
            else
                return AddToCartResult.NotAvailable;

            var item = cart.Items.FirstOrDefault(i =>
                i.DronID == dronId &&
                i.Status == CartItemStatus.InCart &&
                i.IsRent == isRent
            );

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    DronID = dronId,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    IsRent = isRent,
                    Status = CartItemStatus.InCart,
                    AddedAt = DateTime.Now,

                    // 🔥 AQUÍ VA LA MEJORA CLAVE
                    AvailabilityMode = availability
                });
            }

            cart.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return AddToCartResult.Success;
        }

        // =========================
        // AGREGAR RENTA
        // =========================
        public async Task<AddToCartResult> AddRentItemAsync(
            int userId,
            int dronId,
            DateTime startDate,
            DateTime endDate,
            int quantity = 1)
        {
            if (endDate.Date < startDate.Date)
                throw new Exception("La fecha de fin no puede ser menor que la de inicio");

            var cart = await GetOrCreateActiveCartAsync(userId);

            var drone = await _context.Drones.FindAsync(dronId);
            if (drone == null)
                throw new Exception("Drone no encontrado");

            if (!drone.PriceRent.HasValue)
                return AddToCartResult.NotAvailable;

            if (drone.Stock < quantity)
                return AddToCartResult.OutOfStock;

            var rentDays = (endDate.Date - startDate.Date).Days + 1;
            if (rentDays <= 0)
                throw new Exception("Días de renta inválidos");

            var rentPricePerDay = drone.PriceRent.Value;

            cart.Items.Add(new CartItem
            {
                DronID = dronId,
                Quantity = quantity,
                IsRent = true,
                RentStartDate = startDate.Date,
                RentEndDate = endDate.Date,
                RentPricePerDay = rentPricePerDay,
                UnitPrice = rentPricePerDay,
                Status = CartItemStatus.InCart,
                AddedAt = DateTime.Now,

                // 🔥 IMPORTANTE TAMBIÉN AQUÍ
                AvailabilityMode = DroneAvailability.Rent
            });

            cart.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return AddToCartResult.Success;
        }

        // =========================
        // OBTENER ITEMS
        // =========================
        public async Task<List<CartItem>> GetItemsAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Drone)
                        .ThenInclude(d => d.Images)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Drone)
                        .ThenInclude(d => d.Category)
                .FirstOrDefaultAsync(c =>
                    c.UserID == userId &&
                    c.Status == CartStatus.Active
                );

            if (cart == null)
                return new List<CartItem>();

            return cart.Items
                .Where(i => i.Status == CartItemStatus.InCart)
                .ToList();
        }

        // =========================
        // ELIMINAR ITEM
        // =========================
        public async Task RemoveItemAsync(int cartItemId)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item == null) return;

            item.Status = CartItemStatus.Removed;
            item.RemovedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}