using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UDPProxyLibrary;

namespace UDPProxyREST.Managers
{
    public class SpeedManager
    {
        //Could be a dictionary for easier lookup of unique sensordata names
        private static List<SensorData> _data = new List<SensorData>();
        //A simple int to keep track of ID's
        private static int _nextID = 1;

        //Return a copy of the list
        public List<SensorData> GetAll()
        {
            return new List<SensorData>(_data);
        }

        public List<SensorData> GetAllFromName(string name)
        {
            //Iterates the entire list, and only finds the SensorData with the given name (case insensitive)
            return _data.FindAll(s => s.SensorName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        public List<SensorData> GetAllBetween(DateTime startTime, DateTime endTime)
        {
            //Iterates the entire list, and only finds the SensorData with is between the two parameter dates (both including)
            return _data.FindAll(s => s.TimeStamp >= startTime && s.TimeStamp <= endTime);
        }

        public IEnumerable<string> GetAllUniqueNames()
        {
            //Using LinQ to find the unique names in the list
            return _data.Select(s => s.SensorName).Distinct();
        }

        //Adds an object to the list, and assign a unique ID, and adds a timestamp of the current time
        public int Add(SensorData newData)
        {
            newData.ID = _nextID++;
            newData.TimeStamp = DateTime.Now;
            _data.Add(newData);
            return newData.ID;
        }
    }
}
