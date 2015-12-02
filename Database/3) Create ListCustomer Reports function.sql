
Create Proc usp_GetCustomers --1,'678,679'
(
@FactoryID as int,
@Customers as nvarchar(max)
)
As
Begin
SELECT Customer.CustomerId, Customer.CustomerNumber, Customer.Factory, Customer.FirstName, Customer.LastName, Customer.Floor, Customer.Apartment, Customer.AreaPhone1,  
                         Customer.Phone1, Customer.AreaPhone2, Customer.Phone2, Customer.AreaCelolar, Customer.Celolar, Customer.AreaFax, Customer.Fax, Customer.Mail, 
                         Customer.VisitInterval, Customer.NextVisit, Country.CountryDesc, City.CityDesc, Street.StreetDesc, Building.Entry, Building.Number
                         FROM            Customer INNER JOIN
                         Building ON Customer.BuildingCode = Building.BuildingCode INNER JOIN
                         Country ON Building.CountryCode = Country.CountryCode INNER JOIN
                         City ON Building.CityCode = City.CityCode INNER JOIN
                         Street ON Building.StreetCode = Street.StreetCode AND City.CityCode = Street.CityCode
                        Where Customer.Factory=@FactoryID and 
						1= Case when ISNULL(@Customers,'') = '' then 1
						when  ISNULL(@Customers,'') <> '' and Customer.CustomerId in (Select * from dbo.Split(@Customers)) then 1
						else 0 End
End