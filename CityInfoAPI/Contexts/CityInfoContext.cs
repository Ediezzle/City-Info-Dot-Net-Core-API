using CityInfoAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfoAPI.Contexts
{
    public class CityInfoContext : DbContext
    {
        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasData(
                new City()
                {
                    Id = 1,
                    Name = "Cape Town",
                    Description = "The ultimate tourists destination"
                },
                new City()
                {
                    Id = 2,
                    Name = "Johannesburg",
                    Description = "Home of the tsotsis"
                },
                new City()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with that big tower."
                }
                );

            modelBuilder.Entity<PointOfInterest>()
                .HasData(
                new PointOfInterest()
                {
                    Id = 1,
                    CityId = 1,
                    Name = "Table Mountain",
                    Description = "a flat-topped mountain forming a prominent landmark overlooking the city of Cape Town in South Africa."
                },
                new PointOfInterest()
                {
                    Id = 2,
                    CityId = 1,
                    Name = "Robben Island",
                    Description = "an island in Table Bay, 6.9 kilometres west of the coast of Bloubergstrand, north of Cape Town, South Africa."
                },
                new PointOfInterest()
                {
                    Id = 3,
                    CityId = 2,
                    Name = "Apartheid Museum",
                    Description = "Illustrates apartheid and the 20th century history of South Africa."
                },
                new PointOfInterest()
                {
                    Id = 4,
                    CityId = 2,
                    Name = "Gold Reef City",
                    Description = "an authentic portrayal of a turn-of-the-century mining town hotel located within Africa's largest theme park."
                },
                new PointOfInterest()
                {
                    Id = 5,
                    CityId = 3,
                    Name = "Eiffel Tower",
                    Description = "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel."
                },
                new PointOfInterest()
                {
                    Id = 6,
                    CityId = 3,
                    Name = "The Louvre",
                    Description = "The world's largest museum."
                }
                );

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<City> Cities { get; set; }

        public DbSet<PointOfInterest> PointsOfInterest { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("connectionstring");
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
