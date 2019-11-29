using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InterviewTask.Models;
using System.Net;
using System.IO;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using Newtonsoft.Json;

namespace InterviewTask.Controllers
{
    
    public class OpenWeatherMapController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

      
        [HttpGet]
        public ActionResult ShowMap(string city)
        {
            OpenWeatherMap openWeatherMap = new OpenWeatherMap();

            if (city != null && city!=string.Empty)
            {
                /*Calling API http://openweathermap.org/api */
                string apiKey = "41fc4affe642cca03f311fa41b430921";

                HttpWebRequest apiRequest =
                WebRequest.Create("https://api.openweathermap.org/data/2.5/weather?q="+ city + "&appid=" + apiKey + "&units=metric") as HttpWebRequest;
                 string apiResponse = "";
                try
                {
                    
                    using (HttpWebResponse response = apiRequest.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        apiResponse = reader.ReadToEnd();
                        OpenWeatherMapServiceModel.ResponseWeather rootObject = JsonConvert.DeserializeObject<OpenWeatherMapServiceModel.ResponseWeather>(apiResponse);

                        StringBuilder sb = new StringBuilder();
                        sb.Append("<table><tr><th>Weather Description</th></tr>");
                        sb.Append("<tr><td>City:</td><td>" +
                        rootObject.name + "</td></tr>");
                        sb.Append("<tr><td>Country:</td><td>" +
                        rootObject.sys.country + "</td></tr>");
                        sb.Append("<tr><td>Wind:</td><td>" +
                        rootObject.wind.speed + " Km/h</td></tr>");
                        sb.Append("<tr><td>Current Temperature:</td><td>" +
                        rootObject.main.temp + " °C</td></tr>");
                        sb.Append("<tr><td>Humidity:</td><td>" +
                        rootObject.main.humidity + "</td></tr>");
                        sb.Append("<tr><td>Weather:</td><td>" +
                        rootObject.weather[0].description + "</td></tr>");
                        sb.Append("</table>");
                        openWeatherMap.apiResponse = sb.ToString();
                    }
                }
                catch (Exception ex)
                {
                    //Log exception
                }
                /*End*/
               
               
            }
            else
            {
                openWeatherMap.apiResponse = "Not Available";
            }
            
            return View(openWeatherMap);
        }
    }
}
