namespace Appointment_Backend.Queries
{
    public class GetQueries
    {
        public string qGetAll = "SELECT * FROM {0}";
        public string qGetSome = "SELECT * FROM {0} WHERE UPPER([{1}]) LIKE '%{2}%'";
        public string qGetDistinct = "SELECT DISTINCT [{0}] FROM {1}";
        public string qGetSchedule = "SELECT * FROM Appointment_Doctor_Schedule WHERE [Doctor ID] = '{0}' AND CONVERT(DATE, [Appointment Date], 103) = CONVERT(DATE, '{1}', 103);";

        public string QGetAll(string table)
        {
            return string.Format(qGetAll, table);
        }
        public string QGetSome(string table, string column, string keyword)
        {
            return string.Format(qGetSome, table, column, keyword.ToUpper());
        }
        public string QGetDistinct(string targetColumn, string sourceTable)
        {
            return string.Format(qGetDistinct, targetColumn, sourceTable);
        }

        public string QGetSchedule(string drID, DateOnly date)
        {
            return string.Format(qGetSchedule, drID, date);
        }

    }
}
