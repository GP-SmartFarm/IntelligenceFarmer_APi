using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace IntelligenceFarmer.tableDb
{
    [Table("Sensors")]
    public class Sensors
    {
        [Key]
        public int Sensor_Id { get; set; }
        public int Farm_Id { get; set; }
        public string Date_Time { get; set; }
        public double Humidity { get; set; }
        public double Temperature { get; set; }
    }
  
   
}
