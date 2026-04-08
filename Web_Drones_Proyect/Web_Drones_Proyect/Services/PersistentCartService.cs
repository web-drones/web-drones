using Microsoft.EntityFrameworkCore;
using Web_Drones_Proyect.Data;
using Web_Drones_Proyect.Enums;
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

        // Agregar item al carrito (VENTA / RENTA)
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

            //  Stock
            if (drone.Stock <= 0)
                return AddToCartResult.OutOfStock;

            // 🔁 Si no es venta pero solo existe renta → forzar renta
            if (!isRent && !drone.PriceSale.HasValue && drone.PriceRent.HasValue)
            {
                return await AddRentItemAsync(
                    userId,
                    dronId,
                    DateTime.Today,
                    DateTime.Today.AddDays(1),
                    quantity
                );
            }

            // Validaciones normales
            if (isRent && !drone.PriceRent.HasValue)
                return AddToCartResult.NotAvailable;

            if (!isRent && !drone.PriceSale.HasValue)
                return AddToCartResult.NotAvailable;

            var unitPrice = isRent
                ? drone.PriceRent!.Value
                : drone.PriceSale!.Value;

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
                    AddedAt = DateTime.Now
                });
            }

            cart.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return AddToCartResult.Success;
        }

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

            // ⚠️ RENTAS NUNCA SE AGRUPAN
            cart.Items.Add(new CartItem
            {
                DronID = dronId,
                Quantity = quantity,
                IsRent = true,
                RentStartDate = startDate.Date,
                RentEndDate = endDate.Date,
                RentPricePerDay = rentPricePerDay,
                UnitPrice = rentPricePerDay, // solo para compatibilidad
                Status = CartItemStatus.InCart,
                AddedAt = DateTime.Now
            });

            cart.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return AddToCartResult.Success;
        }

        //  Obtener items del carrito
        public async Task<List<CartItem>> GetItemsAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Drone)
                        .ThenInclude(d => d.Images)
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

        //  Eliminar item (lógico)
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