using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Controllers
{
    //Attribute not strictly necessary but aids api development experience such as routing (400 bad requests) etc
    [ApiController]

    //prefix of class name is cities and we can therefore substitute [controller] for it
    //[Route("api/[controller]")]
    [Route("api/cities")]
    //Extending to controller and not ControllerBase would give us access to views which isn't necessary when building an API
    public class CitiesController : ControllerBase
    {
        [HttpGet] 
        public IActionResult GetCities()
        {
            return Ok(CitiesDataStore.Current.Cities);
            //return new JsonResult(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
        {
            var requestedCity = CitiesDataStore.Current.Cities.FirstOrDefault(c=>c.Id == id);
            if(requestedCity == null)
            {
                return NotFound();
            }

            return Ok(requestedCity);
        }
    }
}
