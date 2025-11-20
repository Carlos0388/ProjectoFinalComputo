using System;
using System.ComponentModel.DataAnnotations;

namespace VisaAppointmentSystem.Models;

public class AvailableSlot
{
    [Key]
    public int SlotID { get; set; }
    public DateTime SlotDateTime { get; set; }
    public bool IsBooked { get; set; }
    public string Location { get; set; } = string.Empty;
}
