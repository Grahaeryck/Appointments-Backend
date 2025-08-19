namespace Appointment_Backend.Models
{
    public class Insert_Doctor_Schedule
    {
        public string drID { get; set; }
        public string patientName { get; set; }
        public string patientEmail { get; set; }
        public string patientPhoneNumber { get; set; }
        public DateOnly appointmentDate { get; set; }
        public TimeOnly appointmentTime { get; set; }
    }
}
