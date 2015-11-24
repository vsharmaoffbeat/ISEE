$(document).ready(function () {
    LoadMapByFactoryID();
    $(document).on('click', '#tblmapsearchgridEmployee tr', function () {
        $("#tblmapsearchgridEmployee tr").removeClass('active');
        $(this).addClass('active');
        addToSelectedEmployeeDiv($(this));
    });

    $(document).on('click', '#tblmapsearchgridCustomer tr td', function () {
        if ($(this).find('.chk').val() == undefined) {
            $("#tblmapsearchgridCustomer tr").removeClass('active');
            $(this).closest('.customerRow').addClass('active');
            $("#tblmapsearchgridCustomer tr").find('.chk').prop('checked', '');
            $(this).closest('.customerRow').find('.chk').prop('checked', 'true');
            addToSelectedCustomerDiv($(this).closest('.customerRow'));
        }
    });
    GetAllStatesByCountry();
    showEmployeeById();
    var $datepicker = $("#dpDate");
    $datepicker.datepicker();
    $datepicker.datepicker('setDate', new Date());
});


var markers = [];
var Custmarkers = [];
var map;
var CustomerPositionArrayWithEmployee = [];
var CustTitle = '';
var Liverpool = '';

function LoadMapByFactoryID() {
    $.ajax({
        url: "/Data/GetCurrentLogedUserCountery", success: function (result) {
            google.maps.visualRefresh = true;
            Liverpool = new google.maps.LatLng(result[0].Lat, result[0].Long);
            var mapOptions = {
                zoom: result[0].Zoom,
                center: Liverpool,
                mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
            };
            map = new google.maps.Map(document.getElementById("mapmainDiv"), mapOptions);
        }
    });
}

function SearchEmployee() {
    var firstName = $('#txtfirstName').val();
    var LastName = $('#txtlastName').val();
    var Active = $('#chkActive:checked').val() != "on" ? "1" : "0";
    var Number = $('#txtnumber').val();

    $.ajax({
        type: "POST",
        url: "/Map/GetEmployeeForMap",
        data: { firstName: firstName, LastName: LastName, Active: Active, Number: Number },
        dataType: "json",
        success: function (response) {
            $("#tblmapsearchgridEmployee").html('');
            if (response != null) {
                for (var i = 0; i < response.length; i++) {
                    $("#tblmapsearchgridEmployee").append("<tr rel='" + response[i].LastName + "' id='" + response[i].EmployeeID + "'><td class='tg-dx8v'>" + response[i].Number + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].FirstName + "</td></tr>");
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}


var PolyLineArray = [];
function ShowDataOnMap() {
    var EmployeeID = $('#tblmapsearchgridEmployee .active').attr('id');
    var Date = $('#dpDate').val();
    var FromTime = timeParseExact($('#txtfromTime').val());
    var EndTime = timeParseExact($('#txtendTime').val());
    var selectedOpation = $("input:radio[name='choices']:checked").val().toLowerCase();
    if (EmployeeID != "" && EmployeeID != undefined) {
        if (selectedOpation == 'runwayshow') {
            $.ajax({
                type: "POST",
                url: "/Map/GetEmployeeGpsPointsByEmployeeID",
                data: { EmployeeID: EmployeeID, FromTime: FromTime, EndTime: EndTime, Date: Date },
                dataType: "json",
                success: function (response) {
                    if (response.length > 0) {
                        map = new google.maps.Map(document.getElementById('mapmainDiv'), {
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
                        if ($("#chkshowwithCustomer").prop('checked') == true) {
                            for (var i = 0; i < CustomerPositionArrayWithEmployee.length; i++) {
                                var Custmarker = new google.maps.Marker({
                                    position: new google.maps.LatLng(CustomerPositionArrayWithEmployee[i].lat, CustomerPositionArrayWithEmployee[i].long),
                                    map: map,
                                    icon: "https://upload.wikimedia.org/wikipedia/commons/9/92/Map_marker_icon_%E2%80%93_Nicolas_Mollet_%E2%80%93_Home_%E2%80%93_People_%E2%80%93_Default.png",
                                    title: CustomerPositionArrayWithEmployee[i].title
                                });
                                Custmarkers.push(Custmarker);
                            }
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

                        map = new google.maps.Map(document.getElementById('mapmainDiv'), {
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
                        if ($("#chkshowwithCustomer").prop('checked') == true) {
                            for (var i = 0; i < CustomerPositionArrayWithEmployee.length; i++) {
                                var Custmarker = new google.maps.Marker({
                                    position: new google.maps.LatLng(CustomerPositionArrayWithEmployee[i].lat, CustomerPositionArrayWithEmployee[i].long),
                                    map: map,
                                    icon: "https://upload.wikimedia.org/wikipedia/commons/9/92/Map_marker_icon_%E2%80%93_Nicolas_Mollet_%E2%80%93_Home_%E2%80%93_People_%E2%80%93_Default.png",
                                    title: CustomerPositionArrayWithEmployee[i].title
                                });
                                Custmarkers.push(Custmarker);
                            }
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
                        map = new google.maps.Map(document.getElementById("mapmainDiv"), mapOptions);
                        var marker = new google.maps.Marker({
                            position: Liverpool,
                            map: map,
                            icon: "https://www.mapsmarker.com/wp-content/plugins/leaflet-maps-marker-pro/leaflet-dist/images/marker.png",
                            title: response.LastName + " " + response.GpsTime.Hours + ":" + response.GpsTime.Minutes + "  " + response.StopTime.Hours + ":" + response.StopTime.Minutes
                        });
                        markers.push(marker);
                        if ($("#chkshowwithCustomer").prop('checked') == true) {
                            for (var i = 0; i < CustomerPositionArrayWithEmployee.length; i++) {
                                var Custmarker = new google.maps.Marker({
                                    position: new google.maps.LatLng(CustomerPositionArrayWithEmployee[i].lat, CustomerPositionArrayWithEmployee[i].long),
                                    map: map,
                                    icon: "https://upload.wikimedia.org/wikipedia/commons/9/92/Map_marker_icon_%E2%80%93_Nicolas_Mollet_%E2%80%93_Home_%E2%80%93_People_%E2%80%93_Default.png",
                                    title: CustomerPositionArrayWithEmployee[i].title
                                });
                                Custmarkers.push(Custmarker);
                            }
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
    $('#searchDiv').css('display', 'none');
    $('#searchdivCustomer').css('display', 'block');
}


function ShowEmployee() {
    $('#searchdivCustomer').css('display', 'none');
    $('#searchDiv').css('display', 'block');
}
//----------------------------------------------------------------------------------------------------------------------


var stateNames = [];
var stateIds = [];
function GetAllStatesByCountry() {
    stateNames = [];
    $.ajax({
        type: "POST",
        url: "/Data/GetAllStatesByCountry",
        success: function (response) {
            $(response).each(function () {
                $("<option />", {
                    val: this.StateCode,
                    text: this.StateDesc
                }).appendTo($('#ddlstateinputcustomer'));
                if (this.StateDesc == "") {
                    $('#ddlstateinputcustomer').attr('disabled', 'disabled');
                }
                stateNames.push(this.StateDesc);
                stateIds.push(this.StateCode);
            });
            $("#ddlstateinputcustomer").autocomplete({
                source: stateNames,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var abliableDataForCityesName = [];
var abliableDataForCityesIds = [];
function GetCitysByState() {
    if (stateIds[stateNames.indexOf($('#ddlstateinputcustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Data/GetAllCitysByState",
        data: { stateID: stateIds[stateNames.indexOf($('#ddlstateinputcustomer').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                $(response).each(function () {
                    $("<option />", {
                        val: this.CityCode,
                        text: this.CityDesc
                    }).appendTo($('#ddlcityinputCustomer'));
                    if (this.CityDesc == "") {
                        $('#ddlcityinputCustomer').attr('disabled', 'disabled');
                    }
                    abliableDataForCityesName.push(this.CityDesc);
                    abliableDataForCityesIds.push(this.CityCode);
                });
            }
            $("#ddlcityinputCustomer").autocomplete({
                source: abliableDataForCityesName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var abliableDataForStreetName = [];
var abliableDataForStreetId = [];
function GetStreetByCity() {
    if (abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Data/GetAllStreetByCity",
        data: { cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                abliableDataForStreetName = [];
                abliableDataForStreetId = [];
                $(response).each(function () {
                    $("<option />", {
                        val: this.StreetCode,
                        text: this.Streetdesc
                    }).appendTo($('#ddlstreetinputCustomer'));
                    abliableDataForStreetName.push(this.Streetdesc);
                    abliableDataForStreetId.push(this.StreetCode);
                });
            }
            $("#ddlstreetinputCustomer").autocomplete({
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
    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#ddlstreetinputCustomer').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Data/GetAllBuildingsByCity",
        data: { streetID: abliableDataForStreetId[abliableDataForStreetName.indexOf($('#ddlstreetinputCustomer').val())], cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())] },
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
                    }).appendTo($('#ddlbuildinginputCustomer'));
                    abliableDataForBuildingNumber.push(this.BuildingNumber);
                    abliableDataForBuildingId.push(this.BuildingCode);
                    abliableDataForBuildingLat.push(this.BuildingLat);
                    abliableDataForBuildingLong.push(this.BuldingLong);
                });
                $("#ddlbuildinginputCustomer").autocomplete({
                    source: abliableDataForBuildingNumber,
                });
                $('#ddlbuildinginputCustomer').val(abliableDataForBuildingNumber);
                GetSelectedBuildingLatLong();
            }
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var buildingLatLong = [];
function GetSelectedBuildingLatLong() {
    if (abliableDataForBuildingLat[abliableDataForBuildingNumber.indexOf($('#ddlbuildinginputCustomer').val())] == undefined, abliableDataForBuildingLong[abliableDataForBuildingNumber.indexOf($('#ddlbuildinginputCustomer').val())] == undefined)
        return false;
    buildingLatLong = [];
    buildingLatLong.push(abliableDataForBuildingLat[abliableDataForBuildingNumber.indexOf($('#ddlbuildinginputCustomer').val())], abliableDataForBuildingLong[abliableDataForBuildingNumber.indexOf($('#ddlbuildinginputCustomer').val())]);
}


function SearchCustomers() {
    var state, city, street, building, customerNumber, companyName;
    customerNumber = $('#txtcustomernoInput').val();
    companyName = $('#txtcompanynameInputCustomer').val();
    buildingNumber = $('#ddlbuildinginputCustomer').val();

    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#ddlstreetinputCustomer').val())] == undefined || stateIds[stateNames.indexOf($('#ddlstateinputcustomer').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())] == undefined || abliableDataForBuildingId[abliableDataForBuildingNumber.indexOf($('#ddlbuildinginputCustomer').val())] == undefined) {
        $('#ddlcityinputCustomer').val('');
        $('#ddlstreetinputCustomer').val('');
        $('#ddlbuildinginputCustomer').val('');
        $.ajax({
            type: "POST",
            url: "/Map/GetAllCustomers",
            dataType: "json",
            success: function (response) {
                if (response.length > 0) {
                    $("#tblmapsearchgridCustomer").html('');
                    if (response != null) {
                        for (var i = 0; i < response.length; i++) {
                            $("#tblmapsearchgridCustomer").append("<tr class='customerRow' id='" + response[i].CustomerId + "' rel='" + response[i].LastName + "' FirstName='" + response[i].FirstName + "'><td class='tg-dx8v'><input type='checkbox' class='chk' name='chkCustomer' onclick='chkcustomerChange(this)'/></td><td class='tg-dx8v'>" + response[i].CustomerId + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].CityName + "</td><td class='tg-dx8v'>" + response[i].StreetName + "</td></tr>");
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
            data: { state: stateIds[stateNames.indexOf($('#ddlstateinputcustomer').val())], city: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())], street: abliableDataForStreetId[abliableDataForStreetName.indexOf($('#ddlstreetinputCustomer').val())], BuildingNumber: buildingNumber, customerNumber: customerNumber, companyName: companyName },
            dataType: "json",
            success: function (response) {
                if (response.length > 0) {
                    $("#tblmapsearchgridCustomer").html('');
                    if (response != null) {

                        for (var i = 0; i < response.length; i++) {
                            $("#tblmapsearchgridCustomer").append("<tr class='customerRow' id='" + response[i].CustomerId + "' rel='" + response[i].LastName + "' FirstName='" + response[i].FirstName + "'><td class='tg-dx8v'><input type='checkbox' class='chk' name='chkCustomer' onclick='chkcustomerChange(this)'/></td><td class='tg-dx8v'>" + response[i].CustomerId + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].CityName + "</td><td class='tg-dx8v'>" + response[i].StreetName + "</td></tr>");
                        }
                    }
                }
                else {
                    alert("No Location Data");
                    $("#tblmapsearchgridCustomer").html('');
                    $('#selectedCustomer').html('');
                    $('#selectedCustomer').css('display', 'none');
                }
            }
        })
    }
};



var checkedCustomersforMap = '';

function ShowCustomerDataOnMap() {
    checkedCustomersforMap = '';
    GetSelectedCustomersIdsForMap();
    if (checkedCustomersforMap != '') {
        $.ajax({
            type: "POST",
            url: "/Map/GetCustomerForMapByCustomerID",
            data: { CheckedCustomers: checkedCustomersforMap },
            dataType: "json",
            success: function (response) {
                if (response != null || response[0].Lat != null) {
                    CustomerPositionArrayWithEmployee = [];
                    var map = new google.maps.Map(document.getElementById('mapmainDiv'), {
                        zoom: 10,
                        center: Liverpool,
                        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                    });
                    var Custmarker, i;
                    for (i = 0; i < response.length; i++) {
                        CustTitle = response[i].FirstName + " " + response[i].LastName;
                        if (response[i].Lat != null && response[i].Long != null) {
                            CustomerPositionArrayWithEmployee.push({ lat: response[i].Lat, long: response[i].Long, title: response[i].FirstName + " " + response[i].LastName });
                            Custmarker = new google.maps.Marker({
                                position: new google.maps.LatLng(response[i].Lat, response[i].Long),
                                map: map,
                                icon: "https://upload.wikimedia.org/wikipedia/commons/9/92/Map_marker_icon_%E2%80%93_Nicolas_Mollet_%E2%80%93_Home_%E2%80%93_People_%E2%80%93_Default.png",
                                title: CustTitle
                            });
                        }
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
    $("#selectedemployeeDiv").html('');
    $("#selectedemployeeDiv").append("<tr><td>" + obj.attr('id') + " </td><td>" + obj.attr('rel') + " </td></tr>");
    $("#selectedemployeeDiv").css('display', 'block');

}

function addToSelectedCustomerDiv(obj) {
    $("#selectedCustomer").html('');
    $("#selectedCustomer").append("<tr id=" + obj.attr('id') + "><td>" + obj.attr('id') + " </td><td>" + obj.attr('FirstName') + " </td><td>" + obj.attr('rel') + " </td></tr>");
    $("#selectedCustomer").css('display', 'block');
}


function chkcustomerChange(obj) {
    var selectedRow = obj.closest('.customerRow');
    if (obj.checked == true) {
        var firstName = selectedRow.attributes.firstName != null ? selectedRow.attributes.firstName.value : "";
        var rel = selectedRow.attributes.rel != null ? selectedRow.attributes.rel.value : "";
        $("#selectedCustomer").append("<tr id=" + selectedRow.attributes.id.value + "><td>" + selectedRow.attributes.id.value + " </td><td>" + firstName + " </td><td>" + rel + " </td></tr>");
        $("#selectedCustomer").css('display', 'block');
    }
    else {
        $("#selectedCustomer #" + selectedRow.attributes.id.value).remove();
    }
    return false;
}


function GetSelectedCustomersIdsForMap() {
    if ($('#selectedCustomer tr').length > 0) {
        for (var i = 0, l = $('#selectedCustomer tr').length; i < l; i++) {
            checkedCustomersforMap += $('#selectedCustomer tr')[i].id + ",";
        }
    }
}



function showEmployeeById() {
    $.ajax({
        type: "POST",
        url: "/Map/GetEmployeeByIdOnLoad",
        dataType: "json",
        success: function (response) {
            $("#tblmapsearchgridEmployee").html('');
            if (response != null) {
                for (var i = 0; i < response.length; i++) {
                    $("#tblmapsearchgridEmployee").append("<tr rel='" + response[i].LastName + "' id='" + response[i].EmployeeID + "' class='active'><td class='tg-dx8v'>" + response[i].Number + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].FirstName + "</td></tr>");
                }
            }
        },

    });
}

//function showCustomerById() {
//    $.ajax({
//        type: "POST",
//        url: "/Map/GetCustomerByIdOnLoad",
//        dataType: "json",
//        success: function (response) {
//            if (response.length > 0) {
//                $("#tblmapsearchgridCustomer").html('');
//                if (response != null) {
//                    for (var i = 0; i < response.length; i++) {
//                        $("#tblmapsearchgridCustomer").append("<tr class='customerRow active' id='" + response[i].CustomerId + "' rel='" + response[i].LastName + "' FirstName='" + response[i].FirstName + "' ><td class='tg-dx8v'><input type='checkbox' class='chk' name='chkCustomer' onclick='chkcustomerChange(this)'/></td><td class='tg-dx8v'>" + response[i].CustomerId + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].CityName + "</td><td class='tg-dx8v'>" + response[i].StreetName + "</td></tr>");
//                    }
//                    chkcustomerChange();
//                    ShowCustomerDataOnMap();
//                }
//            }
//        }
//    })
//}

