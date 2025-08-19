namespace Appointment_Backend.Models
{
    public class Insert_Doctor_Model
    {
        public string fullName { get; set; }
        public string poli {  get; set; }
        public TimeOnly startTime { get; set; }
        public TimeOnly endTime { get; set; }
        public List<string> offDays { get; set; }
    }
}
