using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class LichtSignalAnlage
    {
        public string Unit { get; set; }
        public string NodeName { get; set; }
        public string DeviceType { get; set; }
        public double UtmEast { get; set; }
        public double UtmNorth { get; set; }

        public LichtSignalAnlage(string unit, string node, string type, double east, double north)
        {
            Unit = unit;
            NodeName = node;
            DeviceType = type;
            UtmEast = east;
            UtmNorth = north;
        }
    }
}
