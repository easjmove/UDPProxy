using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UDPProxyLibrary;
using UDPProxyREST.Managers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UDPProxyREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpeedController : ControllerBase
    {
        //Internal reference to a manager
        //for improvement, could be an Interface instead, so it will be easier to change
        private SpeedManager _manager = new SpeedManager();

        //Simply returns all sensordata
        //Return a 204 if no sensors has send any data
        //Instead of 204 we could use 404, but that is seen as an error
        // GET: api/Speed
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<SensorData>> Get()
        {
            IEnumerable<SensorData> result = _manager.GetAll();
            if (result.Count() > 0)
            {
                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        //Returns all sensordata between the starttime and endtime specified
        //Here we return a 404 instead of the 204, as the caller asked for a specific time period, which doesn't contain data
        // GET: api/Speed/time?startTime=2021-11-26T13%3A04%3A12&endTime=2021-11-26T13%3A04%3A14
        //Here notice in the example that the timestamp looks weird, its is because of : and other characters that we need to encode before a browser will see them properly
        //Using this in C#, look into: HttpUtility.UrlEncode
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("time")]
        public ActionResult<IEnumerable<SensorData>> GetAllBetween([FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            IEnumerable<SensorData> result = _manager.GetAllBetween(startTime, endTime);
            if (result.Count() > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("No data found between the start and end time period");
            }
        }

        //Simply returns all sensordata that a specific sensor send
        //Here we return a 404 instead of the 204, as the caller asked for a specific sensorname, which we don't have
        // GET: api/Speed/MoveSpeedtrap
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("names/{sensorName}")]
        public ActionResult<IEnumerable<SensorData>> Get(string sensorName)
        {
            IEnumerable<SensorData> result = _manager.GetAllFromName(sensorName);
            if (result.Count() > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound("No such name exists at this time");
            }
        }

        //added a unique route this method, so that it knows when to execute this instead of the normal Get
        //Return a 204 if no sensors has send any data (meaning we have no names)
        // GET: api/Speed/Names
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("names")]
        public ActionResult<IEnumerable<string>> GetUniqueNames()
        {
            IEnumerable<string> result = _manager.GetAllUniqueNames();
            if (result.Count() > 0)
            {
                return Ok(result);
            }
            else
            {
                return NoContent();
            }
        }

        //Adds a sensordata object, delegates the responsibility of added timestamp and id to the manager
        //Here we use the 201 status code to tell the user that it has been created, but because we have no access to the specific sensordata, we just return the Uri to the get method
        // POST api/Speed
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public ActionResult Post([FromBody] SensorData value)
        {
            _manager.Add(value);
            return Created("/api/speed", value);
        }
    }
}
