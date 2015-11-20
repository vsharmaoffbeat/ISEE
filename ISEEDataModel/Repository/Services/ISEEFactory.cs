using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISEEDataModel.Repository.Services
{

    public class ISEEFactory
    {
        private static ISEEEntities _context;

        public ISEEFactory()
        {
            _context = new ISEEEntities();
        }

        public IQueryable<State> GetAllStates(int county)
        {
            return _context.States.Where(x => x.CountryCode == county).OrderBy(x => x.StateDesc);
        }

        public IQueryable<City> GetAllCities(int county, int state)
        {
            return _context.Cities.Where(x => x.CountryCode == county && x.StateCode == state).OrderBy(x => x.CityDesc);
        }

        public IQueryable<City> GetAllCitys(int county)
        {
            return _context.Cities.Where(x => x.CountryCode == county).OrderBy(x => x.CityDesc);
        }

        public IQueryable<Street> GetAllStreets(int county, int city)
        {
            return _context.Streets.Where(x => x.CountryCode == county && x.CityCode == city).OrderBy(x => x.StreetDesc);
        }

        public IQueryable<Building> GetAllNumbers(int county, int city, int street)
        {
            return _context.Buildings.Where(x => x.CountryCode == county && x.CityCode == city && x.StreetCode == street);
        }






    }
}
