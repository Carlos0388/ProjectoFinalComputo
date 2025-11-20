using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisaAppointmentSystem.Data;
using VisaAppointmentSystem.Models;

namespace VisaAppointmentSystem.Controllers;

public class AppointmentsController : Controller
{
    private readonly ApplicationDbContext _db;

    public AppointmentsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var slots = await _db.AvailableSlots.Where(s => !s.IsBooked).OrderBy(s => s.SlotDateTime).ToListAsync();
        return View(slots);
    }

    public async Task<IActionResult> Book(int id)
    {
        var slot = await _db.AvailableSlots.FindAsync(id);
        if (slot == null || slot.IsBooked) return NotFound();

        var appointment = new Appointment
        {
            SlotID = slot.SlotID,
            AvailableSlot = slot
        };

        return View(appointment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BookConfirm(int slotId, string firstName, string lastName, string email, string? phoneNumber)
    {
        var slot = await _db.AvailableSlots.FindAsync(slotId);
        if (slot == null) return NotFound();
        if (slot.IsBooked)
        {
            TempData["ErrorMessage"] = "El horario ya está reservado.";
            return RedirectToAction("Index");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["ErrorMessage"] = "Se requiere un correo electrónico para completar la reserva.";
            return RedirectToAction("Book", new { id = slotId });
        }

        // Try to find an existing user by email
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            user = new User
            {
                FirstName = firstName ?? "",
                LastName = lastName ?? "",
                Email = email,
                PhoneNumber = phoneNumber
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        var appointment = new Appointment
        {
            UserID = user.UserID,
            SlotID = slot.SlotID,
            BookingDate = DateTime.UtcNow,
            Status = "Scheduled"
        };
        _db.Appointments.Add(appointment);
        slot.IsBooked = true;
        await _db.SaveChangesAsync();

        return RedirectToAction("Confirmation", new { id = appointment.AppointmentID });
    }

    public async Task<IActionResult> Confirmation(int id)
    {
        var appointment = await _db.Appointments.Include(a => a.AvailableSlot).Include(a => a.User).FirstOrDefaultAsync(a => a.AppointmentID == id);
        if (appointment == null) return NotFound();
        return View(appointment);
    }

    // GET: Appointments/MyAppointments?email=someone@domain
    public async Task<IActionResult> MyAppointments(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return View((IEnumerable<Appointment>?)null);
        }

        var appointments = await _db.Appointments
            .Include(a => a.AvailableSlot)
            .Include(a => a.User)
            .Where(a => a.User != null && a.User.Email == email)
            .OrderByDescending(a => a.BookingDate)
            .ToListAsync();

        return View(appointments);
    }

    // GET: Appointments/Cancel/5
    public async Task<IActionResult> Cancel(int id)
    {
        var appointment = await _db.Appointments.Include(a => a.AvailableSlot).Include(a => a.User).FirstOrDefaultAsync(a => a.AppointmentID == id);
        if (appointment == null) return NotFound();

        if (appointment.Status == "Cancelled")
        {
            TempData["Message"] = "La cita ya está cancelada.";
            return RedirectToAction("MyAppointments", new { email = appointment.User?.Email });
        }

        return View(appointment);
    }

    // POST: Appointments/Cancel/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Cancel")]
    public async Task<IActionResult> CancelConfirmed(int id)
    {
        var appointment = await _db.Appointments.Include(a => a.AvailableSlot).Include(a => a.User).FirstOrDefaultAsync(a => a.AppointmentID == id);
        if (appointment == null) return NotFound();

        if (appointment.Status != "Cancelled")
        {
            appointment.Status = "Cancelled";
            if (appointment.AvailableSlot != null)
            {
                appointment.AvailableSlot.IsBooked = false;
            }
            await _db.SaveChangesAsync();
        }

        TempData["Message"] = "Cita cancelada correctamente.";
        return RedirectToAction("MyAppointments", new { email = appointment.User?.Email });
    }
}
