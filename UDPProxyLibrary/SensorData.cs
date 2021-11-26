using System;

namespace UDPProxyLibrary
{
    public class SensorData
    {
        public int ID { get; set; }
        public String SensorName { get; set; }
        public int Speed { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
