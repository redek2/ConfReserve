using System.ComponentModel.DataAnnotations;

namespace ConfReserve.Models.Entities
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonIgnore]
        public AppUser? User { get; set; }

        [Required]
        public int ConferenceRoomId { get; set; }

        public ConferenceRoom? ConferenceRoom { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Possible values: Pending, Approved, Cancelled
    }
}
