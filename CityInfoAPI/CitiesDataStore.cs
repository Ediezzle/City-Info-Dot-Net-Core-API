using CityInfoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI
{
    public class CitiesDataStore
    {
        //for easy access and so we can work on the same instance as long as the server hasn't been restarted
        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public List<CityDto> Cities { get; set; }

        //init dummy data
        public CitiesDataStore()
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "Cape Town",
                    Description = "The ultimate tourists destination",
                    PointsOfInterest = new List<PointOfInterestDto>()
                     {
                         new PointOfInterestDto() 
                         {
                             Id = 1,
                             Name = "Table Mountain",
                             Description = "a flat-topped mountain forming a prominent landmark overlooking the city of Cape Town in South Africa." 
                         },
                          new PointOfInterestDto() 
                          {
                             Id = 2,
                             Name = "Robben Island",
                             Description = "an island in Table Bay, 6.9 kilometres west of the coast of Bloubergstrand, north of Cape Town, South Africa." 
                          },
                     }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Johannesburg",
                    Description = "Home of the tsotsis",
                    PointsOfInterest = new List<PointOfInterestDto>()
                     {
                         new PointOfInterestDto() 
                         {
                             Id = 3,
                             Name = "Apartheid Museum",
                             Description = "Illustrates apartheid and the 20th century history of South Africa." 
                         },
                          new PointOfInterestDto() 
                          {
                             Id = 4,
                             Name = "Gold Reef City",
                             Description = "an authentic portrayal of a turn-of-the-century mining town hotel located within Africa's largest theme park." 
                          },
                     }
                },
                new CityDto()
                {
                    Id= 3,
                    Name = "Paris",
                    Description = "The one with that big tower.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                     {
                         new PointOfInterestDto() 
                         {
                             Id = 5,
                             Name = "Eiffel Tower",
                             Description = "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel." 
                         },
                          new PointOfInterestDto() 
                          {
                             Id = 6,
                             Name = "The Louvre",
                             Description = "The world's largest museum." 
                          },
                     }
                }
            };
            
        }
    }
}
