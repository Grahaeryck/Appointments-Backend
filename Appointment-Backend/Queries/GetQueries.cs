namespace Appointment_Backend.Queries
{
    public class GetQueries
    {
        public string qGetAll = "SELECT * FROM {0}";
        public string qGetDistinct = "SELECT DISTINCT [{0}] FROM {1}";

        public string QGetAll(string table)
        {
            return string.Format(qGetAll, table);
        }
        public string QGetDistinct(string targetColumn, string sourceTable)
        {
            return string.Format(qGetDistinct, targetColumn, sourceTable);
        }

    }
}
