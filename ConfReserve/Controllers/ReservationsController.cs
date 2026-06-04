using ConfReserve.Data;
using ConfReserve.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConfReserve.Controllers
{
    [Authorize(Roles = "User")]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ReservationsController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> MyReservations()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var reservations = await _context.Reservations
                .Include(r => r.ConferenceRoom)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();

            return View(reservations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int conferenceRoomId, DateTime startTime, DateTime endTime)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            if (startTime >= endTime)
            {
                TempData["ErrorMessage"] = "Data zakończenia musi być późniejsza niż data rozpoczęcia.";
                return RedirectToAction("Details", "ClientRooms", new { id = conferenceRoomId });
            }

            if (startTime < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Nie można rezerwować terminów z przeszłości.";
                return RedirectToAction("Details", "ClientRooms", new { id = conferenceRoomId });
            }

            bool isOverlapping = await _context.Reservations.AnyAsync(r =>
                r.ConferenceRoomId == conferenceRoomId &&
                r.Status != "Cancelled" &&
                startTime < r.EndTime &&
                endTime > r.StartTime);

            if (isOverlapping)
            {
                TempData["ErrorMessage"] = "Wybrany termin jest już zajęty przez inną rezerwację.";
                return RedirectToAction("Details", "ClientRooms", new { id = conferenceRoomId });
            }

            var reservation = new Reservation
            {
                ConferenceRoomId = conferenceRoomId,
                UserId = userId,
                StartTime = startTime,
                EndTime = endTime,
                Status = "Pending"
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Rezerwacja została pomyślnie złożona i oczekuje na zatwierdzenie.";
            return RedirectToAction(nameof(MyReservations));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null) return NotFound();

            if (reservation.UserId != userId)
            {
                return Forbid();
            }

            if (reservation.Status == "Pending" || reservation.Status == "Approved")
            {
                reservation.Status = "Cancelled";
                _context.Update(reservation);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Rezerwacja została anulowana.";
            }
            else
            {
                TempData["ErrorMessage"] = "Nie można anulować tej rezerwacji.";
            }

            return RedirectToAction(nameof(MyReservations));
        }
    }
}