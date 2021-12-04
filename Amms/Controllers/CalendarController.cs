using Amms.Models;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Events.Calendar;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Amms.Controllers
{
    public class CalendarController : Controller
    {
        public ActionResult Calendar()
        {
            return View();
        }
        public ActionResult Backend()
        {
            return new Dpm().CallBack(this);
        }
        public ActionResult Week()
        {
            return new Dpc().CallBack(this);
        }
        class Dpm : DayPilotMonth
        {
            //protected override void OnInit(InitArgs e)
            //{
            //    Events = new EventManager().FilteredData(VisibleStart, VisibleEnd).AsEnumerable();
            //    Update();
            //}
        }
    }
    class Dpc : DayPilotCalendar
    {
        protected override void OnInit(InitArgs e)
        {
            
            Events = new EventManager().FilteredData().AsEnumerable();
            DataStartField = "start";
            DataEndField = "end";
            DataTextField = "name";
            DataTextField = "id";
            Update(CallBackUpdateType.Full);
        }

        protected override void OnTimeRangeSelected(TimeRangeSelectedArgs e)
        {
           
        }

    }
        public class EventManager
    {
        public DataTable FilteredData()
        {
            DataTable dt = new DataTable();
            dt.Clear();
            dt.Columns.Add("start", typeof(DateTime));
            dt.Columns.Add("end", typeof(DateTime));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("id", typeof(string));

            List<Appointment> ListApp = new List<Appointment>();
                using (MyDatabaseEntities dc = new MyDatabaseEntities())
                {

                    var loadapp = dc.Appointments; // load all data from unit table to the unit list
                    foreach (var i in loadapp)
                    {
                    DateTime dtStartDate = i.AppointmentDate;
                    DateTime dtStartTime = DateTime.Parse(i.AppointmentStartTime);

                    DateTime startdate = new DateTime(
                                dtStartDate.Year,
                                dtStartDate.Month,
                                dtStartDate.Day,
                                dtStartTime.Hour,
                                dtStartTime.Minute,
                                dtStartTime.Second);

                    DateTime todate = new DateTime(
                               dtStartDate.Year,
                               dtStartDate.Month,
                               dtStartDate.Day,
                               dtStartTime.Hour +2,
                               dtStartTime.Minute,
                               dtStartTime.Second);


                    DataRow _ravi = dt.NewRow();
                    _ravi["start"] = startdate; //i.AppointmentStartTime;
                    _ravi["end"] = todate;//i.AppointmentEndTime;
                    _ravi["name"] = "event1";
                    _ravi["id"] = 1;
                    dt.Rows.Add(_ravi);

                }
            }
         
            return dt;
        }

        internal void EventCreate(DateTime start, DateTime end, string v)
        {

            throw new NotImplementedException();
        }
    }
}