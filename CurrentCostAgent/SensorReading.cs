using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CurrentCostAgent
{
    public class SensorReading
    {
        public int Timestamp { get; set; }
        public int Sensor { get; set; }
        public int Watts { get; set; }
    }
}
