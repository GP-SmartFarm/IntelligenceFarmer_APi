using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntelligenceFarmer.tableDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace IntelligenceFarmer.Controllers
{
    [Route("api/Request")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private GpDbContext gpDb;
        public RequestController(GpDbContext gpDB)
        {
            this.gpDb = gpDB;
            
        }
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<hassan>>> GetRequests()
        {
            //DeleteCookies();
            //var farms = gpDb.table.Where(e=> e.id.Equals(1)).FirstOrDefault();
            //if (farms != null)
            //    return Ok(gpDb.table.Where(e => e.id.Equals(farms.id)));
            //else
            var us = gpDb.table.Select(x => x.name).ToList();
            return Ok(us);
        }
       

        private void DeleteCookies()
        {
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }
        }

        [Produces("application/json")]
        [HttpPost]
       
        [HttpPost]
        public async Task<IActionResult> SendRequest(Request requestInfo)
        {
            if (requestInfo == null)
                return BadRequest("Request is empty.....");
            else
            {
                if (requestInfo.Ssn_Id != null)
                {
                    var userrequest = new Request()
                    {
                        Area = requestInfo.Area,
                        Back_Cardimage = requestInfo.Back_Cardimage,
                        Cartimage_ofAgriculture = requestInfo.Cartimage_ofAgriculture,
                        Front_Cardimage = requestInfo.Front_Cardimage,
                        FullName = requestInfo.FullName,
                        Governorate = requestInfo.Governorate,
                        Image_farm = requestInfo.Image_farm,
                        Latitude = requestInfo.Latitude,
                        Longitude = requestInfo.Longitude,
                        Password_Number = requestInfo.Password_Number,
                        Phone_Number = requestInfo.Phone_Number,
                        Region = requestInfo.Region,
                        Ssn_Id = requestInfo.Ssn_Id,
                        Type_Soil = requestInfo.Type_Soil,
                        UserName = requestInfo.UserName,
                        hieght=requestInfo.hieght

                    };

                    await gpDb.Requests.AddAsync(userrequest);
                    await gpDb.SaveChangesAsync();
                    return Ok("Done.....");
                    //}

                }
                else
                    return BadRequest("change SSn");
            }
            
        }

        private bool check_ssnId(string SSn_id)
        {
            Request user = gpDb.Requests.Where(e => e.Ssn_Id.Equals(SSn_id)).FirstOrDefault();
           // Request user = gpDb.Requests.Where(e => e.Ssn_Id.Equals(SSn_id));
            if (user.Ssn_Id.Equals(SSn_id)) 
                return true;
            else
                return false;
        }
    }
}