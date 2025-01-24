using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HowsTheWeather.JSON_Response
{
    public class WeatherResponse
    {
        public Main main;
        public List<Weather> weather;
        public string name;
    }
}
