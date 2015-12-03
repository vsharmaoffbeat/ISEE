using ISEEDataModel.Utilities;
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

        #region Admin Tree
        public IQueryable<TreeView> GetTreeViewRoot(int factoryId)
        {
            return _context.TreeViews.Where(x => x.FactoryID == factoryId && x.ParentID == null);
        }


        public List<TreeNodeData> CreateJsonTree(List<TreeView> data, string FactoryDesc)
        {
            List<TreeNodeData> treeList = new List<TreeNodeData>();
            if (data.Count == 0)
            {
                treeList.Add(new TreeNodeData() { id = -100, text = FactoryDesc, textCss = "customnode", objecttype = "companyNode" });
            }
            TreeNodeData parentTreeNode = new TreeNodeData();
            CreateTreeNodes(data, ref treeList, ref parentTreeNode, false);

            return treeList;
        }

        private void CreateTreeNodes(List<TreeView> data, ref List<TreeNodeData> treeList, ref TreeNodeData parentTreeNode, bool hasChildren = false)
        {
            TreeNodeData objTreeNodeData;
            foreach (var objTreeView in data)
            {
                if (objTreeView.EmployeeID != null)
                {
                    var emp = _context.Employees.Where(x => x.EmployeeId == objTreeView.EmployeeID).FirstOrDefault();
                    objTreeNodeData = new TreeNodeData() { id = objTreeView.ID, text = emp.LastName + " " + emp.FirstName, objectid = objTreeView.EmployeeID, textCss = "employeeTitle", objecttype = "employee", iconUrl = "/images/img/employee_16.png" };
                }
                else if (objTreeView.CustomerID != null)
                {
                    var cust = _context.Customers.Where(x => x.CustomerId == objTreeView.CustomerID).FirstOrDefault();
                    objTreeNodeData = new TreeNodeData() { id = objTreeView.ID, text = cust.LastName + " " + cust.FirstName, objectid = objTreeView.CustomerID, textCss = "customerTitle", objecttype = "customer", iconUrl = "/images/img/customer_16.png" };
                }
                else
                    objTreeNodeData = new TreeNodeData() { id = objTreeView.ID, text = objTreeView.Description, textCss = "customnode", objecttype = objTreeView.ParentID == null ? "companyNode" : "branchNode" };
                if (hasChildren)
                {
                    if (parentTreeNode.children == null)
                    {
                        parentTreeNode.children = new List<TreeNodeData>();
                    }
                    parentTreeNode.children.Add(objTreeNodeData);
                }
                else
                {
                    treeList.Add(objTreeNodeData);
                }

                if (objTreeView.TreeView1.Any())
                {
                    CreateTreeNodes(objTreeView.TreeView1.ToList(), ref  treeList, ref objTreeNodeData, true);
                }
            }
        }

        public void SaveTree(List<TreeNodeData> treeNodeList, int FactoryID)
        {
            DeleteNodes(treeNodeList, FactoryID);
            SaveProcess(treeNodeList, null, FactoryID);
            _context.SaveChanges();
        }

        private void DeleteNodes(List<TreeNodeData> treeNodeList, int factoryId)
        {
            List<long> _idsList = new List<long>();
            GetTreeIds(treeNodeList, ref _idsList);
            var deleteNodes = _context.TreeViews.Where(c => !_idsList.Contains(c.ID) && c.ID > 0 && c.FactoryID == factoryId).ToList();
            for (int i = deleteNodes.Count; i > 0; i--)
            {
                _context.TreeViews.Remove(deleteNodes[i - 1]);
            }
            _context.SaveChanges();
        }

        private void GetTreeIds(List<TreeNodeData> treeNodeList, ref  List<long> _idsList)
        {
            foreach (var item in treeNodeList)
            {
                _idsList.Add(item.id);
                if (item.children != null && item.children.Any())
                {
                    GetTreeIds(item.children, ref _idsList);
                }
            }
        }


        private void SaveProcess(List<TreeNodeData> treeNodeList, long? objParentNodeID, int? FactoryID)
        {
            TreeView objNode;
            foreach (var objTreeNode in treeNodeList)
            {
                objNode = _context.TreeViews.FirstOrDefault(c => c.ID == objTreeNode.id);
                if (objNode == null)
                {
                    objNode = new TreeView();
                    objNode.Description = objTreeNode.text;
                    objNode.FactoryID = FactoryID;
                    if (!string.IsNullOrEmpty(objTreeNode.objecttype) && objTreeNode.objecttype == "employee" && objTreeNode.objectid != null)
                        objNode.EmployeeID = objTreeNode.objectid;
                    else if (!string.IsNullOrEmpty(objTreeNode.objecttype) && objTreeNode.objecttype == "customer" && objTreeNode.objectid != null)
                        objNode.CustomerID = objTreeNode.objectid;

                    if (objParentNodeID.HasValue)
                    {
                        objNode.ParentID = objParentNodeID;
                    }
                    _context.TreeViews.Add(objNode);
                    _context.SaveChanges();
                }
                else
                {
                    objNode.Description = objTreeNode.text;
                }
                if (objTreeNode.children != null && objTreeNode.children.Any())
                {
                    SaveProcess(objTreeNode.children, objNode.ID, FactoryID);
                }
            }
        }

        #endregion

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
            return _context.EmployeeDiaryTemplates.Where(x => x.EmployeeId == empID).OrderBy(x => x.OrderDay);
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

        #region "SchedulerEmployees"
        /// <summary>
        /// Get EmployeeGpsPoint by SysID
        /// </summary>
        /// <param name="sysId"></param>
        /// <returns></returns>
        public EmployeeGpsPoint GetEmployeeGpsPointBySysId(int sysId)
        {
            return _context.EmployeeGpsPoints.FirstOrDefault(model => model.SysId == sysId);
        }


        /// <summary>
        ///  To get Employee Appointment where Customer not schduled
        /// </summary>
        /// <param name="factoryId">Factory ID</param>
        /// <param name="ID">Employee ID</param>
        /// <param name="dt1">Start Date</param>
        /// <param name="dt2">End Date</param>
        /// <param name="from_time">Start Time</param>
        /// <param name="to_time">End Time</param>
        /// <returns></returns>
        public IQueryable<EmployeeGpsPoint> GetStopPoints(int factoryId, int ID, DateTime dt1, DateTime dt2, TimeSpan from_time, TimeSpan to_time)
        {

            var factory = _context.FactoryParms.FirstOrDefault(x => x.FactoryId == factoryId);

            if (factory != null)
            {
                if (factory.StopEmployeeTime != null)
                {
                    var stopTime = (int)factory.StopEmployeeTime;

                    var query = from c in _context.EmployeeGpsPoints
                                where !(from o in _context.GpsEmployeeCustomers
                                        where o.EmployeeId == ID &&
                                              (o.EmployeeGpsPoint.GpsDate >= dt1 &&
                                               o.EmployeeGpsPoint.GpsDate <= dt2) &&
                                              (o.EmployeeGpsPoint.GpsTime >= from_time &&
                                               o.EmployeeGpsPoint.GpsTime <= to_time)
                                        select o.GpsPointId).Contains((int)c.SysId) && (c.GpsDate >= dt1 && c.GpsDate <= dt2) && (c.GpsTime >= from_time && c.GpsTime <= to_time) && c.EmployeeId == ID && (c.PointStatus == 2 || c.PointStatus == 3)
                                select c;



                    var q1 = from x in query.ToList()
                             where x.StopTime.HasValue && x.StopTime.Value.TotalMinutes >= stopTime
                             select x;


                    return q1.AsQueryable();
                }
            }
            return null;
        }

        /// <summary>
        /// Get Employee Appointments where customer assigned
        /// </summary>
        /// <param name="ID">Employee Id</param>
        /// <param name="dt1">Start Date</param>
        /// <param name="dt2">End Date</param>
        /// <param name="from1">Start Time</param>
        /// <param name="to1">End Time</param>
        /// <returns></returns>
        public IQueryable<GpsEmployeeCustomer> GetGpsEmployeeCustomerPoints(int ID, DateTime dt1, DateTime dt2, TimeSpan from1, TimeSpan to1)
        {
            return _context.GpsEmployeeCustomers.Where(c => c.EmployeeId == ID && (c.EmployeeGpsPoint.GpsDate >= dt1 && c.EmployeeGpsPoint.GpsDate <= dt2) && (c.EmployeeGpsPoint.GpsTime >= from1 && c.EmployeeGpsPoint.GpsTime <= to1));
        }


        public void SaveUpdateEvents(List<CalendarEvent> listEvents)
        {
            int gpsPointId;
            int objCustomerId;
            double Dist = 0;
            foreach (var objEvent in listEvents)
            {
                gpsPointId = Convert.ToInt32(objEvent.uniqueid);
                objCustomerId = Convert.ToInt32(objEvent.customerId);
                Dist = 0;

                var objEmployeeGpsPoint = _context.EmployeeGpsPoints.Where(x => x.SysId == gpsPointId).FirstOrDefault();
                GpsEmployeeCustomer item = _context.GpsEmployeeCustomers.Where(x => x.GpsPointId == gpsPointId).FirstOrDefault();
                if (objEmployeeGpsPoint != null)
                {
                    var myEventCustomer = _context.Customers.FirstOrDefault(c => c.CustomerId == objCustomerId);
                    //distination - Lat, Long from customer; Lat, Long from stop point
                    if (myEventCustomer.Building.Lat != null && myEventCustomer.Building.Long != null)
                        Dist = Utility.distance((double)myEventCustomer.Building.Lat, (double)myEventCustomer.Building.Long, (double)objEmployeeGpsPoint.Lat, (double)objEmployeeGpsPoint.Long, 'K');

                    Dist = Dist * 1000; //distanse -meters
                    if (item == null)
                    {


                        item = new GpsEmployeeCustomer();
                        item.CreateDate = DateTime.Now;
                        item.VisiteDate = objEmployeeGpsPoint.GpsDate;
                        item.VisitTime = (TimeSpan)objEmployeeGpsPoint.GpsTime;
                        item.GpsPointId = (int)gpsPointId;
                        item.CustomerId = objCustomerId;
                        item.EmployeeId = Convert.ToInt32(objEvent.employeeId);
                        //if distance > 250 status of point = 4 else 3.
                        item.InsertStatus = Dist > 250 ? 4 : 3;

                        _context.GpsEmployeeCustomers.Add(item);
                    }
                    else
                    {
                        item.InsertStatus = Dist > 250 ? 4 : 3;
                        item.CustomerId = objCustomerId;
                    }
                }
            }



            _context.SaveChanges();
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

        //Country tab start
        public IQueryable<Country> GetAllcountries()
        {
            return _context.Countries.OrderBy(x => x.CreateDate);
        }
        public void UpdateCountryDesc(int Countrycode, string CountryNameEN, string UTC, string CountryDesc)
        {
            var obj = _context.Countries.Where(c => c.CountryCode == Countrycode).FirstOrDefault();
            if (obj != null)
            {
                obj.CountryDesc = CountryDesc;
                obj.CountryDescEn = CountryNameEN;
                obj.CurrentGmt = float.Parse(UTC);
            }
            _context.SaveChanges();
        }
        public void SaveCountry(string CountryNameEN, string UTC, string CountryDesc)
        {
            Country objCountry = new Country();
            objCountry.CountryDesc = CountryDesc;
            objCountry.CountryDescEn = CountryNameEN;
            objCountry.CurrentGmt = float.Parse(UTC);
            objCountry.CreateDate = DateTime.Now.Date;
            _context.Countries.Add(objCountry);
            _context.SaveChanges();

        }
        public void UpdateStateDesc(int Countrycode, int StateCode, string StateDescEn, string StateDesc)
        {
            var obj = _context.States.Where(c => c.CountryCode == Countrycode && c.StateCode == StateCode).FirstOrDefault();
            if (obj != null)
            {
                obj.StateDesc = StateDesc;
                obj.StateDescEn = StateDescEn;
            }
            _context.SaveChanges();
        }
        public void SaveState(int Countrycode, int StateCode, string StateDescEn, string StateDesc)
        {
            State objState = new State();
            objState.StateDesc = StateDesc;
            objState.StateDescEn = StateDescEn;
            objState.CountryCode = Countrycode;
            objState.CreateDate = DateTime.Now.Date;
            objState.StatusCode = 0;
            _context.States.Add(objState);
            _context.SaveChanges();
        }
        public void UpdateCityDesc(int Countrycode, int StateCode, int CityCode, string CityDescEN, string CityDesc)
        {
            var obj = _context.Cities.Where(c => c.CountryCode == Countrycode && c.StateCode == StateCode && c.CityCode == CityCode).FirstOrDefault();
            if (obj != null)
            {
                obj.CityDesc = CityDesc;
                obj.CityDescEn = CityDescEN;
            }
            _context.SaveChanges();
        }

        public void SaveCity(int Countrycode, int StateCode, int CityCode, string CityDescEN, string CityDesc)
        {
            City objCity = new City();
            objCity.StateCode = StateCode;
            objCity.CityDesc = CityDesc;
            objCity.CityDescEn = CityDescEN;
            objCity.CountryCode = Countrycode;
            objCity.CreateDate = DateTime.Now.Date;
            objCity.StatusCode = 0;
            _context.Cities.Add(objCity);
            _context.SaveChanges();

        }


        public void UpdateStreetDesc(int Countrycode, int StateCode, int CityCode, int StreetCode, string StreetDescEN, string StreetDesc)
        {
            var obj = _context.Streets.Where(c => c.CountryCode == Countrycode && c.StateCode == StateCode && c.CityCode == CityCode && c.StreetCode == StreetCode).FirstOrDefault();
            if (obj != null)
            {
                obj.StreetDesc = StreetDesc;
                obj.StreetDescEn = StreetDescEN;
            }
            _context.SaveChanges();
        }

        public int SaveStreet(int Countrycode, int StateCode, int CityCode, int StreetCode, string StreetDescEN, string StreetDesc)
        {
            Street objStreet = new Street();
            objStreet.StreetDesc = StreetDesc;
            objStreet.StreetDescEn = StreetDescEN;
            objStreet.CountryCode = Countrycode;
            objStreet.CreatDate = DateTime.Now.Date;
            objStreet.StreetCode = StateCode;
            objStreet.CityCode = CityCode;
            objStreet.StatusCode = 0;
            _context.Streets.Add(objStreet);
            _context.SaveChanges();

            return objStreet.StreetCode;
        }

        //country tab end










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



        // Methods for map tab
        public IQueryable<Employee> SearchEmploeesForMap(int factoryId, string _LN, string _FN, string _Num, bool _Active)
        {
            return _context.Employees.Where(x => x.Factory == factoryId
                                                          && (string.IsNullOrEmpty(_FN) || x.FirstName.Contains(_FN))
                                                          && (string.IsNullOrEmpty(_LN) || x.LastName.Contains(_LN))
                                                          && (string.IsNullOrEmpty(_Num) || x.EmployeeNum.Contains(_Num))
                                                          && (_Active == true ? (x.EndDay == null || (x.EndDay != null && x.EndDay >= DateTime.Now)) : (x.EndDay != null && x.EndDay < DateTime.Now))).OrderBy(x => x.EmployeeNum);

        }
        public IQueryable<EmployeeGpsPoint> GetRunWayForEmploees(int employeeID, TimeSpan _From, TimeSpan _To, DateTime date)
        {
            return _context.EmployeeGpsPoints.Where(s => s.EmployeeId == employeeID
                && s.GpsDate == date
                && s.GpsTime >= _From
                && s.GpsTime <= _To
           );
        }
        public IQueryable<EmployeeGpsPoint> GetStopPointForEmploees(int employeeID, TimeSpan _From, TimeSpan _To, DateTime date)
        {
            return _context.EmployeeGpsPoints.Where(s => s.EmployeeId == employeeID
                && s.GpsDate == date
                && s.GpsTime >= _From
                && s.GpsTime <= _To
                && s.StopTime != null
           );
        }
        public IQueryable<EmployeeGpsPoint> GetLastPointForEmploees(int employeeID, TimeSpan _From, TimeSpan _To, DateTime date)
        {
            return _context.EmployeeGpsPoints.Where(s => s.EmployeeId == employeeID
                       && s.GpsDate == date
                       && s.GpsTime >= _From
                       && s.GpsTime <= _To
                       && s.StopTime != null
                   ).OrderByDescending(x => x.SysId);
        }
        public IQueryable<Customer> GetCustomersByCustomerID(string checkedcustomers)
        {
            var numbers = checkedcustomers.TrimEnd(',').Split(',').Select(Int32.Parse).ToList();
            return _context.Customers.Where(s => numbers.Contains(s.CustomerId));
        }

        public IQueryable<Customer> GetAllCustomers(int factoryId)
        {
            return _context.Customers.Where(s => s.Factory == factoryId);
        }
        public IQueryable<Employee> GetEmployeeById(int employeeID, int factoryId)
        {
            return _context.Employees.Where(x => x.Factory == factoryId
                      && x.EmployeeId == employeeID);
        }

        #endregion

    }
}
