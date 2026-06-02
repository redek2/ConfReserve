using Microsoft.AspNetCore.Identity;

namespace ConfReserve.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Relation
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
