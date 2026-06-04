using ConfReserve.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConfReserve.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reservations = await _context.Reservations
                .Include(r => r.ConferenceRoom)
                .Include(r => r.User)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();

            ViewBag.TotalReservations = reservations.Count;
            ViewBag.PendingCount = reservations.Count(r => r.Status == "Pending");
            ViewBag.ApprovedCount = reservations.Count(r => r.Status == "Approved");
            ViewBag.CancelledCount = reservations.Count(r => r.Status == "Cancelled");

            ViewBag.TotalRevenue = reservations
                .Where(r => r.Status == "Approved")
                .Sum(r => Math.Ceiling((r.EndTime - r.StartTime).TotalHours) * (double)r.ConferenceRoom!.PricePerHour);

            return View(reservations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = "Approved";
            _context.Update(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = "Cancelled";
            _context.Update(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}