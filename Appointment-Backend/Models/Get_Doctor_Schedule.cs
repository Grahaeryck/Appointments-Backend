namespace Appointment_Backend.Models
{
    public class Get_Doctor_Schedule_Input
    {
        public string drID { get; set; }
        public DateOnly appointmentDate { get; set; }
    }
    public class Get_Doctor_Schedule
    {
        public string drID { get; set; }
        public List<Get_Doctor_Schedule_Slot> timeSlot { get; set; }
    }

    public class Get_Doctor_Schedule_Slot
    {
        public TimeOnly appointmentTime { get; set; }
        public bool isAvailable { get; set; }
        public string patientName { get; set; }
        public string patientEmail { get; set; }
        public string patientPhoneNumber { get; set; }
    }
}
