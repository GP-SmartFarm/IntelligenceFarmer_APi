using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IntelligenceFarmer.tableDb;
using java.sql;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace IntelligenceFarmer.Controllers
{

    [Route("api/Hardware/[action]")]
    [ApiController]
    public class HardwareController : ControllerBase
    {
        //private static System.Timers.Timer aTimer;
        private GpDbContext gpDb;
        //private static System.Timers.Timer aTimer;
        public HardwareController(GpDbContext gpDB)
        {
            this.gpDb = gpDB;

        }
      //  private int farmid ;
        private string today = DateTime.Now.ToString("M/dd/yyyy");


        //public ActionResult<IEnumerable<hassan>> GetDaata()
        //{
        //    return gpDb.table.Where(x=>x.name =="hassan").ToList();
        //}
        [HttpGet]
        public async Task<ActionResult> SavedDataFromHardWare()
        {

            var client = new RestClient($"https://api.thingspeak.com/channels/1431281/feeds.json?api_key=0MSU36KNA2PCDT41&results=1");
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);
            var message = JsonConvert.DeserializeObject<Welcome>(response.Content);

            var message2 = message.Feeds;
            var temperature = message.Feeds[0].Field1;
            var humidity = message.Feeds[0].Field2;
            var created_at = message.Feeds[0].CreatedAt.ToString("M/dd/yyyy");
            int idFarm = int.Parse(message.Feeds[0].Field3);
            //farmid = idFarm;
            var sen = new Sensors()
            {
                Farm_Id = idFarm,
                Date_Time = created_at,
                Humidity = double.Parse(humidity),
                Temperature = double.Parse(temperature)
            };
            gpDb.Sensor.Add(sen);
            await gpDb.SaveChangesAsync();
            return Ok(sen);
            //return Ok(temperature);
        }
        [HttpGet]
        public ActionResult showdata()
        {
            return Ok(gpDb.Sensor.Where(s=>s.Date_Time.Equals(today)).ToList());
        }
        
         [HttpGet]
        public async Task<ActionResult> SendDataToHardWare()
        {
            int milliseconds = 15000;
            //var peman = "field1=";
            //var farm_id = "field2=";
            List<double> test = new List<double>();
            List<int> farm = gpDb.farms.Where(x => x.Accept == 1).Select(x => x.Farm_Id).Distinct().ToList();
           // var result = await Cal_Water(farm[0]);
          //  var result1 = await Cal_Water(farm[1]);
            foreach (var i in farm)
            {
                var result = await Cal_Water(i);
                connection(i, result.Item1);
                test.Add(result.Item1);
                Thread.Sleep(milliseconds);
            }
            //connection(farm, test);
          //  connection(farm[0], result.Item1);
         //   int milliseconds = 20000;
         //   Thread.Sleep(milliseconds);
           // connection(farm[1], result1.Item1);
            //var uri = String.Concat(u, peman, result);
            //var client = new RestClient(uri);
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("content-type", "application/json");
            // request.AddParameter("application/json", "{\"messages\": [{\"source\": \"mashape\",\"from\": \"Test\",\"body\": \"This is a test\",\"to\": \"+61411111111\",\"schedule\": \"1452244637\",\"custom_string\": \"this is a test\"}]}", ParameterType.RequestBody);


            return Ok(test);
        }
        public void connection(int i ,double result)
        {
            var u = $"https://api.thingspeak.com/update?api_key=IYQ0WLN5JO50899A&";

            string peman = "field1=" + result;
            string farm_id = "field2=" + i;
            var uri = String.Concat(u, peman, "&", farm_id);
            var client = new RestClient(uri);
            var request = new RestRequest(Method.POST);
            IRestResponse response = client.Execute(request);
        }

        [HttpGet]
        public async Task<Tuple<double, double>> GetWeather(int farmid)
        {
            var z = "&zip=11566&city=helwan&state=Egypt&country=Egypt";
            var Longitude = gpDb.farms.Where(x => x.Farm_Id==farmid&&x.Accept==1).Select(x => new { x.Longitude ,x.Latitude }).Distinct().ToList();
          //  var Latitude = gpDb.farms.Where(x => x.Farm_Id.Equals(farmid)).Select(x => x.Latitude).Distinct().ToList();
           // var l = "lat=31.11&lon=21.14";
            var u = "https://weather-by-api-ninjas.p.rapidapi.com/v1/weather?";
            var uri = String.Concat(u, Longitude[0].Longitude,"&", Longitude[0].Latitude,z);
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {

                Method = HttpMethod.Get,
                RequestUri = new Uri(uri),
                Headers =
                {
                     { "x-rapidapi-key", "25de0ccf33mshd6ddae3b74b5f86p1620fdjsn0c760fdd06c5" },
                     { "x-rapidapi-host", "weather-by-api-ninjas.p.rapidapi.com" },
                 },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<WeatherData>(body);
              
                return Tuple.Create(message.cloud_pct,message.wind_speed);
                //  return Ok(DateTime.Now);
            }
            //    //   return Ok(penman(21,12,84,63,100,22, 6, 8,2.73));
        }
        //[HttpGet]
        [HttpGet("{farmid}")]

        public async Task<Tuple<double>> Cal_Water(int farmid)
        {
            double result = 0;
            string today = DateTime.Now.ToString("MM/dd/yyyy");
            int month = int.Parse(DateTime.Now.Month.ToString());

            //List<double> 
            var farmdata = gpDb.Sensor.Where(x =>x.Farm_Id==farmid&&x.Date_Time.Equals("7/02/2021"));
            List<double> Humidity = farmdata.Select(x => x.Humidity).Distinct().ToList();

            List<double> Temperature = farmdata.Select(x => x.Temperature).Distinct().ToList();

            var farm = gpDb.farms.Where(x => x.Farm_Id.Equals(farmid)).Select(x => new { x.Latitude,x.Area,x.hieght }).Distinct().ToList();

            int Latitude = Convert.ToInt32(farm[0].Latitude);

            //penman(double Tmax , double Tmin  , double Hmax, double Hmin ,int hieght,int latitudes, int Month,double cloud_pct, double windSpeed)
            Tuple<double, double> r  =await GetWeather(farmid);
            if (Humidity.Count != 0 || Temperature.Count != 0) {
                 result = penman(Temperature.Max(), Temperature.Min(), Humidity.Max(), Humidity.Min(),farm[0].hieght , Latitude, month, r.Item1, r.Item2*(1000/3600));

            }
            else
            {
                result = 0;
            }

            double KC = KC_value(farmid);
            await  SaveQuantity(result* KC* farm[0].Area, farmid);

            return Tuple.Create(result * KC * farm[0].Area);
          //  return Tuple.Create(result);

        }
        //[HttpGet("{result}/{farmid}")]
        public async Task SaveQuantity(double result , int farmid)
        {
         //   double result1 = result;
            List<int> Season = gpDb.seasons.Where(x => x.Farm_ID==farmid&&x.End_Season==0).Select(x=>x.Season_Id).Distinct().ToList();
            int seasonId = Season.Max();
            Irregation irregation = new Irregation()
            {
                Season_Id = seasonId,
                Date_time = DateTime.Now.ToString("M/dd/yyyy"),
                Quantity = result
            };
            await gpDb.irregations.AddAsync(irregation);
            await gpDb.SaveChangesAsync();
           // return result1;
        }
        [HttpGet("{farmid}")]
        public double KC_value(int farmid)
        {
            
            int Days_season = 0;
            int x = 0;
            double kc =0;
            var seasonInfo = gpDb.seasons.Where(s => s.Farm_ID.Equals(farmid)&&s.End_Season==0).Select(x => new { x.Crop_Name, x.Date_Beganing }).ToList();
            string today = DateTime.Now.ToString("M/dd/yyyy");
            foreach(var i in seasonInfo)
            {
                Days_season = Date(i.Date_Beganing, today);
                var cropInfo = gpDb.crops.Where(x => x.Crop_Name.Equals(i.Crop_Name)).FirstOrDefault();

                if (Days_season - cropInfo.initial_Day <= 0)
                    return cropInfo.initial_Kc;
                x = cropInfo.initial_Day;
                if (Days_season - (cropInfo.late_Day+x) <= 0)
                    return cropInfo.late_Kc;
                x += cropInfo.late_Day;
                if (Days_season - (cropInfo.mid_Day+x) <= 0)
                    return cropInfo.mid_Kc;
                x += cropInfo.mid_Day;
                if (Days_season - (cropInfo.ripe_Day+x) <= 0)
                    return cropInfo.ripe_Kc;

            }

            return kc;
        }
        
        public int Date(string start , string end)
        {
            var date_start = Convert.ToDateTime(start);
            var date_end = Convert.ToDateTime(end);
            int result = Convert.ToInt32((date_end - date_start).TotalDays);
            return result;
        }

        //public Double Nvalues(int latitudes,int month)
        //{
        //    var re = gpDb.tableNs.Where(x => x.Lat == latitudes).FirstOrDefault();
        //    Double[] N = { re.Jan, re.Feb ,re.Mar, re.Apr, re.May, re.Jun, re.July, re.Aug, re.Sep, re. Oct, re.Nov, re.Dec };

        //    return N[month-1];
        //}

        //public Double Get()
        //{
        //    var Et = Kc * penman(Tmax,  Tmin,  Hmax, Hmin, latitudes, Month,actoalLight ,windSpeed) ;
        //    return Et;
        //}


        // POST: api/Hardware
        //[HttpPost]
        //[Route("api/YOURCONTROLLER/{paramOne}/{paramTwo}")]
        //public  IActionResult PostData(Sensors sensors)
        //{

        //    if (sensors.Equals(null))
        //    {
        //        return BadRequest("null Data...");
        //    }
        //    else
        //    {
        //        var sen = new Sensors() {
        //            Farm_Id = 2,
        //            Date_Time = DateTime.Now.ToString(),
        //            Humidity = sensors.Humidity,
        //            Temperature = sensors.Temperature
        //        };

        //        gpDb.Sensor.Add(sen);
        //        gpDb.SaveChanges();
        //        return Ok(sen.Humidity);
        //    }

        //}
        //[HttpPost("Humidity={Humidity}&Temperature={Temperature}")]
        // [Route("api/Hardware/{Humidity}/{Temperature}")]
        ////[HttpPost]
        // public IActionResult PostData(string Temperature, string Humidity)
        // {

        //     if (Temperature.Equals(null))
        //     {
        //         return BadRequest("null Data...");
        //     }
        //     else
        //     {
        //         var sen = new Sensors()
        //         {
        //             Farm_Id = 2,
        //             Date_Time = DateTime.Now.ToString(),
        //             Humidity = int.Parse(Humidity) ,
        //             Temperature = int.Parse(Temperature)
        //         };

        //         gpDb.Sensor.Add(sen);
        //         gpDb.SaveChanges();
        //         return Ok(sen.Humidity);
        //     }

        // }
        [HttpGet("{latitudes}/{month}")]
        public Double extraterrestrialValue(int latitudes, int month)
        {
            List<double> N = new List<double>();
            var re = gpDb.extraterrestrial_Radiations.Where(x => x.Lat == latitudes).FirstOrDefault();
            if (re != null) {
                N.Add(re.Jan);
                N.Add(re.Feb);
                N.Add(re.Mar);
                N.Add(re.Apr);
                N.Add(re.May);
                N.Add(re.Jun);
                N.Add(re.July);
                N.Add(re.Aug);
                N.Add(re.Sep);
                N.Add(re.Oct);
                N.Add(re.Nov);
                N.Add(re.Dec);
                return N[month];
            }
           

            //  Double[] N = { re.Jan, re.Feb, re.Mar, re.Apr, re.May, re.Jun, re.July, re.Aug, re.Sep, re.Oct, re.Nov, re.Dec };

            return 0;
        }

        public Double eo(double t)
        {
            var x = (17.27 *t ) / (t + 237.3);
            return 0.6108*Math.Exp(x);
        }
        public Double slopeFun(double t)
        {
            return 4098 * eo(t) / Math.Pow((t + 237.3), 2);
        }
        [HttpGet("{Tmax}/{Tmin}/{Hmax}/{Hmin}/{hieght}/{latitudes}/{Month}/{cloud_pct}/{windSpeed}")]
        public Double penman(double Tmax , double Tmin  , double Hmax, double Hmin ,double hieght,int latitudes, int Month,double cloud_pct, double windSpeed)
        {
            var p = 0.067;
            var Tmean = (Tmax + Tmin) / 2;
            var Es = (eo(Tmin) + eo(Tmax)) / 2;
            var Slope = slopeFun(Tmean);
            var Ea = ((eo(Tmin) * (Hmax / 100)) + ((eo(Tmax) * (Hmin / 100)))) / 2 ;

            var Ra = extraterrestrialValue(latitudes, Month);
            if (Ra != 0)
            {
                var Rs = (0.25 + 0.5 * (100 - cloud_pct) / 100);
                var Rso = (0.75 + 2 * hieght * Math.Pow(10, -5)) * Ra;
                var Rns = (1 - 0.23) * Rs;
                var Rnl = 4.903 * Math.Pow(10, -9) * ((Math.Pow(Tmax + 273.16, 4)) + Math.Pow(Tmin + 273.16, 4)) / 2 * (0.34 - 0.14 * Math.Pow(Ea, 0.5)) * (1.35 * (Rs / Rso) - 0.35);
                var Rn = Rns - Rnl;
                var ETo = ((0.408 * Slope * (Rn - 0)) + (p * (900 / (Tmean + 273)) * windSpeed * (Es - Ea))) / (Slope + (p * (1 + 0.34 * windSpeed)));
                return ETo;

            }
            //  var N = Nvalues(latitudes, Month);
            return 0;
        }

    }
}
