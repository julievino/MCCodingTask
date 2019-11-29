using System;
using System.Web.Mvc;
using System.Collections.Generic;
using InterviewTask.Services;
using InterviewTask.Models;
using System.Collections;
using InterviewTask.Helpers;

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
                Util.Logger("excpetion raised: " + ex.InnerException);
                return View("Error");
            }
            finally
            {
                Util.Logger("Time of Access: " + DateTime.Now + "IP address: " + GetIpValue());
            }
        }

        public void PrepareOpeningTimes(HelperServiceModel helpcenter)
        {
            int dayOfWeek = (int)System.DateTime.Now.DayOfWeek;

            List<int> openingHours = FindOpeningTimes(helpcenter, dayOfWeek);

            SetHelpCenterCurrentStatus(helpcenter, openingHours);

        }

        private static List<int> FindOpeningTimes(HelperServiceModel helpcenter, int dayOfWeek)
        {
            List<int> openingHours = null;
            switch (dayOfWeek)
            {

                case 0:
                    openingHours = helpcenter.SundayOpeningHours;
                    break;
                case 1:
                    openingHours = helpcenter.MondayOpeningHours;
                    break;

                case 2:
                    openingHours = helpcenter.TuesdayOpeningHours;
                    break;

                case 3:
                    openingHours = helpcenter.WednesdayOpeningHours;
                    break;

                case 4:
                    openingHours = helpcenter.ThursdayOpeningHours;
                    break;

                case 5:
                    openingHours = helpcenter.FridayOpeningHours;
                    break;

                case 6:
                    openingHours = helpcenter.SaturdayOpeningHours;
                    break;
                default:
                    //This should never hit, System.DateTime.Now.DayOfWeek should give a value within 0-6
                    break;


            }

            return openingHours;
        }

        private string FindNextWorkingDayDisplayText(HelperServiceModel helpcenter)
        {
            string nextWorkingDayDisplayText = string.Empty;
            int dayOfWeek = (int)System.DateTime.Now.DayOfWeek;
            ArrayList holidaysOfWeek = new ArrayList();
            List<int> NextWorkingDay;


            if (helpcenter.SundayOpeningHours != null && helpcenter.SundayOpeningHours[0] == 0)
                holidaysOfWeek.Add(0);

            if (helpcenter.MondayOpeningHours != null && helpcenter.MondayOpeningHours[0] == 0)
                holidaysOfWeek.Add(1);

            if (helpcenter.TuesdayOpeningHours != null && helpcenter.TuesdayOpeningHours[0] == 0)
                holidaysOfWeek.Add(2);
            if (helpcenter.WednesdayOpeningHours != null && helpcenter.WednesdayOpeningHours[0] == 0)
                holidaysOfWeek.Add(3);
            if (helpcenter.ThursdayOpeningHours != null && helpcenter.ThursdayOpeningHours[0] == 0)
                holidaysOfWeek.Add(4);
            if (helpcenter.FridayOpeningHours != null && helpcenter.FridayOpeningHours[0] == 0)
                holidaysOfWeek.Add(5);
            if (helpcenter.SaturdayOpeningHours != null && helpcenter.SaturdayOpeningHours[0] == 0)
                holidaysOfWeek.Add(6);
            

            do
            {

              dayOfWeek++;
              dayOfWeek = dayOfWeek % 7;
            } while (holidaysOfWeek.Contains(dayOfWeek));

            NextWorkingDay = FindOpeningTimes(helpcenter, dayOfWeek);


            nextWorkingDayDisplayText= "CLOSED - REOPENS " + Enum.GetName(typeof(DayOfWeek), dayOfWeek) + " at " + NextWorkingDay[0].ToString() + " AM";
            return nextWorkingDayDisplayText;



        }

        public void SetHelpCenterCurrentStatus( HelperServiceModel helpcenter, List<int> openingHours)
        {
            if (openingHours != null)
            {

                TimeSpan start = new TimeSpan(openingHours[0], 0, 0);
                TimeSpan end = new TimeSpan(openingHours[1], 0, 0);
                TimeSpan now = DateTime.Now.TimeOfDay;
                var dateTimeEnd = new DateTime(end.Ticks); // Date part is 01-01-0001
                var formattedEndTime = dateTimeEnd.ToString("hh:mm");
                var dateTimeStart = new DateTime(start.Ticks); // Date part is 01-01-0001
                var formattedStartTime = dateTimeStart.ToString("hh:mm");

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
                    helpcenter.CurrentStatusInfo = FindNextWorkingDayDisplayText(helpcenter);
                }
            }
            else
            {
                helpcenter.CurrentStatus = "Not available";
                helpcenter.CurrentStatusInfo = "We're sorry, we are temporarily unable to display";

            }
           

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