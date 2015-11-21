using ISEEDataModel.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Web;

namespace ISEE.Common
{
    public enum LocationSearchResultEnum
    {
        NotFound = -1,
        Found = 0,
        NotProcessed = 1,

        Error = 2,
        OverProcessCountLimit = 3,
        RequestDenied = 4,
        FoundMulty = 5

    }

    [DataContract]
    public class Address
    {
        //[DataMember]
        //[Key]
        //public int ID { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string Building { get; set; }

        [DataMember]
        public string FullAddressString { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public string LocationType { get; set; }

        [DataMember]
        public int SearchResult { get; set; }

    }

    [DataContract]
    public class GoogleSearchAddressResponse
    {
        [DataMember]
        public int SearchResult { get; set; }  // 0-ok,-1 -not found,1-not processed, 2-error; 
        [DataMember]
        public IEnumerable<Address> ServiceResponseAddresses { get; set; }


    }
    [DataContract]
    public class GeoResponse
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "results")]
        public CResult[] Results { get; set; }
        [DataContract]
        public class CResult
        {
            [DataMember(Name = "formatted_address")]
            public string formatted_address { get; set; }

            [DataMember(Name = "type")]
            public string type { get; set; }
            [DataMember(Name = "address_components")]
            public GeocodeAddressComponent[] AddressComponents { get; set; }

            [DataContract]
            public class GeocodeAddressComponent
            {
                [DataMember(Name = "long_name")]
                public string LongName { get; set; }
                [DataMember(Name = "short_name")]
                public string ShortName { get; set; }
                [DataMember(Name = "types")]
                public string[] Type { get; set; }
            }
            [DataMember(Name = "geometry")]
            public CGeometry Geometry { get; set; }
            [DataContract]
            public class CGeometry
            {
                [DataMember(Name = "location")]
                public CLocation Location { get; set; }
                [DataMember(Name = "location_type")]
                public string LocationType { get; set; }
                [DataContract]
                public class CLocation
                {
                    [DataMember(Name = "lat")]
                    public double Lat { get; set; }
                    [DataMember(Name = "lng")]
                    public double Lng { get; set; }
                }
            }
        }
    }

    [DataContract]
    public class GoogleAddressPartsToDB
    {

        [DataMember]
        public int CountryId { get; set; }
        [DataMember]
        public int PartId { get; set; }
        [DataMember]
        public string GooglePartName { get; set; }
        [DataMember]
        public int OrderToShow { get; set; }
        [DataMember]
        public int Insert { get; set; }
    }

    
    [EnableClientAccess()]
    public class GoogleDomainService : DomainService
    {
        public GoogleSearchAddressResponse GetLocationFullString(int CountryId, string Country, string strAddress)
        {

            GoogleSearchAddressResponse location = new GoogleSearchAddressResponse();
            List<Address> points = new List<Address>();
            GeoResponse r = GetGoogleLocation(strAddress);
            if (r != null)
            {
                if ((r.Status == "OK"))
                {
                    location.SearchResult = (int)LocationSearchResultEnum.Found;
                    var v = r.Results.ToList();
                    foreach (var rr in v)
                    {

                        Address s_addr = new Address();
                        GeoResponse.CResult.CGeometry.CLocation loc = rr.Geometry.Location;
                        s_addr.Latitude = loc.Lat;
                        s_addr.Longitude = loc.Lng;
                        s_addr.FullAddressString = rr.formatted_address;
                        s_addr.LocationType = rr.Geometry.LocationType;
                        string sNum = "", sStreet = "", sCity = "", sState = "";
                        GeoResponse.CResult.GeocodeAddressComponent[] addrc = rr.AddressComponents;
                        List<GoogleAddressPartsToDB> addrDbProps = GetCountryGoogleProps(CountryId);
                        if ((addrDbProps != null) && (addrDbProps.Count > 0))
                        {
                            List<GeoResponse.CResult.GeocodeAddressComponent> responseComponents = addrc.ToList();
                            sState = GetAddressPartString(2, responseComponents, addrDbProps);
                            sCity = GetAddressPartString(3, responseComponents, addrDbProps);
                            sStreet = GetAddressPartString(4, responseComponents, addrDbProps);
                            sNum = GetAddressPartString(5, responseComponents, addrDbProps);
                            s_addr.Building = sNum;
                            s_addr.Street = sStreet;
                            s_addr.City = sCity;
                            s_addr.State = sState;
                            s_addr.Country = Country;


                        }
                        else
                        {
                            for (int i = 0; i < addrc.Length; i++)
                            {
                                string sprop = addrc[i].Type[0];
                                if (sprop == "street_number")
                                    sNum = addrc[i].ShortName;
                                if (sprop == "route")
                                    sStreet = addrc[i].ShortName;
                                if (sprop == "administrative_area_level_1")
                                    sState = addrc[i].ShortName;
                                if (sprop == "locality")
                                    sCity = addrc[i].ShortName;
                            }
                        }
                        s_addr.Building = sNum;
                        s_addr.Street = sStreet;
                        s_addr.City = sCity;
                        s_addr.State = sState;
                        s_addr.Country = Country;
                        points.Add(s_addr);
                    }

                }

                else if (r.Status == "ZERO_RESULTS")
                {
                    location.SearchResult = (int)LocationSearchResultEnum.NotFound;
                }
                else if (r.Status == "REQUEST_DENIED")
                {
                    location.SearchResult = (int)LocationSearchResultEnum.RequestDenied;
                }
                else if (r.Status == "OVER_QUERY_LIMIT")
                {
                    location.SearchResult = (int)LocationSearchResultEnum.OverProcessCountLimit;
                }
                else
                    location.SearchResult = (int)LocationSearchResultEnum.Error;
            }

            location.ServiceResponseAddresses = points;

            return location;
        }

        public GoogleSearchAddressResponse GetLocation(Address address)
        {
            string strAddress = BuildAddressString(address);
            GoogleSearchAddressResponse location = new GoogleSearchAddressResponse();
            List<Address> points = new List<Address>();
            GeoResponse r = GetGoogleLocation(strAddress);
            if (r != null)
            {
                if ((r.Status == "OK"))
                {
                    location.SearchResult = (int)LocationSearchResultEnum.Found;
                    var v = r.Results.ToList();
                    foreach (var rr in v)
                    {

                        Address s_addr = new Address();
                        GeoResponse.CResult.CGeometry.CLocation loc = rr.Geometry.Location;
                        s_addr.Latitude = loc.Lat;
                        s_addr.Longitude = loc.Lng;
                        s_addr.FullAddressString = rr.formatted_address;
                        s_addr.LocationType = rr.Geometry.LocationType;
                        string sNum = "", sStreet = "", sCity = "", sState = "";
                        GeoResponse.CResult.GeocodeAddressComponent[] addrc = rr.AddressComponents;
                        List<GoogleAddressPartsToDB> addrDbProps = GetCountryGoogleProps(address.CountryId);
                        if ((addrDbProps != null) && (addrDbProps.Count > 0))
                        {
                            List<GeoResponse.CResult.GeocodeAddressComponent> responseComponents = addrc.ToList();
                            sState = GetAddressPartString(2, responseComponents, addrDbProps);
                            sCity = GetAddressPartString(3, responseComponents, addrDbProps);
                            sStreet = GetAddressPartString(4, responseComponents, addrDbProps);
                            sNum = GetAddressPartString(5, responseComponents, addrDbProps);
                            s_addr.Building = sNum;
                            s_addr.Street = sStreet;
                            s_addr.City = sCity;
                            s_addr.State = sState;
                            s_addr.Country = address.Country;


                        }
                        else
                        {
                            for (int i = 0; i < addrc.Length; i++)
                            {
                                string sprop = addrc[i].Type[0];
                                if (sprop == "street_number")
                                    sNum = addrc[i].ShortName;
                                if (sprop == "route")
                                    sStreet = addrc[i].ShortName;
                                if (sprop == "administrative_area_level_1")
                                    sState = addrc[i].ShortName;
                                if (sprop == "locality")
                                    sCity = addrc[i].ShortName;
                            }
                        }
                        s_addr.Building = sNum;
                        s_addr.Street = sStreet;
                        s_addr.City = sCity;
                        s_addr.State = sState;
                        s_addr.Country = address.Country;
                        points.Add(s_addr);
                    }

                }

                else if (r.Status == "ZERO_RESULTS")
                {
                    location.SearchResult = (int)LocationSearchResultEnum.NotFound;
                }
                else if (r.Status == "REQUEST_DENIED")
                {
                    location.SearchResult = (int)LocationSearchResultEnum.RequestDenied;
                }
                else if (r.Status == "OVER_QUERY_LIMIT")
                {
                    location.SearchResult = (int)LocationSearchResultEnum.OverProcessCountLimit;
                }
                else
                    location.SearchResult = (int)LocationSearchResultEnum.Error;
            }

            location.ServiceResponseAddresses = points;

            return location;
        }
        public Address GetGpsAddress(string countryName, int countryId, string Lat, string Long)
        {

            string url = "http://maps.google.com/maps/api/geocode/json?latlng=" + Lat + "," + Long + "&sensor=false";
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GeoResponse));
            GeoResponse r = null;
            try
            {
                r = (GeoResponse)serializer.ReadObject(request.GetResponse().GetResponseStream());
            }
            catch (Exception ee)
            {
                string str = ee.Message;
            }


            Address _location = new Address();
            if (r.Status == "OK")
            {
                _location.SearchResult = (int)LocationSearchResultEnum.Found;
                var v = r.Results.ToList();
                GeoResponse.CResult rr;
                //rr = r.Results.Where(c => c.AddressComponents.Any(x => c.Type.Contains("street_number"))).FirstOrDefault();
                rr =
                    r.Results.Where(c => c.AddressComponents.Any(x => x.Type.Contains("street_number"))).FirstOrDefault();

                if (rr == null)
                {
                    rr = r.Results.Where(c => c.AddressComponents.Any(x => x.Type.Contains("route"))).FirstOrDefault();
                }
                if (rr == null)
                    _location.SearchResult = (int)LocationSearchResultEnum.NotFound;
                else
                {
                    _location.LocationType = rr.Geometry.LocationType;
                    string sNum = "", sStreet = "", sCity = "", sState = "";
                    GeoResponse.CResult.GeocodeAddressComponent[] addrc = rr.AddressComponents;
                    List<GoogleAddressPartsToDB> addrDbProps = GetCountryGoogleProps(Convert.ToInt32(countryId));
                    if ((addrDbProps != null) && (addrDbProps.Count > 0))
                    {
                        List<GeoResponse.CResult.GeocodeAddressComponent> responseComponents = addrc.ToList();
                        sState = GetAddressPartString(2, responseComponents, addrDbProps);
                        sCity = GetAddressPartString(3, responseComponents, addrDbProps);
                        sStreet = GetAddressPartString(4, responseComponents, addrDbProps);
                        sNum = GetAddressPartString(5, responseComponents, addrDbProps);

                    }
                    else
                    {
                        for (int i = 0; i < addrc.Length; i++)
                        {
                            string sprop = addrc[i].Type[0];
                            if (sprop == "street_number")
                                sNum = addrc[i].ShortName;
                            if (sprop == "route")
                                sStreet = addrc[i].ShortName;
                            if (sprop == "administrative_area_level_1")
                            {
                                sState = addrc[i].ShortName;

                            }
                            if (sprop == "locality")
                                sCity = addrc[i].ShortName;
                        }


                    } //if ((addrDbProps != null) && (addrDbProps.Count > 0))

                    _location.Building = sNum;
                    _location.Street = sStreet;
                    _location.City = sCity;
                    _location.State = sState;
                    _location.Country = countryName;
                    _location.FullAddressString = countryName;
                    if (sState.Length > 0)
                    {
                        _location.FullAddressString += " ";
                        _location.FullAddressString += sState;
                    }
                    _location.FullAddressString = _location.FullAddressString + " " + sCity + " " + sStreet;
                    if (sNum.Length > 0)
                    {
                        _location.FullAddressString += " ";
                        _location.FullAddressString += sNum;
                    }
                }


            }
            else if (r.Status == "ZERO_RESULTS")
            {
                _location.SearchResult = (int)LocationSearchResultEnum.NotFound;
            }
            else if (r.Status == "REQUEST_DENIED")
            {
                _location.SearchResult = (int)LocationSearchResultEnum.RequestDenied;
            }
            else if (r.Status == "OVER_QUERY_LIMIT")
            {
                _location.SearchResult = (int)LocationSearchResultEnum.OverProcessCountLimit;
            }
            else
                _location.SearchResult = (int)LocationSearchResultEnum.Error;

            return _location;
        }
        public GeoResponse GetGoogleLocation(string address)
        {
            string url = string.Format("http://maps.google.com/maps/api/geocode/json?address={0}&region=dk&sensor=false", HttpUtility.UrlEncode(address));
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GeoResponse));
            var res = (GeoResponse)serializer.ReadObject(request.GetResponse().GetResponseStream());
            return res;
        }

        private string BuildAddressString(Address _address)
        {
            if (_address.FullAddressString == null)
                return _address.Country + " " + _address.State + " " + _address.City + " " + _address.Street + " " + _address.Building;
            else
            {
                return _address.FullAddressString;
            }
        }
        public List<GoogleAddressPartsToDB> GetCountryGoogleProps(int countryCode)
        {
            List<GoogleAddressPartsToDB> countryProps = null;

            try
            {
                Dictionary<int, object> props = (Dictionary<int, object>)HttpContext.Current.Session["props"];
                object oValue;
                if (props != null)
                {
                    if (props.TryGetValue(countryCode, out oValue))
                    {
                        countryProps = (List<GoogleAddressPartsToDB>)oValue;
                    }
                    else
                    {
                        countryProps = LoadCountryGoogleProps(countryCode);
                        props.Add(countryCode, countryProps);
                    }

                }
                else
                {
                    props = new Dictionary<int, object>();
                    countryProps = LoadCountryGoogleProps(countryCode);
                    props.Add(countryCode, countryProps);
                    HttpContext.Current.Session["props"] = props;
                }

            }
            catch (Exception ex)
            {

                //countryProps = null;
            }


            return countryProps;
        }
        public List<GoogleAddressPartsToDB> LoadCountryGoogleProps(int countryCode)
        {


            List<GoogleAddressPartsToDB> lresult = new List<GoogleAddressPartsToDB>();
            using (var context = new ISEEEntities())
            {

                var v = from c in context.GeocodeToTables
                        join s in context.GeocodeStringResponses
                            on c.GeocodeTagResponse equals s.GeocodeStringResponsePk
                        where c.CountryCode == countryCode

                        select new GoogleAddressPartsToDB()
                        {
                            CountryId = c.CountryCode,
                            PartId = c.AddressTypeTable,
                            GooglePartName = s.GeocodeTagResponse,
                            OrderToShow = c.OrderGeocodeTagResponse,
                            Insert = c.Insert
                        };

                lresult = v.ToList();
            }
            return lresult;
        }
        private string GetAddressPartString(int partId, List<GeoResponse.CResult.GeocodeAddressComponent> addrComponents, List<GoogleAddressPartsToDB> dbParts)
        {
            string strResult = "";

            if (dbParts.Count > 0)
            {
                var v = dbParts.Where(c => c.PartId == partId).OrderBy(c => c.OrderToShow);
                foreach (var googleAddressPartsToDb in v)
                {

                    //string strGoodleName=addrComponents.Where(c=>c.Type.Any())

                    //var s=from c in addrComponents where c.Type.Contains(googleAddressPartsToDb.GooglePartName)


                    var ss = addrComponents.FirstOrDefault(c => c.Type[0] == googleAddressPartsToDb.GooglePartName);
                    if (ss != null)
                    {
                        string strGoodleName = ss.ShortName;
                        strGoodleName = strGoodleName.Replace("Region", "");
                        strGoodleName = strGoodleName.Replace("Province", "");
                        if (strGoodleName.Length > 0)
                        {

                            if (strResult != "")
                                strResult += "-";
                            strResult += strGoodleName;
                        }
                    }

                }
            }

            return strResult.Trim();
        }

    }
}