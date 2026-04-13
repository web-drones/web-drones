using Web_Drones_Proyect.Enums;
using Web_Drones_Proyect.Models;

namespace Web_Drones_Proyect.Helpers
{
    public static class DronePricingHelper
    {
        public static DroneAvailability GetAvailability(Drone drone)
        {
            if (drone.PriceSale.HasValue && drone.PriceRent.HasValue)
                return DroneAvailability.Both;

            if (drone.PriceSale.HasValue)
                return DroneAvailability.Sale;

            if (drone.PriceRent.HasValue)
                return DroneAvailability.Rent;

            return DroneAvailability.Sale;
        }

        public static string GetAvailabilityText(Drone drone)
        {
            return GetAvailability(drone) switch
            {
                DroneAvailability.Both => "💰 Venta / 🗓️ Renta",
                DroneAvailability.Sale => "💰 Solo Venta",
                DroneAvailability.Rent => "🗓️ Solo Renta",
                _ => "No disponible"
            };
        }
    }
}