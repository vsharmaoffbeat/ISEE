﻿$(document).ready(function () {
    LoadMapByFactoryID();
    $(document).on('click', '#mapSearchGrid tr', function () {
        $("#mapSearchGrid tr").removeClass('active');
        $(this).addClass('active');
    });
    $(document).on('click', '#mapSearchGridCustomer tr', function () {
        $("#mapSearchGridCustomer tr").removeClass('active');
        $(this).addClass('active');
    });
    GetStatesByFactoryID();
    $("#Date").datepicker({
        onSelect: function () {
            datepicker1 = $(this).datepicker('getDate');
        }
    });
});

//var customerEmployeeMarkers = [];
function LoadMapByFactoryID() {
    $.ajax({
        url: "/Admin/GetCurrentLogedUserCountery", success: function (result) {
            google.maps.visualRefresh = true;
            var Liverpool = new google.maps.LatLng(result[0].Lat, result[0].Long);
            var mapOptions = {
                zoom: 6,
                center: Liverpool,
                mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
            };
            var map = new google.maps.Map(document.getElementById("MapMainDiv"), mapOptions);
        }
    });
}

function SearchEmployee() {
    debugger;
    var firstName = $('#firstName').val();
    var LastName = $('#lastName').val();
    var Active = $('#Active:checked').val() != "on" ? "0" : "1";
    var Number = $('#number').val();

    $.ajax({
        type: "POST",
        url: "/Map/GetEmployeeForMap",
        data: { firstName: firstName, LastName: LastName, Active: Active, Number: Number },
        dataType: "json",
        success: function (response) {
            $("#mapSearchGrid").html('');
            if (response != null) {
                for (var i = 0; i < response.length; i++) {
                    $("#mapSearchGrid").append("<tr id='" + response[i].EmployeeID + "'><td class='tg-dx8v'></td><td class='tg-dx8v'>" + response[i].FirstName + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td></tr>");
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}


var PolyLineArray = [];
function ShowDataOnMap() {
    var EmployeeID = $('#mapSearchGrid .active').attr('id');
    var Date = $('#Date').val();
    var FromTime = timeParseExact($('#FromTime').val());
    var EndTime = timeParseExact($('#EndTime').val());
    debugger;
    if ($('.abc:checked').val().toLowerCase() == 'runwayshow') {
        $.ajax({
            type: "POST",
            url: "/Map/GetEmployeeGpsPointsByEmployeeID",
            data: { EmployeeID: EmployeeID, FromTime: FromTime, EndTime: EndTime, Date: Date },
            dataType: "json",
            success: function (response) {
                var map = new google.maps.Map(document.getElementById('MapMainDiv'), {
                    zoom: 10,
                    center: new google.maps.LatLng(response[0].Lat, response[0].Long),
                    mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                });
                for (var i = 0; i < response.length; i++) {
                    PolyLineArray.push(new google.maps.LatLng(response[i].Lat, response[i].Long));
                    //customerEmployeeMarkers.push(response[i].Lat, response[i].Long);
                }
                var flightPath = new google.maps.Polyline({
                    path: PolyLineArray,
                    strokeColor: "#0000FF",
                    strokeOpacity: 0.8,
                    strokeWeight: 2
                });
                flightPath.setMap(map);
                //var marker, i;
                //for (i = 0; i < response.length; i++) {
                //    marker = new google.maps.Marker({
                //        position: new google.maps.LatLng(response[i].Lat, response[i].Long),
                //        map: map
                //    });
                //}

            }
        });
    }
    else if ($('.abc:checked').val().toLowerCase() == 'stoppoint') {
        $.ajax({
            type: "POST",
            url: "/Map/GetStopPointsForEmployee",
            data: { EmployeeID: EmployeeID, FromTime: FromTime, EndTime: EndTime, Date: Date },
            dataType: "json",
            success: function (response) {
                debugger;
                // google.maps.visualRefresh = true;
                var map = new google.maps.Map(document.getElementById('MapMainDiv'), {
                    zoom: 10,
                    center: new google.maps.LatLng(response[0].Lat, response[0].Long),
                    mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                });

                var marker, i;
                for (i = 0; i < response.length; i++) {
                    marker = new google.maps.Marker({
                        position: new google.maps.LatLng(response[i].Lat, response[i].Long),
                        map: map
                    });
                }
            }
        });
    }
    else {
        debugger;
        $.ajax({
            type: "POST",
            url: "/Map/GetLastPointForEmployee",
            data: { EmployeeID: EmployeeID, FromTime: FromTime, EndTime: EndTime, Date: Date },
            dataType: "json",
            success: function (response) {
                debugger;
                var Liverpool = new google.maps.LatLng(response.Lat, response.Long);
                var mapOptions = {
                    zoom: 14,
                    center: Liverpool,
                    mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                };
                var map = new google.maps.Map(document.getElementById("MapMainDiv"), mapOptions);
                var marker = new google.maps.Marker({
                    position: Liverpool,
                    map: map,
                });



            }
        });
    }
}
function showTime(obj) {
    $('#' + obj.id + '').timepicker({ 'timeFormat': 'h:i A' });

    if ($('#' + obj.id + '').val() == null || $('#' + obj.id + '').val() == undefined) {
        debugger;
    }
}

function timeParseExact(time) {
    var hhmm = time.split(' ')[0];
    var tt = time.split(' ')[1].toLowerCase();
    var hh = hhmm.split(':')[0];
    var mm = hhmm.split(':')[1];
    if (tt == "pm") {
        if (hh == "12") {
            hh = "0";
        }
        return parseFloat(hh + "." + mm) + 12;
    }
    else {
        if (hh == "12") {
            hh = "0";
        }
        return parseFloat(hh + "." + mm);
    }


}

function ShowCustomer() {
    $('#searchdiv').css('display', 'none');
    $('#searchdivCustomer').css('display', 'block');
}


function ShowEmployee() {
    $('#searchdivCustomer').css('display', 'none');
    $('#searchdiv').css('display', 'block');
}
//----------------------------------------------------------------------------------------------------------------------


var stateNames = [];
var stateIds = [];
function GetStatesByFactoryID() {
    stateNames = [];
    $.ajax({
        type: "POST",
        url: "/Admin/GetStaresByFactoryID",
        success: function (response) {
            $(response).each(function () {
                $("<option />", {
                    val: this.CountryCode,
                    text: this.CountryDescEng
                }).appendTo($('#StateInputCustomer'));
                if (this.CountryDescEng == null) {
                    $('#StateInputCustomer').attr('disabled', 'disabled');
                }
                stateNames.push(this.CountryDescEng);
                stateIds.push(this.CountryCode);
            });
            $("#StateInputCustomer").autocomplete({
                source: stateNames,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var abliableDataForCityesName = [];
var abliableDataForCityesIds = [];
function GetCitysByState() {
    if (stateIds[stateNames.indexOf($('#StateInputCustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Admin/GetCitysByState",
        data: { stateID: stateIds[stateNames.indexOf($('#StateInputCustomer').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                $(response).each(function () {
                    $("<option />", {
                        val: this.CityCode,
                        text: this.CityDesc
                    }).appendTo($('#CityInputCustomer'));
                    abliableDataForCityesName.push(this.CityDesc);
                    abliableDataForCityesIds.push(this.CityCode);
                });
            }
            $("#CityInputCustomer").autocomplete({
                source: abliableDataForCityesName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var abliableDataForStreetName = [];
var abliableDataForStreetId = [];
function GetStreetByCity() {
    if (stateIds[stateNames.indexOf($('#StateInputCustomer').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Admin/GetStreetByCity",
        data: { stateID: stateIds[stateNames.indexOf($('#StateInputCustomer').val())], cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                abliableDataForStreetName = [];
                abliableDataForStreetId = [];
                $(response).each(function () {
                    $("<option />", {
                        val: this.StreetCode,
                        text: this.Streetdesc
                    }).appendTo($('#StreetInputCustomer'));
                    abliableDataForStreetName.push(this.Streetdesc);
                    abliableDataForStreetId.push(this.StreetCode);
                });
            }
            $("#StreetInputCustomer").autocomplete({
                source: abliableDataForStreetName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var abliableDataForBuildingNumber = [];
var abliableDataForBuildingId = [];
var abliableDataForBuildingLat = [];
var abliableDataForBuildingLong = [];

function GetBuildingsByCity() {
    abliableDataForBuilding = [];
    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#StreetInputCustomer').val())] == undefined || stateIds[stateNames.indexOf($('#StateInputCustomer').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Admin/GetBuildingsByCity",
        data: { streetID: abliableDataForStreetId[abliableDataForStreetName.indexOf($('#StreetInputCustomer').val())], stateID: stateIds[stateNames.indexOf($('#StateInputCustomer').val())], cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                debugger;
                $(response).each(function () {
                    abliableDataForBuildingNumber = [];
                    abliableDataForBuildingId = [];
                    abliableDataForBuildingLat = [];
                    abliableDataForBuildingLong = [];
                    $("<option />", {
                        val: this.BuildingCode,
                        text: this.BuildingNumber
                    }).appendTo($('#BuildingInputCustomer'));
                    abliableDataForBuildingNumber.push(this.BuildingNumber);
                    abliableDataForBuildingId.push(this.BuildingCode);
                    abliableDataForBuildingLat.push(this.BuildingLat);
                    abliableDataForBuildingLong.push(this.BuldingLong);
                });
                $("#BuildingInputCustomer").autocomplete({
                    source: abliableDataForBuildingNumber,
                });
                $('#BuildingInputCustomer').val(abliableDataForBuildingNumber);
                GetSelectedBuildingLatLong();
            }
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var buildingLatLong = [];
function GetSelectedBuildingLatLong() {
    if (abliableDataForBuildingLat[abliableDataForBuildingNumber.indexOf($('#BuildingInputCustomer').val())] == undefined, abliableDataForBuildingLong[abliableDataForBuildingNumber.indexOf($('#BuildingInputCustomer').val())] == undefined)
        return false;
    buildingLatLong = [];
    buildingLatLong.push(abliableDataForBuildingLat[abliableDataForBuildingNumber.indexOf($('#BuildingInputCustomer').val())], abliableDataForBuildingLong[abliableDataForBuildingNumber.indexOf($('#BuildingInputCustomer').val())]);
}


function SearchCustomers() {
    var state, city, street, building, customerNumber, companyName;
    customerNumber = $('#CustomerNoInput').val();
    companyName = $('#CompanyNameInput').val();

    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#StreetInputCustomer').val())] == undefined || stateIds[stateNames.indexOf($('#StateInputCustomer').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())] == undefined || abliableDataForBuildingId[abliableDataForBuildingNumber.indexOf($('#BuildingInputCustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Map/GetCustomersForMap",
        data: { state: stateIds[stateNames.indexOf($('#StateInputCustomer').val())], city: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())], street: abliableDataForStreetId[abliableDataForStreetName.indexOf($('#StreetInputCustomer').val())], BuildingID: abliableDataForBuildingId[abliableDataForBuildingNumber.indexOf($('#BuildingInputCustomer').val())], customerNumber: customerNumber, companyName: companyName },
        dataType: "json",
        success: function (response) {
            $("#mapSearchGridCustomer").html('');
            if (response != null) {
                debugger;
                for (var i = 0; i < response.length; i++) {
                    $("#mapSearchGridCustomer").append("<tr id='" + response[i].CustomerID + "' rel='" + response[i].BuildingCode + "'><td class='tg-dx8v'></td><td class='tg-dx8v'>" + response[i].FirstName + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td></tr>");
                }
            }
        }
    })
};

function ShowCustomerDataOnMap() {
    var CustomerID = $('#mapSearchGridCustomer .active').attr('id');
    $.ajax({
        type: "POST",
        url: "/Map/GetCustomerForMapByCustomerID",
        data: { CustomerID: CustomerID },
        dataType: "json",
        success: function (response) {
            if (response != null || response[0].Lat != null) {
                var map = new google.maps.Map(document.getElementById('MapMainDiv'), {
                    zoom: 10,
                    center: new google.maps.LatLng(response[0].Lat, response[0].Long),
                    mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                });
                //if ($('#showWithCustomer:checked').val() != "on") {
                var marker, i;
                for (i = 0; i < response.length; i++) {
                    //customerEmployeeMarkers.push(response[i].Lat, response[i].Long);
                    marker = new google.maps.Marker({
                        position: new google.maps.LatLng(response[i].Lat, response[i].Long),
                        map: map
                    });
                }
                //}
                //else {
                //    var marker, i;
                //    for (i = 0; i < customerEmployeeMarkers.length; i++) {
                //        marker = new google.maps.Marker({
                //            position: new google.maps.LatLng(customerEmployeeMarkers[i].Lat, customerEmployeeMarkers[i].Long),
                //            map: map
                //        });
                //    }
                //}
            }
        }
    })
}