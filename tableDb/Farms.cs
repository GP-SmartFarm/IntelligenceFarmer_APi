using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace IntelligenceFarmer.tableDb
{
    [Table("farms")]
    public class Farms
    {

        [Key]
        public int Farm_Id { get; set; }
       
        //  [ForeignKey("User_Id")]
        public int User_Id { get; set; }
        //public Users users { get; set; }
        public Double hieght { get; set; }

        public string Type_Soil { get; set; }
        public Double Area   { get; set; }
        public string Governorate { get; set; }
        public string Region { get; set; }
        public Byte[] Image_farm { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public int Accept { get; set; }
    }
    [Table("Crop")]
    public class Crop
    {

        [Key]
        public string Crop_Name { get; set; }
        public int initial_Day { get; set; }
        public double initial_Kc { get; set; }
        public int mid_Day { get; set; }
        public double mid_Kc { get; set; }
        public int late_Day { get; set; }
        public double late_Kc { get; set; }
        public int ripe_Day { get; set; }
        public double ripe_Kc { get; set; }



    }
    [Table("Season")]
    public class Season
    {

        [Key]
        public int Season_Id { get; set; }
        public int Farm_ID { get; set; }
        public string Date_Beganing { get; set; }
        public string Crop_Name { get; set; }
        public int End_Season { get; set; }
    }
    [Table("Irregation")]
    public class Irregation
    {

        [Key]
        public int Irregation_Id { get; set; }
        public int Season_Id { get; set; }
        public string Date_time { get; set; }
        public double Quantity { get; set; }

    }


}
