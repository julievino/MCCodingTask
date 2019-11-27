using System;
using System.Web.Mvc;
using System.Collections.Generic;
using InterviewTask.Services;
using InterviewTask.Models;
namespace InterviewTask.Controllers
{
    public class HomeController : Controller
    {
        /*
         * Prepare your opening times here using the provided HelperServiceRepository class.       
         */
        public ActionResult Index()
        {


            HelperServiceRepository helperServiceCenter1 = new HelperServiceRepository();

            IEnumerable<HelperServiceModel> helperServiceCenterInfoWithOpeningtimes = helperServiceCenter1.Get();

            foreach (var helpcenter in helperServiceCenterInfoWithOpeningtimes)
            {
                int dayOfWeek = (int)System.DateTime.Now.DayOfWeek;


                List<int> openingHours = null;
                List<int> openingHoursNextDay = null;
                
                switch (dayOfWeek)
                {
                    case 1:
                        openingHours = helpcenter.MondayOpeningHours;
                        openingHoursNextDay = helpcenter.TuesdayOpeningHours;
                        break;
                    case 2:
                        openingHours = helpcenter.TuesdayOpeningHours;
                        openingHoursNextDay = helpcenter.WednesdayOpeningHours;
                        break;
                    case 3:
                        openingHours = helpcenter.WednesdayOpeningHours;
                        openingHoursNextDay = helpcenter.ThursdayOpeningHours;
                        break;
                    case 4:
                        openingHours = helpcenter.ThursdayOpeningHours;
                        openingHoursNextDay = helpcenter.FridayOpeningHours;
                        break;
                    case 5:
                        openingHours = helpcenter.FridayOpeningHours;
                        openingHoursNextDay = helpcenter.SaturdayOpeningHours;
                        break;
                    case 6:
                        openingHours = helpcenter.SaturdayOpeningHours;
                        openingHoursNextDay = helpcenter.SundayOpeningHours;
                        break;
                    case 0:
                        openingHours = helpcenter.SundayOpeningHours;
                        openingHoursNextDay = helpcenter.MondayOpeningHours;
                        break;


                }

                if (openingHours != null)
                {

                    TimeSpan start = new TimeSpan(openingHours[0], 0, 0); 
                    TimeSpan end = new TimeSpan(openingHours[1], 0, 0); 
                    TimeSpan now = DateTime.Now.TimeOfDay;


                    if ((now > start) && (now < end))
                    {
                        //match found
                        helpcenter.CurrentStatus = "open";
                        helpcenter.CurrentStatusInfo = "OPEN - OPEN TODAY UNTIL "+ openingHours[0].ToString() + "AM";
                    }
                    if ((now < start) && (now < end))
                    {
                        helpcenter.CurrentStatus = "close";
                        helpcenter.CurrentStatusInfo = "CLOSED - OPEN TODAY AT " + openingHours[0].ToString() + "AM";
                    }
                    else
                    {
                        helpcenter.CurrentStatus = "close";
                        helpcenter.CurrentStatusInfo = "CLOSED - REOPENS " + DateTime.Now.AddDays(1).DayOfWeek.ToString() + " at " + openingHoursNextDay[0].ToString() + "AM";
                    }
                }
                else
                {
                    helpcenter.CurrentStatus = "Not available";
                    helpcenter.CurrentStatusInfo = "Not available";

                }
               
            }
            return View(helperServiceCenterInfoWithOpeningtimes);
        }
    }
}