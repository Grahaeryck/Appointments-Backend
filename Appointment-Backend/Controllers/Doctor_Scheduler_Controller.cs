using Appointment_Backend.Models;
using Appointment_Backend.Queries;
using Appointment_Backend.Worker;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Appointment_Backend.Controllers
{
    [Route("API/Schedule")]
    [ApiController]
    [EnableCors("ReactPolicy")]
    public class Doctor_Scheduler_Controller : Controller
    {
        private SQLConnections connecttions;
        private InsertQuery insertQuery;
        private GetQueries getQueries;
        private Doctor_Controller drController;

        private string filePath { get; set; }
        private readonly ILogger _logger;
        private readonly Serilog.ILogger _serilogger;
        private string Message { get; set; }

        public Doctor_Scheduler_Controller
        (
            ILogger<Doctor_Scheduler_Controller> logger,
            ILogger<Doctor_Controller> drLogger,
            SQLConnections sQLConnections
        )
        {
            connecttions = sQLConnections;
            insertQuery = new InsertQuery();
            getQueries = new GetQueries();
            drController = new Doctor_Controller(drLogger, connecttions);
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

        /// <summary>
        /// POST /API/Schedule/InsertSchedule
        /// {
        ///     "drID": "DR0001",
        ///     "patientName": "Andi Pratama",
        ///     "patientEmail": "andi.pratama@example.com",
        ///     "patientPhoneNumber": "+6281234567890",
        ///     "appointmentDate": "2025-09-10",
        ///     "appointmentTime": "09:30:00"
        /// }
        /// </summary>
        /// <returns></returns>
        [HttpPost("InsertSchedule")]
        public async Task<ActionResult> InsertDoctors(Insert_Doctor_Schedule input)
        {
            try
            {
                string id = drController.GenerateUID("SID");
                await Task.Run(() => connecttions.GetTable(insertQuery.QInsertIntoSchedule(id, input.drID, input.patientName, input.patientEmail, input.patientPhoneNumber, input.appointmentDate, input.appointmentTime)));
                return StatusCode(StatusCodes.Status200OK);
            }

            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// POST /API/Schedule/GetSchedule
        /// {
        ///     "drID": "DR0001",
        ///     "appointmentDate": "2025-09-10"
        /// }
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("GetSchedule")]
        public async Task<ActionResult<Get_Doctor_Schedule>> GetSchedule(Get_Doctor_Schedule_Input input)
        {
            try
            {
                Get_Doctor_Schedule res = new Get_Doctor_Schedule();
                res.drID = input.drID;
                res.timeSlot = new List<Get_Doctor_Schedule_Slot>();

                TimeOnly startTime = new TimeOnly();
                TimeOnly endTime = new TimeOnly();

                DataTable doctorDetails = connecttions.GetTable(getQueries.QGetSome("Appointment_Doctors", "Doctor ID", input.drID));
                if (doctorDetails.Rows.Count > 0) 
                {
                    startTime = TimeOnly.Parse(doctorDetails.Rows[0]["StartTime"].ToString());
                    endTime = TimeOnly.Parse(doctorDetails.Rows[0]["EndTime"].ToString());
                }

                DataTable data = await Task.Run(() => connecttions.GetTable(getQueries.QGetSchedule(input.drID, input.appointmentDate)));
                
                List<string> timeSlots = new List<string>();
                TimeOnly current = startTime;

                while (current <= endTime)
                {
                    Get_Doctor_Schedule_Slot temp = new Get_Doctor_Schedule_Slot();
                    temp.appointmentTime = current;

                    var group = data.AsEnumerable()
                    .FirstOrDefault(row =>
                    {
                        var timeStr = row.Field<string>("Appointment Time");
                        if (TimeOnly.TryParse(timeStr, out var parsedTime))
                        {
                            return parsedTime == temp.appointmentTime;
                        }
                        return false;
                    });

                    if(group != null)
                    {
                        temp.isAvailable = false;
                        temp.patientName = group.Field<string>("Patient Name")?.Replace("'", "''");
                        temp.patientEmail = group.Field<string>("Patient Email")?.Replace("'", "''");
                        temp.patientPhoneNumber = group.Field<string>("Patient Phone Number")?.Replace("'", "''");
                    }
                    else
                    {
                        temp.isAvailable = true;
                        temp.patientName = "";
                        temp.patientEmail = "";
                        temp.patientPhoneNumber = "";
                    }

                    res.timeSlot.Add(temp);
                    current = current.AddMinutes(15);
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