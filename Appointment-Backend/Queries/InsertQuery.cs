namespace Appointment_Backend.Queries
{
    public class InsertQuery
    {

        public string QInsertIntoDoctor(string id, string poli, string fullName, TimeOnly strTime, TimeOnly endTime, string offDays)
        {
            string qInsertIntoDoctor = "INSERT INTO Appointment_Doctors ([Doctor ID], [Poli], [Full Name],[StartTime],[EndTime],[Off Days]) VALUES ('{0}','{1}','{2}','{3}','{4}', '{5}') ";
            return string.Format(qInsertIntoDoctor, id, poli, fullName, strTime, endTime, offDays);
        }

        public string QInsertIntoSchedule(string sID, string drID, string pName, string pEmail, string pPhNumber, DateOnly date, TimeOnly time)
        {
            string qInsertIntoSchedule = "INSERT INTO Appointment_Doctor_Schedule ([Schedule ID],[Doctor ID],[Patient Name],[Patient Email],[Patient Phone Number],[Appointment Date],[Appointment Time]) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}');";
            return string.Format(qInsertIntoSchedule, sID, drID, pName, pEmail, pPhNumber, date, time);
        }

    }




}
