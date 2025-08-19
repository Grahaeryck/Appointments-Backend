using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;
using SOPBackend.Helper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SOPBackend.Connection_Bridges
{
    public class SQLConnections
    {
        private SqlConnection connection;
        private SqlCommand command;
        private Dencrypt dencrypt;

        public IConfiguration Configuration { get; }
        public string environment { get; set; }
        //private readonly IConfiguration Configuration;

        public SQLConnections(IConfiguration configuration)
        {
            Configuration = configuration;
            dencrypt = new Dencrypt(Configuration);
           
                string dataSource = dencrypt.Decrypt("om947N7xI94RlmEj85ZWdNMsiBsrR59JdV+3kF/qW7qd6oUTbzRBw25zd8UiDjQkgAt6txtR+vaIjJ5OcB7RZmfHKV36VZRF6+0NS655Vc0=");
                string initialCatalog = dencrypt.Decrypt("vj4LXMuzRJTnbXg9jAcm4Q==");
                string userID = dencrypt.Decrypt("wkTkRMhjh5gQy1rUtRUdrw==");
                string password = dencrypt.Decrypt("Os22O0zYnC2/tinh+VmWHJu7v6nSmJn+E6YYUKqSgpNl/uk4A7xUqoVwUZucEpYF");
                string cs = Configuration["ConnectionStrings:ConnectionStrings"];
                cs = string.Format(cs, dataSource, initialCatalog, userID, password);
                connection = new SqlConnection(cs);
         
            command = null;
        }

        public void Open()
        {
            if (connection != null && connection.State == ConnectionState.Closed)
                connection.Open();
        }
        public void Close()
        {
            if (connection != null && connection.State != ConnectionState.Closed)
                connection.Close();
        }
        public void RunIUAD(string query) //Insert, update, alter, and delete
        {
            Open();
            using (command = new SqlCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
            Close();
        }
        public int RunIUADScalar(string query)
        {
            int resInt;
            Open();
            using (command = new SqlCommand(query, connection))
            {
                resInt = (int)command.ExecuteScalar();
            }
            Close();
            return resInt;
        }

        public string RunIUADScalarString(string query)
        {
            string resString = null;
            Open();
            using (command = new SqlCommand(query, connection))
            {
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    resString = result.ToString();
                }

            }
            Close();
            return resString;
        }

        public List<string> RunSS(string query) //Select, search
        {
            Open();
            List<string> value = new List<string>();
            using (command = new SqlCommand(query, connection))
            {
                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                    value.Add(sqlDataReader[0].ToString());
            }
            Close();
            return value;
        }

        public SqlDataReader RunSSReader(string query) //Select, search but returns the datareader
        {
            Open();
            SqlDataReader sqlDataReader = null;
            using (command = new SqlCommand(query, connection))
            {
                sqlDataReader = command.ExecuteReader();
            }
            return sqlDataReader;
        }
        public DataTable GetTable(string query)
        {
            Open();
            DataTable dataTable = new DataTable();
            using (command = new SqlCommand(query, connection))
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(command);
                sqlAdapter.Fill(dataTable);
                sqlAdapter.Dispose();
            }
            Close();
            return dataTable;
        }

        public DataRow GetRow(string query)
        {
            Open();
            DataTable dataTable = new DataTable();
            using (command = new SqlCommand(query, connection))
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(command);
                sqlAdapter.Fill(dataTable);
                sqlAdapter.Dispose();
            }
            Close();

            // Return the first row if any
            return dataTable.Rows.Count > 0 ? dataTable.Rows[0] : null;
        }
        public void SetTable(DataTable table, string tableName)
        {
            Open();
            using (SqlBulkCopy bulkInsert = new SqlBulkCopy(connection))
            {
                bulkInsert.DestinationTableName = tableName;
                bulkInsert.BulkCopyTimeout = 300;
                bulkInsert.WriteToServer(table);
            }
        }
    }
}
