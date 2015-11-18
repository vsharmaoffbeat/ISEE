var stateNames = [];
var stateIds = [];
$(document).ready(function () {
    GetStaresByFactoryID();
});





function GetStaresByFactoryID() {
    stateNames = [];
    stateIds = [];
    $.ajax({
        type: "POST",
        url: "/Data/GetStaresByFactoryID",
        success: function (response) {
            if (response.length == 1 && response[0].CountryDescEng == null) {

                stateNames.push('');
                stateIds.push(response[0].CountryCode);
                $('#custState').val('');
                GetCitysByState();
            }
            else if (response != null && response.length > 0 && response[0].CityDesc != null) {
                $(response).each(function () {
                    stateNames.push(this.CountryDescEng);
                    stateIds.push(this.CountryCode);
                });
            }
            $("#custState").autocomplete({
                source: stateNames,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

//Get cities based on state
var abliableDataForCityesName = [];
var abliableDataForCityesIds = [];
function GetCitysByState() {
    if (stateIds[stateNames.indexOf($('#custState').val())] == undefined)
        return false;

    abliableDataForCityesName = [];
    abliableDataForCityesIds = [];
    $.ajax({
        type: "POST",
        url: "/Data/GetCitysByState",
        data: { stateID: stateIds[stateNames.indexOf($('#custState').val())] },
        dataType: "json",
        success: function (response) {
            if (response.length == 1 && response[0].CityDesc == null) {
                abliableDataForCityesName.push('');
                abliableDataForCityesIds.push(response[0].CityCode);
                $('#custCity').val('');
                GetStreetByCity();
            }
            else if (response != null && response.length > 0 && response[0].CityDesc != null) {
                $(response).each(function () {
                    abliableDataForCityesName.push(this.CityDesc);
                    abliableDataForCityesIds.push(this.CityCode);
                });
            }
            $("#custCity").autocomplete({
                source: abliableDataForCityesName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

//Get street based on state and city
var abliableDataForStreetName = [];
var abliableDataForStreetId = [];
function GetStreetByCity() {
    if (stateIds[stateNames.indexOf($('#custState').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Data/GetStreetByCity",
        data: { stateID: stateIds[stateNames.indexOf($('#custState').val())], cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())] },
        dataType: "json",
        success: function (response) {
            if (response.length == 1 && response[0].Streetdesc == null) {

                abliableDataForStreetName.push('');
                abliableDataForStreetId.push(response[0].StreetCode);

                $('#custStreet').val('');
                GetBuildingsByCity();
            }
            else if (response != null && response.length > 0 && response[0].Streetdesc != null) {
                $(response).each(function () {
                    abliableDataForStreetName.push(this.Streetdesc);
                    abliableDataForStreetId.push(this.StreetCode);
                });
            }
            $("#custStreet").autocomplete({
                source: abliableDataForStreetName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

//Get Building  based on state and city and street
var abliableDataForBuildingNumber = [];
var abliableDataForBuildingId = [];
var abliableDataForBuildingLat = [];
var abliableDataForBuildingLong = [];

function GetBuildingsByCity() {
    abliableDataForBuilding = [];
    abliableDataForBuildingId = [];
    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#custStreet').val())] == undefined || stateIds[stateNames.indexOf($('#custState').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Data/GetBuildingsByCity",
        data: { streetID: abliableDataForStreetId[abliableDataForStreetName.indexOf($('#custStreet').val())], stateID: stateIds[stateNames.indexOf($('#custState').val())], cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())] },
        dataType: "json",
        success: function (response) {
            if (response.length == 1 && response[0].BuildingNumber == null) {

                abliableDataForBuildingNumber.push('');
                abliableDataForBuildingId.push(response[0].BuildingCode);

                $('#custBuldingNumber').val('');
                //   GetBuildingsByCity();
            }
            if (response != null && response.length > 0 && response[0].BuildingNumber != null) {
                $(response).each(function () {
                    abliableDataForBuildingNumber.push(this.BuildingNumber);
                    abliableDataForBuildingId.push(this.BuildingCode);
                });
            }
            $("#custBuldingNumber").autocomplete({
                source: abliableDataForBuildingNumber,
            });
            //  $('#inputBuldingNumber').val(abliableDataForBuildingNumber);

            //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
        }
    });
}

//Search function for customer 
function searchCustomerData() {
    $('#left_employee_window').empty();
    var state = 0;
    var city = 0;
    //   var building = 0;
    var street = 0;

    if (stateIds[stateNames.indexOf($('#custState').val())] != undefined)
        state = stateIds[stateNames.indexOf($('#custState').val())];
    if (abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())] != undefined)
        city = abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())];
    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#custStreet').val())] != undefined)
        street = abliableDataForStreetId[abliableDataForStreetName.indexOf($('#custStreet').val())];

    //if (abliableDataForBuildingId[abliableDataForBuildingNumber.indexOf($('#custBuldingNumber').val())] != undefined)
    //    building = abliableDataForBuildingId[abliableDataForBuildingNumber.indexOf($('#custBuldingNumber').val())];


    var data = { state: state, city: city, street: street, building: $('#custBuldingNumber').val(), custNumber: $('#custNumber').text().trim(), firstName: $('#custName').text().trim(), lastName: $('#custCompany').text().trim(), phone: $('#custPhone').text().trim(), phone1: $('#custPhone1').text().trim(), isActive: $('#isActive').is(':checked') }


    // { manufacture: $("#ddlmanufacture").val(), lastName: $('#empLastname').val(), firstName: $('#empFirstname').val(), empNumber: $('#empNumber').val(), phoneType: $('#ddlphoneType').val(), isActive: $('#isActive').is(':checked') }
    $.ajax({
        type: "POST",
        url: "/Customer/GetCustomerSarch",
        data: data,
        dataType: "json",
        success: function (response) {
            if (response == null) {
                $('#left_employee_window').text('No records found.');
                return true;
            }
            debugger;
            var setAttr = ''
            $(response).each(function () {
                $('<div class="col-sm-12 col-xs-12 tab_box" onclick="selectedCustomer(this)"' +

                     '" CustomerId="' + this.CustomerId + '" CustomerNumber="' + this.CustomerNumber + '" FirstName="' + this.FirstName +
                      '" LastName="' + this.LastName + '" Floor="' + this.Floor + '" Apartment="' + this.Apartment +
                       '" Phone2="' + this.Phone2 + '" AreaFax="' + this.AreaFax + '" Fax="' + this.Fax +
                        '" AreaPhone2="' + this.AreaPhone2 + '" Phone1="' + this.Phone1 + '" AreaPhone1="' + this.AreaPhone1 +
                         '" Mail="' + this.Mail + '" CustomerRemark1="' + this.CustomerRemark1 + '" CustomerRemark2="' + this.CustomerRemark2 +
                          '" VisitInterval="' + this.VisitInterval + '" NextVisit="' + this.NextVisit + '" LastName="' + this.LastName +
                           '" VisitDate="' + this.VisitDate + '" VisitTime="' + this.VisitTime + '" ZipCode="' + this.ZipCode +
                       '>Company Name: '
                   + this.FirstName + ' <p>Contact Name: ' + this.LastName + '</p><p>City: ' + this.MainAreaPhone + '-' + this.MainPhone
                   + '</p>        </div>').appendTo($('.scroll_window'));
                $('<div class="row" onclick="selectedEmployee(this)" EmployeeId="' + this.EmployeeId + '" EmployeeNum="' + this.EmployeeNum +
                    '" Mail="' + this.Mail + '" FirstName="' + this.FirstName + '" LastName="' + this.LastName +
                    '" LastSendApp="' + this.LastSendApp + '" EndDay="' + this.EndDay + '" PhoneManufactory="' + this.PhoneManufactory +
                    '" PhoneType="' + this.PhoneType +
                    '" MainPhone="' + this.MainPhone + '" MainAreaPhone="' + this.MainAreaPhone +
                    '" SecondAreaPhone="' + this.SecondAreaPhone + '" SecondPhone="' + this.SecondPhone +
                    '" StartDay="' + this.StartDay +

                    '"> <div class="col-md-12 col-xs-12 tab_box">First Name: '
                   + this.FirstName + ' <p>Last Name: ' + this.LastName + '</p><p>Phone1: ' + this.MainAreaPhone + '-' + this.MainPhone
                   + '</p></div></div>').appendTo($('#left_employee_window'));
            });


        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });

}
function selectedEmployee(obj) {
    var d = new Date();
    d.setMonth(d.getMonth() - 3);
    $("#datepicker1").datepicker('setDate', d);
    d = new Date();
    $("#datepicker2").datepicker('setDate', d);
    var data = $(obj).attr('EmployeeId').split('|');
    _employeeId = $(obj).attr('EmployeeId');
    //get messgae history
    getMessageHistory(_employeeId, $("#datepicker1 input").val(), $("#datepicker2 input").val());
    //get Employeefill hours
    getEmployeeTimeTemplate(_employeeId);
    //get Employee history template
    getEmployeeTimeHistoryDiary();
    //Set employee data
    setInputValues(obj);



}

function selectedCustomer(obj) {
    alert('test');
}