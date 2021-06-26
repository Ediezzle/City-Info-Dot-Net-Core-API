using AutoMapper;
using CityInfoAPI.Models;
using CityInfoAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsOfInterest")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));

            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            
            try
            {
                //throw new Exception("Exception example");

                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

                //if (city == null)
                //{
                //    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");

                //    return NotFound();
                //}

                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} wasn't found when accessing points of interest.");
                    return NotFound();
                }

                var pointsOfInterestForCity = _cityInfoRepository.GetPointsOfInterestForCity(cityId);

                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterestForCity));
            }

            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex);
                return StatusCode(500, "A problem happened whilst handling your request!");
            }
  
        }

        [HttpGet("{id}", Name = "SinglePointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            if(!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterestForCity(cityId, id);

            if(pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //{
            //    return NotFound();
            //}

            ////find point of interest
            //var pointOfInterest = city.PointsOfInterest.FirstOrDefault(c => c.Id == id);
            //if(pointOfInterest == null)
            //{
            //    return NotFound();
            //}

            //return Ok(pointOfInterest);
        }

        [Route("testPointsOfInterest")]
        [HttpGet]
        public IActionResult GetPointOfInterestTest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
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
           
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            //code when using in-memory data store (citiesDataStore) 
            //calculate pointOfInterestDto
            //since our data store currently works on a PointOfInterestDto and not PointOfInterestForCreationDto we have to apply mapping
            //var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(c=>c.PointsOfInterest).Max(p=>p.Id);
            //var finalPointOfInterest = new PointOfInterestDto()
            //{
            //    Id = ++maxPointOfInterestId,
            //    Name = pointOfInterest.Name,
            //    Description = pointOfInterest.Description
            //};

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest
                >(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            _cityInfoRepository.Save();

            var createdPointOfInterestToReturn = _mapper.
                Map<Models.PointOfInterestDto>(finalPointOfInterest);

            //Because the "GetPointOfInterest" route expects two parameters we pass them in using an anonymous type
            //Response body should also contain the newly created pointOfInterest resource
            return CreatedAtRoute("SinglePointOfInterest", new { cityId, id = createdPointOfInterestToReturn.Id}, createdPointOfInterestToReturn);
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

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.
                GetPointOfInterestForCity(cityId, id);
            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            //won't execute anything in this case but just a good habit to make code consistent, stable and reliable
            _cityInfoRepository.UpdatePointOfInterestForCity(cityId, pointOfInterestEntity);

            _cityInfoRepository.Save();

            //According to Http standards Put should fully update a resource
            //We already have the id coming from the Api. If we didn't and we didn't provide it it would fall back to the default value
            //pointOfInterestFromStore.Name = pointOfInterest.Name;
            //pointOfInterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }

        //[HttpPatch("{id}")]
        //public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        //{
        //    var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
        //    if (pointOfInterestFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
        //    {
        //        Name = pointOfInterestFromStore.Name,
        //        Description = pointOfInterestFromStore.Description
        //    };

        //    //Pass in ModelState as second argument to validate the input from the client
        //    patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

        //    //N.B In this case the input model to be validated will be the JsonApacheDocument , not the pointOfInterestForUpdateDto
        //    if(!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    //Now we validate our pointOfInterestForUpdateDto
        //    if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
        //    {
        //        ModelState.AddModelError("Description", "The provided description should be different from name");
        //    }

        //    if(!TryValidateModel(pointOfInterestToPatch))
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    pointOfInterestFromStore.Name = pointOfInterestToPatch.Name;
        //    pointOfInterestFromStore.Description = pointOfInterestToPatch.Description;

        //    return NoContent();
        //}

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            //Apache operation is on the DTO and not directly on the entity as we shouldn't expose Entity implementation details to the outer-facing layer
            //Thus we first need to find the entity and map it to a DTO before patching
          
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.
                GetPointOfInterestForCity(cityId, id);
            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.
                Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            //Pass in ModelState as second argument to validate the input from the client
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            //N.B In this case the input model to be validated will be the JsonApacheDocument , not the pointOfInterestForUpdateDto
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Now we validate our pointOfInterestForUpdateDto
            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from name");
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);

            //won't execute anything in this case but just a good habit to make code consistent, stable and reliable, in case another implementation of the repository should be used
            _cityInfoRepository.UpdatePointOfInterestForCity(cityId, pointOfInterestEntity);

            _cityInfoRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            //check if the city exists
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }
           
            //check if the point of interest we want to delete exists
            var pointOfInterestEntity = _cityInfoRepository.
                GetPointOfInterestForCity(cityId, id);
            if(pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);

            _cityInfoRepository.Save();

            _mailService.Send("Point of interest deleted.", $"Point of interest {pointOfInterestEntity.Name} with id { pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
