using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IntelligenceFarmer.tableDb;
using Microsoft.EntityFrameworkCore;

namespace IntelligenceFarmer.Controllers
{
    [Route("api/User/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private GpDbContext gpDb;
        public UserController(GpDbContext gpDB)
        {
            this.gpDb = gpDB;

        }

        [HttpGet("{FarmId}")]
        public ActionResult ShowSensorsData(int FarmId)
        {
            List<int> z = new List<int>();
            string today = DateTime.Now.ToString("M/dd/yyyy");
            var SensorsData = gpDb.Sensor.Where(s => s.Farm_Id.Equals(FarmId)&&s.Date_Time.Contains(today));
            if (SensorsData == null)
                return BadRequest(z);
            else
                return Ok(SensorsData);
        }

        [HttpGet("{SeasonId}")]
        public ActionResult Showirregation(int SeasonId)
        {
            // DateTime.Now.ToString("M/dd/yyyy")
            var irregationsData = gpDb.irregations.Where(s => s.Season_Id.Equals(SeasonId));
            return Ok(irregationsData.Select(i=>new { i.Date_time,i.Quantity}));
        }
      
        [HttpGet("{username}/{password}")]
        public ActionResult login(string username, string password)
        {
            // gpDb.user.Where(x=>x.UserName.Equals(username) && x.Password_Number.Equals(Password))
            int userid = check(username, password);
            if (userid == 0)
                return BadRequest("User Name or Password is Wrong");
            else
                return Ok(userid);
        }

        public int check(string username, string password)
        {
            var users = gpDb.user.Where(x => x.UserName.Equals(username)).ToList();
            int userid = 0;
            if (!users.Equals(null))
            {
                foreach (var i in users)
                {
                    if (i.Password_Number == password)
                        userid = i.User_Id;
                        
                }
            }
            if (userid != 0)
                return userid;
            else
                return 0;
        }
        [HttpGet("{userid}/{status}")]
        public ActionResult GetFarms(int userid ,string status)
        {
            //   var farm = gpDb.farms;
            List<int> x = new List<int>();
            if (userid.Equals(null))
                return BadRequest("try login again");
            else
            {
                if (status.Equals("accept"))
                {
                    var accept = gpDb.farms.Where(f => f.User_Id.Equals(userid) && f.Accept == 1).ToList();
                    if (accept.Count!=0)
                        return Ok(accept);
                    else
                        return BadRequest(x);
                }
                    
                else if (status.Equals("waiting"))
                {
                    var wiatting = gpDb.farms.Where(f => f.User_Id.Equals(userid) && f.Accept == 0).ToList();
                    if (wiatting !=null)
                        return Ok(wiatting);
                    else
                        return BadRequest(x);
                }
                    

                return BadRequest("...");
            }
            
        }



        [HttpGet("{userid}")]
        public ActionResult UserInfo(int userid)
        {
            if (userid.Equals(null))
                return BadRequest("try login again");
            else
            {
                var user = gpDb.user.Where(f => f.User_Id.Equals(userid)).ToList();
                if(user.Count==0)
                    return BadRequest("user not fuond");
                return Ok(user.Select(p=>new { Full_Name = p.FullName , User_Name = p.UserName }));
            }

        }

        [HttpPost]
        public async Task<IActionResult> AddFarm(Farms farm)
        {
            if (farm == null)
                return BadRequest("farm is empty.....");
            else
            {
                Farms newfarm = new Farms() { 
                     Area = farm.Area,
                     Governorate = farm.Governorate,
                     Image_farm = farm.Image_farm,
                     Latitude = farm.Latitude,
                     Longitude = farm.Longitude,
                     Region = farm.Region,
                     Type_Soil = farm.Type_Soil,
                     User_Id =farm.User_Id,
                     hieght=farm.hieght
                };
                await gpDb.farms.AddAsync(newfarm);
                await gpDb.SaveChangesAsync();
                return Ok("Done to creat Farm");
            }
        }
        [HttpGet("{FarmId}")]
        public async Task<IActionResult> DeleteFarm(int FarmId)
        {
            if (FarmId.Equals(null))
                return BadRequest("erorr id is empty");
            Farms farm = gpDb.farms.Find(FarmId);
            if (farm == null)
                return BadRequest("Not Found the farm");
            else
            {
                gpDb.farms.Remove(farm);
                await gpDb.SaveChangesAsync();
                return Ok("Done to delete the farm...");
            }

        }
       
        [HttpPost]
        public async Task<IActionResult> AddSeason(Season season)
        {
            string time = DateTime.Now.ToString("M/dd/yyyy");
            if (season == null)
                return BadRequest("season is empty.....");
            else
            {
                Season newSeason = new Season() {
                    Crop_Name = season.Crop_Name,
                    Farm_ID = season.Farm_ID,
                    Date_Beganing = time,
                    End_Season=0
                };
               
                await gpDb.seasons.AddAsync(newSeason);
                await gpDb.SaveChangesAsync();
                return Ok("Done to creat Season");
            }
        }
        [HttpGet("{FarmID}/{status}")]
        public IActionResult ShowSeason(int FarmID ,string status) {
            var Seasons = gpDb.seasons;
            if (FarmID.Equals(null))
                return BadRequest("Error...");
            else
            {
                if (status.Equals("end"))
                    return Ok(Seasons.Where(s => s.Farm_ID.Equals(FarmID) && s.End_Season.Equals(1)).ToList());
                else if(status.Equals("active"))
                    return Ok(Seasons.Where(s => s.Farm_ID.Equals(FarmID) && s.End_Season.Equals(0)).ToList());
                //var Seasons = gpDb.seasons.Where(s=>s.Farm_ID.Equals(FarmID)&&s.End_Season.Equals(0)).ToList();
                if (Seasons.Equals(null))
                    return BadRequest("Farm not found");
                return BadRequest("");

            }
        }
        //[HttpGet("{FarmID}")]
        //public IActionResult ShowSeasonEnd(int FarmID)
        //{
        //    if (FarmID.Equals(null))
        //        return BadRequest("Error...");
        //    else
        //    {
        //        var Seasons = gpDb.seasons.Where(s => s.Farm_ID.Equals(FarmID) && s.End_Season.Equals(1)).ToList();
        //        if (Seasons.Equals(null))
        //            return BadRequest("Farm not found");
        //        return Ok(Seasons);
        //    }
        //}
        [HttpGet("{SeasonId}")]
        public async Task<IActionResult> EndSeason(int SeasonId)
        {
            if (SeasonId.Equals(null))
                return BadRequest("erorr id is empty");
            Season Seasons = gpDb.seasons.Find(SeasonId);
            if (Seasons == null)
                return BadRequest("Not Found the season");
            else if (ModelState.IsValid)
            {
                Seasons.End_Season = 1;
                gpDb.Entry(Seasons).State = EntityState.Modified;
               //  gpDb.seasons.Remove(Seasons);
                await gpDb.SaveChangesAsync();
    
                return Ok("Done to End the Season...");
            }
            return BadRequest();
        }
        [HttpGet]
        public IActionResult showCrops()
        {
            return Ok(gpDb.crops.Select(c => c.Crop_Name).ToList());
        }
        
    }

}