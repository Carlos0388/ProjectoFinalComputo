using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisaAppointmentSystem.Data;
using VisaAppointmentSystem.Models;

namespace VisaAppointmentSystem.Controllers;

public class AvailableSlotsController : Controller
{
    private readonly ApplicationDbContext _db;

    public AvailableSlotsController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var slots = await _db.AvailableSlots.OrderBy(s => s.SlotDateTime).ToListAsync();
        return View(slots);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AvailableSlot slot)
    {
        if (ModelState.IsValid)
        {
            _db.AvailableSlots.Add(slot);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(slot);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var slot = await _db.AvailableSlots.FindAsync(id);
        if (slot == null) return NotFound();
        return View(slot);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AvailableSlot slot)
    {
        if (id != slot.SlotID) return BadRequest();
        if (ModelState.IsValid)
        {
            _db.Update(slot);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(slot);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var slot = await _db.AvailableSlots.FindAsync(id);
        if (slot == null) return NotFound();
        return View(slot);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var slot = await _db.AvailableSlots.FindAsync(id);
        if (slot != null)
        {
            _db.AvailableSlots.Remove(slot);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // Seed helper: create slots between dates at an interval
    public IActionResult Seed()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Seed(DateTime start, DateTime end, int minutes, string location)
    {
        if (end <= start || minutes <= 0)
        {
            ModelState.AddModelError(string.Empty, "Invalid seed parameters.");
            return View();
        }

        var list = new List<AvailableSlot>();
        for (var dt = start; dt <= end; dt = dt.AddMinutes(minutes))
        {
            list.Add(new AvailableSlot { SlotDateTime = dt, IsBooked = false, Location = location });
        }
        _db.AvailableSlots.AddRange(list);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
