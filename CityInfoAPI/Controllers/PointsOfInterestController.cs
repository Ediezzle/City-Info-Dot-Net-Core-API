using CityInfoAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
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

            if(pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from name");
            }

            //We need to explicitly do this since model binding would have already happened before we modified thew model state hence it's too late for the [ApiController] attribute to handle this
            //To avoid this tedious way of mixing up validation and controller actions we could use a third party library such as Fluent Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

        [HttpPut("{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from name");
            }

            //We need to explicitly do this since model binding would have already happened before we modified thew model state hence it's too late for the [ApiController] attribute to handle this
            //To avoid repeating this check we could use a third party library such as Fluent Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //check whether the city to which the pointOfInterest is being created exists
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //calculate pointOfInterestDto
            //since our data store currently works on a PointOfInterestDto and not PointOfInterestForCreationDto we have to apply mapping
            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            //According to Http standards Put should fully update a resource
            //We already have the id coming from the Api. If we didn't and we didn't provide it it would fall back to the default value
            pointOfInterestFromStore.Name = pointOfInterest.Name;
            pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestFromStore.Name,
                Description = pointOfInterestFromStore.Description
            };

            //Pass in ModelState as second argument to validate the input from the client
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            //N.B In this case the input model to be validated will be the JsonApacheDocument , not the pointOfInterestForUpdateDto
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Now we validate our pointOfInterestForUpdateDto
            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from name");
            }

            if(!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }
    }
}
