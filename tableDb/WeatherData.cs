using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntelligenceFarmer.tableDb
{
    public class WeatherData
    {

        public double cloud_pct { get; set; }
        public int temp { get; set; }
        public int feels_like { get; set; }
        public int humidity { get; set; }
        public int min_temp { get; set; }
        public int max_temp { get; set; }
        public double wind_speed { get; set; }
        public int wind_degrees { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }
}
