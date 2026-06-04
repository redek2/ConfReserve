using ConfReserve.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConfReserve.Controllers
{
    [Authorize(Roles = "User")]
    public class ClientRoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientRoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _context.ConferenceRooms.ToListAsync();
            return View(rooms);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.ConferenceRooms
                .FirstOrDefaultAsync(m => m.Id == id);

            if (room == null) return NotFound();

            return View(room);
        }
    }
}