using Microsoft.EntityFrameworkCore;
using VisaAppointmentSystem.Models;

namespace VisaAppointmentSystem.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<AvailableSlot> AvailableSlots { get; set; } = null!;
    public DbSet<Appointment> Appointments { get; set; } = null!;
}
