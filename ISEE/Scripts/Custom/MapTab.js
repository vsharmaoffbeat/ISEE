$(document).ready(function () {
    LoadMapByFactoryID();
    $(document).on('click', '#mapSearchGrid tr', function () {

        $("#mapSearchGrid tr").removeClass('active');
        $(this).addClass('active');
        addToSelectedEmployeeDiv($(this));
    });
    $(document).on('click', '#mapSearchGridCustomer tr', function () {
        $("#mapSearchGridCustomer tr").removeClass('active');
        $(this).addClass('active');
        addToSelectedCustomerDiv($(this));
    });
    GetStatesByFactoryID();

    var $datepicker = $("#Date");
    $datepicker.datepicker();
    $datepicker.datepicker('setDate', new Date());
});


var markers = [];
var Custmarkers = [];
var map;
var CustomerPositionArrayWithEmployee = [];
var CustTitle = '';


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
            map = new google.maps.Map(document.getElementById("MapMainDiv"), mapOptions);
        }
    });
}

function SearchEmployee() {
    var firstName = $('#firstName').val();
    var LastName = $('#lastName').val();
    var Active = $('#Active:checked').val() != "on" ? "1" : "0";
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
                    $("#mapSearchGrid").append("<tr rel='" + response[i].LastName + "' id='" + response[i].EmployeeID + "'><td class='tg-dx8v'>" + response[i].Number + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].FirstName + "</td></tr>");
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
    var selectedOpation = $('.abc:checked').val() != undefined ? $('.abc:checked').val().toLowerCase() : false;
    if (EmployeeID != "" && EmployeeID != undefined) {
        if (selectedOpation == 'runwayshow') {
            $.ajax({
                type: "POST",
                url: "/Map/GetEmployeeGpsPointsByEmployeeID",
                data: { EmployeeID: EmployeeID, FromTime: FromTime, EndTime: EndTime, Date: Date },
                dataType: "json",
                success: function (response) {
                    if (response.length > 0) {
                        map = new google.maps.Map(document.getElementById('MapMainDiv'), {
                            zoom: 10,
                            center: new google.maps.LatLng(response[0].Lat, response[0].Long),
                            mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                        });
                        for (var i = 0; i < response.length; i++) {
                            PolyLineArray.push(new google.maps.LatLng(response[i].Lat, response[i].Long));

                        }
                        var flightPath = new google.maps.Polyline({
                            path: PolyLineArray,
                            strokeColor: "#0000FF",
                            strokeOpacity: 0.8,
                            strokeWeight: 2
                        });
                        flightPath.setMap(map);
                        if ($("#showWithCustomer").prop('checked') == true) {
                            var Custmarker = new google.maps.Marker({
                                position: new google.maps.LatLng(CustomerPositionArrayWithEmployee[0], CustomerPositionArrayWithEmployee[1]),
                                map: map,
                                icon: "https://upload.wikimedia.org/wikipedia/commons/9/92/Map_marker_icon_%E2%80%93_Nicolas_Mollet_%E2%80%93_Home_%E2%80%93_People_%E2%80%93_Default.png",
                                title: CustTitle
                            });
                            Custmarkers.push(Custmarker);
                        }

                    }
                    else {
                        alert("No Location Data");
                        for (var i = 0; i < markers.length; i++) {
                            markers[i].setMap(null);
                        }
                    }
                }
            });

        }
        else if (selectedOpation == 'stoppoint') {

            $.ajax({
                type: "POST",
                url: "/Map/GetStopPointsForEmployee",
                data: { EmployeeID: EmployeeID, FromTime: FromTime, EndTime: EndTime, Date: Date },
                dataType: "json",
                success: function (response) {
                    if (response.length > 0) {

                        map = new google.maps.Map(document.getElementById('MapMainDiv'), {
                            zoom: 10,
                            center: new google.maps.LatLng(response[0].Lat, response[0].Long),
                            mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                        });
                        var marker, i;
                        for (i = 0; i < response.length; i++) {
                            marker = new google.maps.Marker({
                                position: new google.maps.LatLng(response[i].Lat, response[i].Long),
                                map: map,
                                icon: "https://www.mapsmarker.com/wp-content/plugins/leaflet-maps-marker-pro/leaflet-dist/images/marker.png",
                                title: response[i].LastName + " " + response[i].GpsTime.Hours + ":" + response[i].GpsTime.Minutes + "  " + response[i].StopTime.Hours + ":" + response[i].StopTime.Minutes
                            });
                            markers.push(marker);
                        }
                        if ($("#showWithCustomer").prop('checked') == true) {
                            var Custmarker = new google.maps.Marker({
                                position: new google.maps.LatLng(CustomerPositionArrayWithEmployee[0], CustomerPositionArrayWithEmployee[1]),
                                map: map,
                                icon: "https://upload.wikimedia.org/wikipedia/commons/9/92/Map_marker_icon_%E2%80%93_Nicolas_Mollet_%E2%80%93_Home_%E2%80%93_People_%E2%80%93_Default.png",
                                title: CustTitle
                            });
                            Custmarkers.push(Custmarker);
                        }
                    }
                    else {
                        alert("No Location Data");
                        for (var i = 0; i < markers.length; i++) {
                            markers[i].setMap(null);
                        }
                    }
                }
            });

        }
        else if (selectedOpation == 'lastpoint') {

            $.ajax({
                type: "POST",
                url: "/Map/GetLastPointForEmployee",
                data: { EmployeeID: EmployeeID, FromTime: FromTime, EndTime: EndTime, Date: Date },
                dataType: "json",
                success: function (response) {

                    if (response != false) {
                        var Liverpool = new google.maps.LatLng(response.Lat, response.Long);
                        var mapOptions = {
                            zoom: 14,
                            center: Liverpool,
                            mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                        };
                        map = new google.maps.Map(document.getElementById("MapMainDiv"), mapOptions);
                        var marker = new google.maps.Marker({
                            position: Liverpool,
                            map: map,
                            icon: "https://www.mapsmarker.com/wp-content/plugins/leaflet-maps-marker-pro/leaflet-dist/images/marker.png",
                            title: response.LastName + " " + response.GpsTime.Hours + ":" + response.GpsTime.Minutes + "  " + response.StopTime.Hours + ":" + response.StopTime.Minutes
                        });
                        markers.push(marker);
                        if ($("#showWithCustomer").prop('checked') == true) {
                            var Custmarker = new google.maps.Marker({
                                position: new google.maps.LatLng(CustomerPositionArrayWithEmployee[0], CustomerPositionArrayWithEmployee[1]),
                                map: map,
                                icon: "https://upload.wikimedia.org/wikipedia/commons/9/92/Map_marker_icon_%E2%80%93_Nicolas_Mollet_%E2%80%93_Home_%E2%80%93_People_%E2%80%93_Default.png",
                                title: CustTitle
                            });
                            Custmarkers.push(Custmarker);
                        }

                    } else {
                        alert("No Location Data");
                        for (var i = 0; i < markers.length; i++) {
                            markers[i].setMap(null);
                        }
                    }
                }
            });
        }
    } else {
        alert("Must select a employee");
    }
}
function showTime(obj) {
    $('#' + obj.id + '').timepicker({ 'timeFormat': 'h:i A' });

    if ($('#' + obj.id + '').val() == null || $('#' + obj.id + '').val() == undefined) {

    }
}

function timeParseExact(time) {
    if (time != undefined && time != "") {
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
        url: "/Admin/GetAllStatesByCountry",
        success: function (response) {
            $(response).each(function () {
                $("<option />", {
                    val: this.StateCode,
                    text: this.StateDesc
                }).appendTo($('#StateInputCustomer'));
                if (this.StateDesc == null) {
                    $('#StateInputCustomer').attr('disabled', 'disabled');
                }
                stateNames.push(this.StateDesc);
                stateIds.push(this.StateCode);
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
        url: "/Admin/GetAllCitysByState",
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
    if (abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Admin/GetAllStreetByCity",
        data: { cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())] },
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
    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#StreetInputCustomer').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Admin/GetAllBuildingsByCity",
        data: { streetID: abliableDataForStreetId[abliableDataForStreetName.indexOf($('#StreetInputCustomer').val())], cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {

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
    buildingNumber = $('#BuildingInputCustomer').val();

    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#StreetInputCustomer').val())] == undefined || stateIds[stateNames.indexOf($('#StateInputCustomer').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())] == undefined || abliableDataForBuildingId[abliableDataForBuildingNumber.indexOf($('#BuildingInputCustomer').val())] == undefined) {
        $.ajax({
            type: "POST",
            url: "/Map/GetAllCustomers",
            dataType: "json",
            success: function (response) {
                if (response.length > 0) {
                    $("#mapSearchGridCustomer").html('');
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            $("#mapSearchGridCustomer").append("<tr id='" + response[i].CustomerId + "' rel='" + response[i].LastName + "' FirstName='" + response[i].FirstName + "'><td class='tg-dx8v'>" + response[i].CustomerId + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].CityName + "</td><td class='tg-dx8v'>" + response[i].StreetName + "</td></tr>");
                        }
                    }
                }
            }
        })
    }
    else {
        $.ajax({
            type: "POST",
            url: "/Map/GetCustomersForMap",
            data: { state: stateIds[stateNames.indexOf($('#StateInputCustomer').val())], city: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#CityInputCustomer').val())], street: abliableDataForStreetId[abliableDataForStreetName.indexOf($('#StreetInputCustomer').val())], BuildingNumber: buildingNumber, customerNumber: customerNumber, companyName: companyName },
            dataType: "json",
            success: function (response) {
                if (response.length > 0) {
                    $("#mapSearchGridCustomer").html('');
                    if (response != null) {

                        for (var i = 0; i < response.length; i++) {
                            $("#mapSearchGridCustomer").append("<tr id='" + response[i].CustomerId + "' rel='" + response[i].LastName + "' FirstName='" + response[i].FirstName + "'><td class='tg-dx8v'>" + response[i].CustomerId + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].CityName + "</td><td class='tg-dx8v'>" + response[i].StreetName + "</td></tr>");
                        }
                    }
                }
                else {
                    alert("No Location Data");
                    $("#mapSearchGridCustomer").html('');
                    $('#selectedCustomer').html('');
                    $('#selectedCustomer').css('display', 'none');
                }
            }
        })
    }
};

function ShowCustomerDataOnMap() {
    var CustomerID = $('#mapSearchGridCustomer .active').attr('id');
    if (CustomerID != undefined && CustomerID != null) {
        $.ajax({
            type: "POST",
            url: "/Map/GetCustomerForMapByCustomerID",
            data: { CustomerID: CustomerID },
            dataType: "json",
            success: function (response) {
                if (response != null || response[0].Lat != null) {
                    CustomerPositionArrayWithEmployee = [];
                    CustomerPositionArrayWithEmployee.push(response[0].Lat, response[0].Long);
                    var map = new google.maps.Map(document.getElementById('MapMainDiv'), {
                        zoom: 10,
                        center: new google.maps.LatLng(response[0].Lat, response[0].Long),
                        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                    });
                    var Custmarker, i;
                    for (i = 0; i < response.length; i++) {
                        CustTitle = response[i].FirstName + " " + response[i].LastName;
                        Custmarker = new google.maps.Marker({
                            position: new google.maps.LatLng(response[i].Lat, response[i].Long),
                            map: map,
                            icon: "https://upload.wikimedia.org/wikipedia/commons/9/92/Map_marker_icon_%E2%80%93_Nicolas_Mollet_%E2%80%93_Home_%E2%80%93_People_%E2%80%93_Default.png",
                            title: response[i].FirstName + " " + response[i].LastName
                        });
                        Custmarkers.push(Custmarker);
                    }
                }
            }
        })
    }
    else {
        alert("Must Select a Customer");
        for (var i = 0; i < Custmarkers.length; i++) {
            Custmarkers[i].setMap(null);
        }
    }
}

function addToSelectedEmployeeDiv(obj) {
    $("#selectedEmployee").html('');
    $("#selectedEmployee").append("<tr><td>" + obj.attr('id') + " </td><td>" + obj.attr('rel') + " </td></tr>");
    $("#selectedEmployee").css('display', 'block');

}

function addToSelectedCustomerDiv(obj) {
    $("#selectedCustomer").html('');
    $("#selectedCustomer").append("<tr><td>" + obj.attr('id') + " </td><td>" + obj.attr('FirstName') + " </td><td>" + obj.attr('rel') + " </td></tr>");
    $("#selectedCustomer").css('display', 'block');

}
