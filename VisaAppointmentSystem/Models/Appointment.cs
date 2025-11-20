using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisaAppointmentSystem.Models;

public class Appointment
{
    [Key]
    public int AppointmentID { get; set; }
    public int UserID { get; set; }
    public int SlotID { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Scheduled";

    [ForeignKey("UserID")]
    public User? User { get; set; }

    [ForeignKey("SlotID")]
    public AvailableSlot? AvailableSlot { get; set; }
}

