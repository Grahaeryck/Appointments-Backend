namespace Appointment_Backend.Queries
{
    public class InsertQuery
    {
        public string qInsertIntoDoctor = "INSERT INTO Appointment_Doctors ([Doctor ID], [Poli], [Full Name],[StartTime],[EndTime],[Off Days]) VALUES ('{0}','{1}','{2}','{3}','{4}', '{5}') ";

        public string QInsertIntoDoctor(string id, string poli, string fullName, TimeOnly strTime, TimeOnly endTime, string offDays)
        {
            return string.Format(qInsertIntoDoctor, id, poli, fullName, strTime, endTime, offDays);
        }

    }




}
