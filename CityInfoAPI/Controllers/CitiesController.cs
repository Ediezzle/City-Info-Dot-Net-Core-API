using AutoMapper;
using CityInfoAPI.Models;
using CityInfoAPI.Services;
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
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet] 
        public IActionResult GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();

            var results = new List<CityWithoutPointsOfInterestDto>();

            //manual mapping
            //foreach(var cityEntity in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto
            //    {
            //        Id = cityEntity.Id,
            //        Description = cityEntity.Description,
            //        Name = cityEntity.Name
            //    });
            //}
            //return Ok(results);

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));

            //return new JsonResult(CitiesDataStore.Current.Cities);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false)
        {
            //var requestedCity = CitiesDataStore.Current.Cities.FirstOrDefault(c=>c.Id == id);
            //if(requestedCity == null)
            //{
            //    return NotFound();
            //}

            //return Ok(requestedCity);

            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);
            if(city == null)
            {
                return NotFound();
            }
            if(includePointsOfInterest)
            {
                var cityResult = _mapper.Map<CityDto>(city);
                return Ok(cityResult);
            }

            //manual mapping
            //var cityWithoutPointsOfInterest =
            //    new CityWithoutPointsOfInterestDto()
            //    {
            //        Id = city.Id,
            //        Description = city.Description,
            //        Name = city.Name
            //    };
            //return Ok(cityWithoutPointsOfInterest);

            //using automapper
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(city));
        }
    }
}
