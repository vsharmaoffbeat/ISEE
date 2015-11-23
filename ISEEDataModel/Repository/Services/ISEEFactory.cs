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

        public int GetAddressBuildingCode(int country, int state, string citydesc, int city, int street, string streetdesc, string number, double Lat, double Long, string entry, string zipcode)
        {
            int citycode = 0;
            int buildingcode = 0;

            try
            {
                IQueryable<City> q1;

                if (city == 0)
                    q1 = _context.Cities.Where(x => x.CountryCode == country && x.StateCode == state && x.CityDesc == citydesc);
                else
                    q1 = _context.Cities.Where(x => x.CountryCode == country && x.StateCode == state && x.CityCode == city);

                if (q1.Any())
                {
                    var q2 = _context.Streets.Where(x => x.CountryCode == country && x.StateCode == state && x.City.CityCode == q1.FirstOrDefault().CityCode && x.StreetDesc == streetdesc);
                    if (q2.Any())
                    {

                        //city,street exists 
                        var b1 = new Building
                        {
                            CreateDate = DateTime.Now,
                            StatusCode = 2,
                            CountryCode = country,
                            StateCode = state,
                            CityCode = q2.FirstOrDefault().CityCode,
                            StreetCode = q2.FirstOrDefault().StreetCode,
                            Entry = entry,
                            ZipCode = zipcode,
                            Lat = Lat,
                            Long = Long,
                            BuildingComment = null,
                            Number = number.ToString().Trim()
                        };
                        _context.Buildings.Add(b1);
                        _context.SaveChanges();
                        return b1.BuildingCode;


                    }
                    else
                    {
                        //city exist street not
                        var streetnew = new Street
                        {
                            CountryCode = country,
                            StatusCode = 2,
                            StateCode = state,
                            CityCode = q1.FirstOrDefault().CityCode,
                            CreatDate = DateTime.Now,
                            StreetDesc = streetdesc
                        };
                        _context.Streets.Add(streetnew);
                        _context.SaveChanges();

                        var b2 = new Building
                        {
                            CreateDate = DateTime.Now,
                            StatusCode = 2,
                            CountryCode = country,
                            StateCode = state,
                            CityCode = streetnew.CityCode,
                            StreetCode = streetnew.StreetCode,
                            Entry = entry,
                            ZipCode = zipcode,
                            Lat = Lat,
                            Long = Long,
                            BuildingComment = null,
                            Number = number.ToString().Trim()
                        };
                        _context.Buildings.Add(b2);
                        _context.SaveChanges();
                        return b2.BuildingCode;

                    }

                }
                else
                {//city not exist 
                    var city1 = new City
                    {
                        CreateDate = DateTime.Now,
                        StatusCode = 2,
                        CountryCode = country,
                        StateCode = state,
                        CityDesc = citydesc
                    };

                    var street1 = new Street
                    {
                        StatusCode = 2,
                        CountryCode = country,
                        StateCode = state,
                        CreatDate = DateTime.Now,
                        StreetDesc = streetdesc
                    };

                    city1.Streets.Add(street1);
                    _context.Cities.Add(city1);
                    _context.SaveChanges();
                    var b3 = new Building
                    {
                        CreateDate = DateTime.Now,
                        StatusCode = 2,
                        CountryCode = country,
                        StateCode = state,
                        CityCode = city1.CityCode,
                        StreetCode = street1.StreetCode,
                        Entry = entry,
                        ZipCode = zipcode,
                        Lat = Lat,
                        Long = Long,
                        BuildingComment = null,
                        Number = number.ToString().Trim()
                    };
                    _context.Buildings.Add(b3);
                    _context.SaveChanges();
                    return b3.BuildingCode;

                }
            }
            catch (Exception)
            {

                return -1;
            }

        }

        public Building GetChangeBuildingCode1(int _Country, int State, int _City, int _Street, string _Number, string _Entry, string _ZipCode)
        {
            var q = _context.Buildings.Where(x => x.CountryCode == _Country && x.StateCode == State && x.CityCode == _City && x.StreetCode == _Street && x.Number.CompareTo(_Number == null ? x.Number : _Number) == 0).ToList();
            //get BuildingCode for City,Street,Number 
            if (q.Any())
            {
                foreach (Building e in q)
                    if (Equals(e.Entry, _Entry) && Equals(e.ZipCode, _ZipCode))
                    {

                        return e;
                    }

                Building b = new Building
                {
                    CreateDate = DateTime.Now,
                    StatusCode = 1,
                    CountryCode = _Country,
                    StateCode = State,
                    CityCode = _City,
                    StreetCode = _Street,
                    Entry = _Entry,
                    ZipCode = _ZipCode,
                    Lat = q.FirstOrDefault().Lat,
                    Long = q.FirstOrDefault().Long,
                    BuildingComment = null,
                    Number = _Number.ToString().Trim()
                };

                _context.Buildings.Add(b);
                _context.SaveChanges();

                return b;
            }
            return null;
        }
    }
}
