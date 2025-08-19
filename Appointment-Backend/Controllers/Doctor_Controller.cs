using Appointment_Backend.Models;
using Appointment_Backend.Queries;
using Appointment_Backend.Worker;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Appointment_Backend.Controllers
{
    [Route("API/Appointment")]
    [ApiController]
    [EnableCors("ReactPolicy")]
    public class Doctor_Controller : Controller
    {
        private SQLConnections connecttions;
        private InsertQuery insertQuery;
        private GetQueries getQueries;

        private string filePath { get; set; }
        private readonly ILogger _logger;
        private readonly Serilog.ILogger _serilogger;
        private string Message { get; set; }

        public Doctor_Controller
        (
            ILogger<Doctor_Controller> logger,
            SQLConnections sQLConnections
        )
        {
            connecttions = sQLConnections;
            insertQuery = new InsertQuery();
            getQueries = new GetQueries();
            _logger = logger;
            _serilogger = Serilog.Log.ForContext<Doctor_Controller>();
            Message = "";
        }

        [HttpHead("CheckConnection")]
        public async Task<IActionResult> CheckConnection()
        {
            try
            {
                connecttions.Open();
                connecttions.Close();
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception e)
            {
                await Task.Run(() =>
                {
                    Message = "Failed check connection";
                    //_logger.LogWarning(Message);
                    //_logger.LogError(e.ToString());
                });
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }

        public string GenerateUID(string type)
        {
            int lastID = 0;
            string prefix = "";
            string idColumnName = "";
            string table = "";
            DataTable data = new DataTable();

            if (type.ToUpper().Equals("DID"))
            {
                prefix = "DR";
                idColumnName = "Doctor ID";
                table = "Appointment_Doctors";
            }
            else if(type.ToUpper().Equals("SID"))
            {
                prefix = "SID";
                idColumnName = "Schedule ID";
                table = "Appointment_Doctor_Schedule";
            }

            data = connecttions.GetTable(getQueries.QGetDistinct(idColumnName, table));
            if (data.Rows.Count > 0)
            {
                List<string> idList = data.AsEnumerable().Select(x => x.Field<string>(idColumnName)).ToList();
                List<string> numericEcoID = idList
                                                  .Select(id => id.Substring(4))
                                                  .Where(numericPart => int.TryParse(numericPart, out _))
                                                  .ToList();
                lastID = numericEcoID.Count > 0 ? int.Parse(numericEcoID.Max()) : 0;
            }
            lastID = lastID + 1;

            string requestID = prefix + lastID.ToString("D4");
            return requestID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost("InsertDoctors")]
        public async Task<ActionResult> InsertDoctors(List<Insert_Doctor_Model> input)
        {
            try
            {
                foreach(Insert_Doctor_Model doctor in input)
                { 
                    string id = GenerateUID("DID");
                    await Task.Run(() => connecttions.GetTable(insertQuery.QInsertIntoDoctor(id, doctor.poli, doctor.fullName, doctor.startTime, doctor.endTime, string.Join(", ", doctor.offDays))));
                }
                return StatusCode(StatusCodes.Status200OK);
            }

            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("GetDoctors")]
        public async Task<ActionResult<List<Get_Doctor_Model>>> GetDoctors()
        {
            try
            {
                List<Get_Doctor_Model> res = new List<Get_Doctor_Model>();
                DataTable data = await Task.Run(() => connecttions.GetTable(getQueries.QGetAll("Appointment_Doctors")));
                foreach(DataRow row in data.Rows)
                {
                    Get_Doctor_Model temp = new Get_Doctor_Model();
                    temp.drID = row["Doctor ID"].ToString();
                    temp.fullName = row["Full Name"].ToString();
                    temp.poli = row["Poli"].ToString();
                    temp.startTime = TimeOnly.Parse(row["StartTime"].ToString());
                    temp.endTime = temp.startTime = TimeOnly.Parse(row["EndTime"].ToString());
                    temp.offDays = row["Off Days"].ToString().Split(",").ToList();
                    res.Add(temp);
                }
                return res;
            }

            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}