using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Net;
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


        #region "Employees"


        public IQueryable<Employee> GetEmployees(int factoryId, string _LN, string _FN, string _Num, int _Manuf, int _Phonetype, bool _Active)
        {
            return _context.Employees.Where(x => x.Factory == factoryId
                                                         && x.FirstName.Contains(_FN == null ? x.FirstName : _FN)
                                                         && (string.IsNullOrEmpty(_LN) || x.LastName.Contains(_LN))
                                                         && x.EmployeeNum.Contains(_Num == null ? x.EmployeeNum : _Num)
                                                         && x.PhoneManufactory == (_Manuf == 0 ? x.PhoneManufactory : _Manuf)
                                                         && x.PhoneType == (_Phonetype == 0 ? x.PhoneType : _Phonetype)
                                                         && (_Active == true ? (x.EndDay == null || (x.EndDay != null && x.EndDay >= DateTime.Now)) : (x.EndDay != null && x.EndDay < DateTime.Now))).OrderBy(x => x.EmployeeNum);


        }

        public IQueryable<EmployeeSmsSend> GetSMS(int empID)
        {
            return _context.EmployeeSmsSends.Where(x => x.EmployeeId == empID).OrderByDescending(x => x.SmsCreatDate);
        }
        public IQueryable<EmployeeSmsSend> GetSMSFilter(int empID, DateTime dtFrom, DateTime dtTo)
        {
            return _context.EmployeeSmsSends.Where(x => x.EmployeeId == empID &&
                                                          x.SmsCreatDate >= dtFrom &&
                                                          x.SmsCreatDate <= dtTo).OrderByDescending(x => x.SmsCreatDate);
        }
        public IQueryable<EmployeeDiaryTemplate> GetEmpDiaryTemplate(int empID)
        {
            return _context.EmployeeDiaryTemplate.Where(x => x.EmployeeId == empID).OrderBy(x => x.OrderDay);
        }

        public IQueryable<FactoryDairyTemplet> GetFactoryDairyTemp(int factoryId)
        {
            return _context.FactoryDairyTemplets.Where(x => x.Factory == factoryId).OrderBy(x => x.OrderDay);
        }


        public IQueryable<EmployeeDiaryTime> GetEmployeeDiaryMonth(int empID, int _month, int _year)
        {
            return _context.EmployeeDiaryTimes.Where(x => x.EmployeeId == empID &&
                                                              x.Day.Year == _year &&
                                                              x.Day.Month == _month).OrderBy(x => x.Day);
        }




        //public IQueryable<EmployeeCalendar> GetEmployeeCalendar(Guid EmpGuidID, int year, int month)
        //{

        //    var empID = this.ObjectContext.Employee.FirstOrDefault(x => x.EmployeeKey == EmpGuidID).EmployeeId;
        //    int i = 0;
        //    var q = Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc. 
        //            .Select(day => new EmployeeCalendar
        //            {
        //                id = ++i,
        //                CurrentDate = new DateTime(year, month, day),
        //                StatusDay = (int)(new DateTime(year, month, day)).DayOfWeek + 1,
        //                HolidayDay = 1,
        //                PresentDayStart1 = null,
        //                PresentDayStop1 = null,
        //                PresentDayStart2 = null,
        //                PresentDayStop2 = null,
        //                PresentDayStart3 = null,
        //                PresentDayStop3 = null,
        //            });

        //    /* group by day and get only first row */
        //    var groupDay =
        //        from gr in
        //            ObjectContext.EmployeeDiaryTime.Where(
        //                x => x.EmployeeId == empID && (x.Day.Year == year && x.Day.Month == month))
        //        group gr by gr.Day
        //            into grp
        //            select grp.FirstOrDefault();

        //    /* left join*/
        //    var query = from f in q
        //                join fp in groupDay
        //                on f.CurrentDate equals fp.Day
        //                into JoinedEmpPresence
        //                from fp in JoinedEmpPresence.DefaultIfEmpty()
        //                select new EmployeeCalendar
        //                {
        //                    id = f.id,
        //                    CurrentDate = f.CurrentDate,
        //                    StatusDay = f.StatusDay,
        //                    HolidayDay = f.HolidayDay,
        //                    PresentDayStart1 = fp != null ? fp.Start1 : null,
        //                    PresentDayStop1 = fp != null ? fp.Stop1 : null,
        //                    PresentDayStart2 = fp != null ? fp.Start2 : null,
        //                    PresentDayStop2 = fp != null ? fp.Stop2 : null,
        //                    PresentDayStart3 = fp != null ? fp.Start3 : null,
        //                    PresentDayStop3 = fp != null ? fp.Stop3 : null,
        //                };



        //    return query.AsQueryable();

        //}



        static string PostDataToURL(string szUrl, string szData)
        {
            //Setup web request
            string szResult = string.Empty;
            WebRequest Request = WebRequest.Create(szUrl);
            Request.Timeout = 3000;
            Request.Method = "POST";
            Request.ContentType = "application/x-www-form-urlencoded";

            //Set the POST data in a buffer
            byte[] PostBuffer;
            try
            {
                //replacing " " with "+" according to Http post RPC
                szData = szData.Replace(" ", "+");

                //Specify the length of the buffer
                PostBuffer = Encoding.UTF8.GetBytes(szData);
                Request.ContentLength = PostBuffer.Length;

                //Open up a request stream
                Stream RequestStream = Request.GetRequestStream();

                //Write the POST data
                RequestStream.Write(PostBuffer, 0, PostBuffer.Length);

                //Close the stream
                RequestStream.Close();
                //Create the response object
                WebResponse Response;
                Response = Request.GetResponse();

                //Create the reader for the response
                StreamReader sr = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);

                //Read the response
                szResult = sr.ReadToEnd();

                //Close the reader and response
                sr.Close();
                Response.Close();

                return szResult;
            }
            catch (Exception ex)
            {
                return szResult;
            }
        }

        public IQueryable<GpsEmployeeCustomer> GetCustomerToEmployees(int empID, int _Year, int _Month)
        {
            return _context.GpsEmployeeCustomers.Where(x => x.EmployeeId == empID && x.VisiteDate.Year == _Year && x.VisiteDate.Month == _Month).OrderBy(x => x.CreateDate);

        }

        public IQueryable<EmployeeGpsPoint> GetSchedulerEmployee(int empID, DateTime _From, DateTime _To)
        {
            return _context.EmployeeGpsPoints.Where(x => x.EmployeeId == empID && x.GpsDate >= _From.Date && x.GpsDate < _To.Date && x.PointStatus == 2);
        }

        public IQueryable<EmployeeToGroup> GetEmployeeToGroups(int empID)
        {
            return _context.EmployeeToGroups.Where(x => x.EmployeeId == empID && x.Status != -1);
        }

        public IQueryable<EmployeeGroup> GetGroups(int factoryId, int empID)
        {
            var query = from c in _context.EmployeeGroups
                        where !(from o in _context.EmployeeToGroups
                                where o.EmployeeId == empID && o.Status != -1
                                select o.EmployeeGroupId).Contains(c.EmployeeGroupId) && c.FactoryId == factoryId
                        select c;
            return query.AsQueryable();


        }

        public IQueryable<EmployeeGroup> GetAllGroups(int factoryId)
        {
            return _context.EmployeeGroups.Where(x => x.FactoryId == factoryId);
        }

        public IQueryable<CustomerEmployeeContact> GetCustomerContact(int empID)
        {
            return _context.CustomerEmployeeContacts.Where(x => x.EmployeeId == empID).OrderBy(x => x.CreateDate);

        }


        #endregion

        #region "Customers"


        public int GetChangeBuildingCodeNew(int country, int state, int city, int street, string number)
        {
            var q = _context.Buildings.Where(x => x.CountryCode == country && x.StateCode == state && x.CityCode == city && x.StreetCode == street && x.Number.CompareTo(number == null ? x.Number : number) == 0);
            if (q.Any())
            {
                var firstOrDefault = q.FirstOrDefault();
                if (firstOrDefault != null) return firstOrDefault.BuildingCode;
            }

            return -1;
        }


        public Building GetNewBuildingCode(int country, int state, int city, int street, string number)
        {
            return _context.Buildings.FirstOrDefault(x => x.CountryCode == country &&
                                                                    x.StateCode == state &&
                                                                    x.CityCode == city &&
                                                                    x.StreetCode == street &&
                                                                    x.Number.CompareTo(number == null ? x.Number : number) == 0);

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

        public int GetChangeBuildingCode(int _Country, int _City, int _Street, string _Number, string _Entry, string _ZipCode)
        {
            int BuildingCode = 0;

            var q = _context.Buildings.Where(x => x.CountryCode == _Country && x.CityCode == _City && x.StreetCode == _Street && x.Number.CompareTo(_Number == null ? x.Number : _Number) == 0).ToList();

            //get BuildingCode for City,Street,Number 
            if (q != null && q.ToList().Count > 0)
            {
                foreach (Building e in q)
                    if (Equals(e.Entry, _Entry) && Equals(e.ZipCode, _ZipCode))
                    //  if ( e.Entry.CompareTo(_Entry) == 0 && e.ZipCode.CompareTo(_ZipCode) == 0)
                    {
                        BuildingCode = e.BuildingCode;
                        if (e.Lat == null || e.Long == null)
                        {
                            e.StatusCode = 1;
                            _context.SaveChanges();

                        }
                        return BuildingCode;
                    }

                if (BuildingCode == 0) BuildingCode = q.FirstOrDefault().BuildingCode;
            }

             // add new row
            else if (q != null && q.ToList().Count == 0)
            {
                Building b = new Building();
                b.CreateDate = DateTime.Now;
                b.StatusCode = 1;
                b.CountryCode = _Country;
                b.StateCode = 0;
                b.CityCode = _City;
                b.StreetCode = _Street;
                b.Entry = _Entry;
                b.ZipCode = _ZipCode;
                b.Lat = null;
                b.Long = null;
                b.BuildingComment = null;
                b.Number = _Number;

                try
                {
                    _context.Buildings.Add(b);
                    _context.SaveChanges();
                    BuildingCode = b.BuildingCode;
                }
                catch (Exception)
                {
                    //throw;
                }




            }

            return BuildingCode;
        }

        public IQueryable<Customer> GetCurrentCustomer(int factoryId, int customerID)
        {
            return _context.Customers.Where(x => x.Factory == factoryId && x.CustomerId == customerID);
        }

        public IQueryable<Customer> GetCustomersNew(int factoryId, int _state, int _city, int _street, string _num, string _cusnum, string _FN, string _LN, string _area, string _phone, bool _Active)
        {
            return _context.Customers.Where(x => x.Factory == factoryId
                //  (_state != 0 ? x.Building.StateCode == _state : x.Building.StateCode == null) &&
             && x.Building.StateCode == (_state == 0 ? x.Building.StateCode : _state) && x.Building.CityCode == (_city == 0 ? x.Building.CityCode : _city)
             && x.Building.StreetCode == (_street == 0 ? x.Building.StreetCode : _street)
             && x.Building.Number.Contains(string.IsNullOrEmpty(_num) ? x.Building.Number : _num)
             && x.CustomerNumber.CompareTo(string.IsNullOrEmpty(_cusnum) ? x.CustomerNumber : _cusnum) == 0
             && x.FirstName.Contains(string.IsNullOrEmpty(_FN) ? x.FirstName : _FN)
             && (string.IsNullOrEmpty(x.LastName) || x.LastName.Contains(string.IsNullOrEmpty(_LN) ? x.LastName : _LN))
             && (string.IsNullOrEmpty(x.AreaPhone1) || x.AreaPhone1.Contains(string.IsNullOrEmpty(_area) ? x.AreaPhone1 : _area))
             && (string.IsNullOrEmpty(x.Phone1) || x.Phone1.Contains(string.IsNullOrEmpty(_phone) ? x.Phone1 : _phone))
             && (_Active ? (x.EndDate == null || (x.EndDate != null && x.EndDate >= DateTime.Now)) : (x.EndDate != null && x.EndDate < DateTime.Now)));
            //  (_Active == true ? x.EndDate == null : x.EndDate !=null));


        }

        public IQueryable<Customer> GetCustomers(int factoryId, int _city, int _street, string _num, string _cusnum, string _FN, string _LN, string _area, string _phone)
        {
            return _context.Customers.Where(x => x.Factory == factoryId
                && x.Building.CityCode == (_city == 0 ? x.Building.CityCode : _city)
                && x.Building.StreetCode == (_street == 0 ? x.Building.StreetCode : _street)
                && x.Building.Number.Contains(string.IsNullOrEmpty(_num) ? x.Building.Number : _num)
                && x.CustomerNumber.CompareTo(string.IsNullOrEmpty(_cusnum) ? x.CustomerNumber : _cusnum) == 0
                && x.FirstName.Contains(string.IsNullOrEmpty(_FN) ? x.FirstName : _FN)
                && (x.LastName == null || x.LastName.Contains(string.IsNullOrEmpty(_LN) ? x.LastName : _LN))
                && (x.AreaPhone1 == null || x.AreaPhone1.Contains(string.IsNullOrEmpty(_area) ? x.AreaPhone1 : _area))
                && (x.Phone1 == null || x.Phone1.Contains(string.IsNullOrEmpty(_phone) ? x.Phone1 : _phone)));
            //.OrderBy(x => x.CustomerNumber); //&&

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

        public IQueryable<CustomerRequest> GetRequestCustomer(int customerID, int l1)
        {
            return _context.CustomerRequests.Where(x => x.CustomerId == customerID
                && x.RequsetToFactoryLevel2.RequestSysIdLevel1 == (l1 == -1 ? x.RequsetToFactoryLevel2.RequestSysIdLevel1 : l1)).OrderByDescending(x => x.CreateDate);

        }

        public IQueryable<CustomerRequest> GetRequestCustomerById(int id)
        {
            return _context.CustomerRequests.Where(x => x.SysId == id);
        }

        public IQueryable<CustomerRequest> GetRequestCustomerByDate(int customerID, int fromyear, int frommonth, int fromday, int toyear, int tomonth, int today, int level1, int level2)
        {
            var fromdate = new DateTime(fromyear, frommonth, fromday);
            var todate = new DateTime(toyear, tomonth, today).AddDays(1);
            return _context.CustomerRequests.Where(x => x.CustomerId == customerID
                && x.RequsetToFactoryLevel2.RequestSysIdLevel1 == (level1 == -1 ? x.RequsetToFactoryLevel2.RequestSysIdLevel1 : level1)
                && x.RequestSysIdLevel2 == (level2 == -1 ? x.RequestSysIdLevel2 : level2)
                && x.CreateDate >= fromdate && x.CreateDate < todate).OrderByDescending(x => x.CreateDate);

        }

        public IQueryable<RequsetToFactoryLevel1> GetRequsetLevel1(int factoryId)
        {
            return _context.RequsetToFactoryLevel1.Where(x => x.Factory == factoryId).OrderBy(x => x.RequsetOrder);
        }

        public IQueryable<GpsEmployeeCustomer> GetEmployeesToCustomer(int customerID)
        {
            return _context.GpsEmployeeCustomers.Where(x => x.CustomerId == customerID).OrderBy(x => x.CreateDate);
        }
        public IQueryable<GpsEmployeeCustomer> GetEmployeesToCustomerFilter(int customerID, DateTime dtFrom, DateTime dtTo)
        {
            return _context.GpsEmployeeCustomers.Where(
                                                         x => x.CustomerId == customerID &&
                                                         x.VisiteDate >= dtFrom &&
                                                         x.VisiteDate <= dtTo)
                                                         .OrderByDescending(x => x.VisiteDate).ThenByDescending(y => y.VisitTime);
        }


        public IQueryable<clsEmployeeCustomerContact> GetEmployeeContact(int customerID)
        {

            // return this.ObjectContext.CustomerEmployeeContact.Include("Employee").Where(x => x.CustomerId == cusID).OrderBy(x => x.CreateDate);
            var query = from c in _context.CustomerEmployeeContacts
                        where c.CustomerId == customerID
                        select new clsEmployeeCustomerContact
                        {
                            CreateDate = c.CreateDate,
                            LastName = c == null ? String.Empty : c.Employee.LastName,
                            FirstName = c == null ? String.Empty : c.Employee.FirstName,
                            EmpNumber = c == null ? "-1" : c.Employee.EmployeeNum,
                            EmpID = c == null ? -1 : c.Employee.EmployeeId
                        };

            var q1 = query.ToList();

            int i = 0;
            q1.ForEach(x => x.ID = ++i);
            return q1.AsQueryable();
        }

        #endregion










        #region "Map"

        public IQueryable<EmployeeGroup> GetAllGroupsMap(int factoryId)
        {
            return _context.EmployeeGroups.Where(x => x.FactoryId == factoryId);
        }

        public IQueryable<Employee> GetAllEmployee(int factoryId)
        {
            return _context.Employees.Where(x => x.Factory == factoryId);
        }

        public string TestTimeMap(int[] keywords, DateTime dt, TimeSpan from, TimeSpan to, int status)
        {

            return "2   " + dt.Date.ToShortDateString() + " : " + from.ToString() + " : " + to.ToString();
        }

        public IQueryable<EmployeeGpsPoint> GetEmployeesGPSStatusItay(Guid[] keywords, DateTime dt, int Year, int Month, int Day, TimeSpan from, TimeSpan to, int status)
        {
            DateTime dtFilter = new DateTime(Year, Month, Day);

            TimeSpan frompar = new TimeSpan(from.Hours, from.Minutes, from.Seconds);
            TimeSpan topar = new TimeSpan(to.Hours, to.Minutes, to.Seconds);

            var q = from p in _context.Employees
                    where keywords.Contains(p.EmployeeKey.Value)
                    select p.EmployeeId;

            int[] keysid = new int[q.ToList().Count];
            int i = 0;
            q.ToList().ForEach(x =>
            {
                keysid[i] = x;
                ++i;
            });

            var empList = (from emp in _context.EmployeeGpsPoints
                           where
                              keysid.Contains(emp.EmployeeId.Value) &&
                               emp.GpsDate == dtFilter &&
                               (emp.GpsTime.HasValue && frompar <= emp.GpsTime.Value) &&
                               (emp.GpsTime.HasValue && topar >= emp.GpsTime.Value) &&
                                emp.Lat != 0 && emp.Long != 0 &&
                               (status != -1 || (emp.PointStatus == 2 || emp.PointStatus == 3))
                           select emp)
                    .OrderBy(x => x.EmployeeId);


            return empList.AsQueryable();
        }


        public IQueryable<EmployeeGpsPoint> GetEmployeesGPSStatus(Guid[] keywords, DateTime dt, TimeSpan from, TimeSpan to, int status)
        {

            var predicate = PredicateBuilder.False<EmployeeGpsPoint>();

            var q = from p in _context.Employees
                    where keywords.Contains(p.EmployeeKey.Value)
                    select p.EmployeeId;

            int[] keysid = new int[q.ToList().Count];
            int i = 0;
            q.ToList().ForEach(x =>
            {
                keysid[i] = x;
                ++i;
            });

            foreach (int keyword in keysid)
            {
                int temp = keyword;
                predicate = predicate.Or(p => p.EmployeeId == temp);
            }
            DateTime par = new DateTime(dt.Year, dt.Month, dt.Day);
            predicate = predicate.And(x => x.GpsDate == par);

            TimeSpan frompar = new TimeSpan(from.Hours, from.Minutes, from.Seconds);
            predicate = predicate.And(y => y.GpsTime >= frompar);

            TimeSpan topar = new TimeSpan(to.Hours, to.Minutes, to.Seconds);
            predicate = predicate.And(y => y.GpsTime <= topar);

            predicate = predicate.And(y => !y.Lat.Equals(0));
            predicate = predicate.And(y => !y.Long.Equals(0));

            // if status=-1 get points stops
            if (status == -1)
                predicate = predicate.And(p => p.PointStatus == 2 || p.PointStatus == 3);


            //List<EmployeeGpsPoints> Data = this.ObjectContext.EmployeeGpsPoints.ToList();
            //var empList = from emp in Data.AsQueryable().Where(predicate).OrderBy(x => x.EmployeeId) select emp;
            var empList = _context.EmployeeGpsPoints.ToList().AsQueryable().Where(e => e.Lat != 0 && e.Long != 0).OrderBy(x => x.EmployeeId);
            // var list = empList.ToList();
            return empList.AsQueryable();



        }

        public IQueryable<EmployeeGpsPoint> GetEmployeesGPSLastPointStatus(Guid[] keywords, DateTime dt, int Year, int Month, int Day, TimeSpan from, TimeSpan to)
        {

            var q = from p in _context.Employees
                    where keywords.Contains(p.EmployeeKey.Value)
                    select p.EmployeeId;

            int[] keysid = new int[q.ToList().Count];
            int i = 0;
            q.ToList().ForEach(x =>
            {
                keysid[i] = x;
                ++i;
            });


            var dtFilter = new DateTime(Year, Month, Day);

            var frompar = new TimeSpan(from.Hours, from.Minutes, from.Seconds);
            var topar = new TimeSpan(to.Hours, to.Minutes, to.Seconds);
            var empList = (from emp in _context.EmployeeGpsPoints
                           where
                              keysid.Contains(emp.EmployeeId.Value) &&
                                 emp.GpsDate == dtFilter &&
                                 (emp.GpsTime != null ? frompar <= emp.GpsTime.Value : false) &&
                                 (emp.GpsTime != null ? topar >= emp.GpsTime.Value : false) &&
                                 emp.Lat != 0 && emp.Long != 0

                           select emp);


            // empList.GroupBy(x=>x.EmployeeId).GroupBy()
            var empList1 = from p in empList
                           group p by p.EmployeeId into grp
                           select grp.OrderByDescending(g => g.GpsTime).FirstOrDefault();

            return empList1.AsQueryable();

        }

        private double GetDistance(double pointLat, double pointLong, double centerRegionLat, double centerRegionLong)
        {
            var sCoord = new GeoCoordinate(pointLat, pointLong);
            var eCoord = new GeoCoordinate(centerRegionLat, centerRegionLong);
            return sCoord.GetDistanceTo(eCoord);

        }


        #endregion

    }
}
