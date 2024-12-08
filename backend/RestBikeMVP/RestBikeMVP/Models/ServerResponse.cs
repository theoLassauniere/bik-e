using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RoutingService.Models;

namespace RestBikeMVP.Models
{
    public class ServerResponse
    {
        public List<Segment> Segments { get; set; }
        public List<int> WayPoints { get; set; }
        public List<List<double>> Coordinates { get; set; }
        public string Stations { get; set; }

        public ServerResponse()
        {
            Segments = new List<Segment>();
            WayPoints = new List<int>();
            Coordinates = new List<List<double>>();
            Stations = "";
        }
    }
}
