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
            try
            {

                HelperServiceRepository helperServiceCenter1 = new HelperServiceRepository();

                IEnumerable<HelperServiceModel> helperServiceCenterInfoWithOpeningtimes = helperServiceCenter1.Get();

                foreach (var helpcenter in helperServiceCenterInfoWithOpeningtimes)
                {
                    PrepareOpeningTimes(helpcenter);
                }
                return View(helperServiceCenterInfoWithOpeningtimes);
            }
            catch (Exception ex)
            {
                Logger("excpetion raised: " + ex.InnerException);
                return View("Error");
            }
            finally
            {
                Logger("Time of Access: " + DateTime.Now + "IP address: " + GetIpValue());
            }
        }

        public void PrepareOpeningTimes(HelperServiceModel helpcenter)
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

            SetHelpCenterCurrentStatus(openingHours, openingHoursNextDay, helpcenter);

        }

        public void SetHelpCenterCurrentStatus(List<int> openingHours, List<int> openingHoursNextDay, HelperServiceModel helpcenter)
        {
            if (openingHours != null)
            {

                TimeSpan start = new TimeSpan(openingHours[0], 0, 0);
                TimeSpan end = new TimeSpan(openingHours[1], 0, 0);
                TimeSpan now = DateTime.Now.TimeOfDay;
                var dateTimeEnd = new DateTime(end.Ticks); // Date part is 01-01-0001
                var formattedEndTime = dateTimeEnd.ToString("h:mm");
                var dateTimeStart = new DateTime(start.Ticks); // Date part is 01-01-0001
                var formattedStartTime = dateTimeStart.ToString("h:mm");

                if ((now > start) && (now < end))
                {
                    //match found
                    helpcenter.CurrentStatus = "open";
                    helpcenter.CurrentStatusInfo = "OPEN - OPEN TODAY UNTIL " + formattedEndTime.ToString() + " PM";
                }
                else if ((now < start) && (now < end))
                {
                    helpcenter.CurrentStatus = "close";
                    helpcenter.CurrentStatusInfo = "CLOSED - OPEN TODAY AT " + formattedStartTime.ToString() + " AM";
                }
                else
                {
                    helpcenter.CurrentStatus = "close";
                    helpcenter.CurrentStatusInfo = "CLOSED - REOPENS " + DateTime.Now.AddDays(1).DayOfWeek.ToString() + " at " + openingHoursNextDay[0].ToString() + " AM";
                }
            }
            else
            {
                helpcenter.CurrentStatus = "Not available";
                helpcenter.CurrentStatusInfo = "We're sorry, we are temporarily unable to display";

            }
           

        }

        public void Logger(String lines)
        {

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log

            System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\test.txt", true);
            file.WriteLine(lines);

            file.Close();

        }


        private string GetIpValue()
        {
            string ipAdd = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAdd))
            {
                ipAdd = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ipAdd;
            
        }

    }
   
    
}