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

            //  Validar operación
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