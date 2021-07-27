using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IntelligenceFarmer.tableDb
{
    [Table("Request")]
    public class Request
    {
        [Key] 
        public int Request_Id { get; set; } 
        public string FullName { get; set; } 
        public string Ssn_Id { get; set; } 
        public Byte[] Cartimage_ofAgriculture { get; set; } 
        public Byte[] Front_Cardimage { get; set; } 
        public Byte[] Back_Cardimage { get; set; } 
        public string Phone_Number { get; set; } 
        public string UserName { get; set; } 
        public string Password_Number { get; set; } 
        public string Type_Soil { get; set; }
        public Double Area { get; set; }
        public Double hieght { get; set; }
        public string Governorate { get; set; }
        public string Region { get; set; }
        public Byte[] Image_farm { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
    }
}
