using CityInfoAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsOfInterest")]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if(city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{id}", Name = "SinglePointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //find point of interest
            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);
            if(pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(pointOfInterest);
        }

        [HttpPost]
        //We could explicitly use [FromBody] to deserialize the pointOfInterest from the request body but [ApiController] attribute already takes care of that
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            //We could also let consumer know they sent a bad request if pointOfInterest cannot be desrialised but [ApiController] attribute also takes care of that
            //if (pointOfInterest == null)
            //{
            //    return BadRequest();
            //}

            //check whether the city to which the pointOfInterest is being created exists
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //calculate pointOfInterestDto
            //since our data store currently works on a PointOfInterestDto and not PointOfInterestForCreationDto we have to apply mapping
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c=>c.PointsOfInterest).Max(p=>p.Id);
     
            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            //Because the "GetPointOfInterest" route expects two parameters we pass them in using an anonymous type
            //Response body should also contain the newly created pointOfInterest resource
            return CreatedAtRoute("SinglePointOfInterest", new { cityId, id = finalPointOfInterest.Id}, finalPointOfInterest);
        }
    }
}
