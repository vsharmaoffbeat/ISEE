$(document).ready(function () {
    //LoadMapByCurrentLogedUser();
    $(document).on('click', '#tblmapsearchgridEmployee tr', function () {
        $("#tblmapsearchgridEmployee tr").removeClass('active');
        $(this).addClass('active');
        AddToSelectedEmployeeDiv($(this));
    });

    $(document).on('click', '#tblmapsearchgridCustomer tr td', function () {
        if ($(this).find('.chk').val() == undefined) {
            $("#tblmapsearchgridCustomer tr").removeClass('active');
            $(this).closest('.customerRow').addClass('active');
            $("#tblmapsearchgridCustomer tr").find('.chk').prop('checked', '');
            $(this).closest('.customerRow').find('.chk').prop('checked', 'true');
            AddToSelectedCustomerDiv($(this).closest('.customerRow'));
        }
    });
    GetAllStatesByCountry();
    var $datepicker = $("#dpDate");
    $datepicker.datepicker();
    $datepicker.datepicker('setDate', new Date());
    $('#ddlbuildinginputCustomer').attr('disabled', 'disabled');
    $('#ddlstreetinputCustomer').attr('disabled', 'disabled');

});


var _markers = [];
var _custMarkers = [];
var _map;
var _customerPositionArrayWithEmployee = [];
var _custTitle = '';
var _liverpool = '';
var _polyLineArray = [];
var _stateNames = [];
var _stateIds = [];
var _abliableDataForCityesName = [];
var _abliableDataForCityesIds = [];
var _abliableDataForStreetName = [];
var _abliableDataForStreetId = [];
var _abliableDataForBuildingNumber = [];
var _abliableDataForBuildingId = [];
var _abliableDataForBuildingLat = [];
var _abliableDataForBuildingLong = [];
var _buildingLatLong = [];
var _checkedCustomersforMap = '';

// To load the map on current loged user country.
function LoadMapByCurrentLogedUser() {
    $.ajax({
        url: "/Data/GetCurrentLogedUserCountery", success: function (result) {
            google.maps.visualRefresh = true;
            _liverpool = new google.maps.LatLng(result[0].Lat, result[0].Long);
            var mapOptions = {
                zoom: result[0].Zoom,
                center: _liverpool,
                mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
            };
            _map = new google.maps.Map(document.getElementById("mapMainDiv"), mapOptions);
        }
    });
}

// To search employees by first name, last name,active and customer number
function SearchEmployee() {
    var firstName = $('#txtfirstName').val();
    var lastName = $('#txtlastName').val();
    var active = $('#chkActive:checked').val() != "on" ? false : true;
    var number = $('#txtnumber').val();

    $.ajax({
        type: "POST",
        url: "/Map/GetEmployeeForMap",
        data: { firstName: firstName, lastName: lastName, active: active, number: number },
        dataType: "json",
        success: function (response) {
            $("#tblmapsearchgridEmployee").html('');
            if (response != null) {
                for (var i = 0; i < response.length; i++) {
                    $("#tblmapsearchgridEmployee").append("<tr rel='" + response[i].LastName + "' id='" + response[i].EmployeeID + "'><td class='tg-dx8v'>" + response[i].Number + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].FirstName + "</td></tr>");
                }
            }
        },
    });
}

// To show selected employee on map with selected opations(runwayshow,stoppoint,lastpoint).
function ShowEmployeeDataOnMap() {
    var employeeID = $('#tblmapsearchgridEmployee .active').attr('id');
    var date = $('#dpDate').val();
    var fromTime = TimeParseExact($('#txtfromTime').val());
    var endTime = TimeParseExact($('#txtendTime').val());
    var selectedOpation = $("input:radio[name='choices']:checked").val().toLowerCase();
    if (employeeID != "" && employeeID != undefined) {
        if (selectedOpation == 'runwayshow') {
            $.ajax({
                type: "POST",
                url: "/Map/GetEmployeeGpsPointsByEmployeeID",
                data: { employeeID: employeeID, fromTime: fromTime, endTime: endTime, date: date },
                dataType: "json",
                success: function (response) {
                    if (response.length > 0) {
                        _map = new google.maps.Map(document.getElementById('mapMainDiv'), {
                            zoom: 10,
                            center: new google.maps.LatLng(response[0].Lat, response[0].Long),
                            mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                        });
                        for (var i = 0; i < response.length; i++) {
                            _polyLineArray.push(new google.maps.LatLng(response[i].Lat, response[i].Long));

                        }
                        var flightPath = new google.maps.Polyline({
                            path: _polyLineArray,
                            strokeColor: "#0000FF",
                            strokeOpacity: 0.8,
                            strokeWeight: 2
                        });
                        flightPath.setMap(_map);
                        if ($("#chkshowwithCustomer").prop('checked') == true) {
                            for (var i = 0; i < _customerPositionArrayWithEmployee.length; i++) {
                                var custMarker = new google.maps.Marker({
                                    position: new google.maps.LatLng(_customerPositionArrayWithEmployee[i].lat, _customerPositionArrayWithEmployee[i].long),
                                    map: _map,
                                    icon: "/images/img/Home-321.png",
                                    title: _customerPositionArrayWithEmployee[i].title
                                });
                                _custMarkers.push(custMarker);
                            }
                        }

                    }
                    else {
                        alert("No Location Data");
                        for (var i = 0; i < _markers.length; i++) {
                            _markers[i].setMap(null);
                        }
                        for (var i = 0; i < _custMarkers.length; i++) {
                            _custMarkers[i].setMap(null);
                        }
                    }
                }
            });

        }
        else if (selectedOpation == 'stoppoint') {

            $.ajax({
                type: "POST",
                url: "/Map/GetStopPointsForEmployee",
                data: { employeeID: employeeID, fromTime: fromTime, endTime: endTime, date: date },
                dataType: "json",
                success: function (response) {
                    if (response.length > 0) {

                        _map = new google.maps.Map(document.getElementById('mapMainDiv'), {
                            zoom: 10,
                            center: new google.maps.LatLng(response[0].Lat, response[0].Long),
                            mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                        });
                        var marker, i;
                        for (i = 0; i < response.length; i++) {
                            marker = new google.maps.Marker({
                                position: new google.maps.LatLng(response[i].Lat, response[i].Long),
                                map: _map,
                                icon: "/images/img/employee_1_stop.png",
                                title: response[i].LastName + " " + response[i].GpsTime.Hours + ":" + response[i].GpsTime.Minutes + "  " + response[i].StopTime.Hours + ":" + response[i].StopTime.Minutes
                            });
                            _markers.push(marker);
                        }
                        if ($("#chkshowwithCustomer").prop('checked') == true) {
                            for (var i = 0; i < _customerPositionArrayWithEmployee.length; i++) {
                                var custMarker = new google.maps.Marker({
                                    position: new google.maps.LatLng(_customerPositionArrayWithEmployee[i].lat, _customerPositionArrayWithEmployee[i].long),
                                    map: _map,
                                    icon: "/images/img/Home-321.png",
                                    title: _customerPositionArrayWithEmployee[i].title
                                });
                                _custMarkers.push(custMarker);
                            }
                        }
                    }
                    else {
                        alert("No Location Data");
                        for (var i = 0; i < _markers.length; i++) {
                            _markers[i].setMap(null);
                        }
                        for (var i = 0; i < _custMarkers.length; i++) {
                            _custMarkers[i].setMap(null);
                        }
                    }
                }
            });

        }
        else if (selectedOpation == 'lastpoint') {
            $.ajax({
                type: "POST",
                url: "/Map/GetLastPointForEmployee",
                data: { employeeID: employeeID, fromTime: fromTime, endTime: endTime, date: date },
                dataType: "json",
                success: function (response) {
                    if (response != false) {
                        var mapOptions = {
                            zoom: 14,
                            center: new google.maps.LatLng(response.Lat, response.Long),
                            mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                        };
                        _map = new google.maps.Map(document.getElementById("mapMainDiv"), mapOptions);
                        var marker = new google.maps.Marker({
                            position: new google.maps.LatLng(response.Lat, response.Long),
                            map: _map,
                            icon: "/images/img/employee_1_stop.png",
                            title: response.LastName + " " + response.GpsTime.Hours + ":" + response.GpsTime.Minutes + "  " + response.StopTime.Hours + ":" + response.StopTime.Minutes
                        });
                        _markers.push(marker);
                        if ($("#chkshowwithCustomer").prop('checked') == true) {
                            for (var i = 0; i < _customerPositionArrayWithEmployee.length; i++) {
                                var custMarker = new google.maps.Marker({
                                    position: new google.maps.LatLng(_customerPositionArrayWithEmployee[i].lat, _customerPositionArrayWithEmployee[i].long),
                                    map: _map,
                                    icon: "/images/img/Home-321.png",
                                    title: _customerPositionArrayWithEmployee[i].title
                                });
                                _custMarkers.push(custMarker);
                            }
                        }

                    } else {
                        alert("No Location Data");
                        for (var i = 0; i < _markers.length; i++) {
                            _markers[i].setMap(null);
                        }
                        for (var i = 0; i < _custMarkers.length; i++) {
                            _custMarkers[i].setMap(null);
                        }
                    }
                }
            });
        }
    } else {
        alert("Must select a employee");
    }
}

// To show only time when focus on from time and end time textboxes.
function ShowTime(obj) {
    $('#' + obj.id + '').timepicker({ 'timeFormat': 'h:i A' });

    if ($('#' + obj.id + '').val() == null || $('#' + obj.id + '').val() == undefined) {

    }
}

// To convert time in 24 hours formet
function TimeParseExact(time) {
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

// To flip the div and show customer tab 
function ShowCustomer() {
    $('#searchDiv').css('display', 'none');
    $('#searchdivCustomer').css('display', 'block');
}

// To flip the div and show employee tab 
function ShowEmployee() {
    $('#searchdivCustomer').css('display', 'none');
    $('#searchDiv').css('display', 'block');
}


// To getting all the states that are in the logged user country 
function GetAllStatesByCountry() {
    _stateNames = [];
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
                _stateNames.push(this.StateDesc);
                _stateIds.push(this.StateCode);
            });
            $("#ddlstateinputcustomer").autocomplete({
                source: _stateNames,
            });
        },
    });
}

// To getting all the citys by selected state.
function GetCitysByState() {
    if (_stateIds[_stateNames.indexOf($('#ddlstateinputcustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Data/GetAllCitysByState",
        data: { stateID: _stateIds[_stateNames.indexOf($('#ddlstateinputcustomer').val())] },
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
                    _abliableDataForCityesName.push(this.CityDesc);
                    _abliableDataForCityesIds.push(this.CityCode);
                });
            }
            $("#ddlcityinputCustomer").autocomplete({
                source: _abliableDataForCityesName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

// To getting streets by selected city
function GetStreetByCity() {
    if (_abliableDataForCityesIds[_abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Data/GetAllStreetByCity",
        data: { cityID: _abliableDataForCityesIds[_abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                _abliableDataForStreetName = [];
                _abliableDataForStreetId = [];
                $(response).each(function () {
                    $("<option />", {
                        val: this.StreetCode,
                        text: this.Streetdesc
                    }).appendTo($('#ddlstreetinputCustomer'));
                    _abliableDataForStreetName.push(this.Streetdesc);
                    _abliableDataForStreetId.push(this.StreetCode);
                });
                $('#ddlstreetinputCustomer').removeAttr("disabled");
            }
            $("#ddlstreetinputCustomer").autocomplete({
                source: _abliableDataForStreetName,
            });
        },
    });
}

// To getting all buildings by selected city
function GetBuildingsByCity() {
    abliableDataForBuilding = [];
    if (_abliableDataForStreetId[_abliableDataForStreetName.indexOf($('#ddlstreetinputCustomer').val())] == undefined || _abliableDataForCityesIds[_abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Data/GetAllBuildingsByCity",
        data: { streetID: _abliableDataForStreetId[_abliableDataForStreetName.indexOf($('#ddlstreetinputCustomer').val())], cityID: _abliableDataForCityesIds[_abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {

                $(response).each(function () {
                    _abliableDataForBuildingNumber = [];
                    _abliableDataForBuildingId = [];
                    _abliableDataForBuildingLat = [];
                    _abliableDataForBuildingLong = [];
                    $("<option />", {
                        val: this.BuildingCode,
                        text: this.BuildingNumber
                    }).appendTo($('#ddlbuildinginputCustomer'));
                    _abliableDataForBuildingNumber.push(this.BuildingNumber);
                    _abliableDataForBuildingId.push(this.BuildingCode);
                    _abliableDataForBuildingLat.push(this.BuildingLat);
                    _abliableDataForBuildingLong.push(this.BuldingLong);
                });
                $('#ddlbuildinginputCustomer').removeAttr("disabled");
                $("#ddlbuildinginputCustomer").autocomplete({
                    source: _abliableDataForBuildingNumber,
                });
                $('#ddlbuildinginputCustomer').val(_abliableDataForBuildingNumber);
                GetSelectedBuildingLatLong();
            }
        },
    });
}

// To getting latittude and longitude of selected building 
function GetSelectedBuildingLatLong() {
    if (_abliableDataForBuildingLat[_abliableDataForBuildingNumber.indexOf($('#ddlbuildinginputCustomer').val())] == undefined, _abliableDataForBuildingLong[_abliableDataForBuildingNumber.indexOf($('#ddlbuildinginputCustomer').val())] == undefined)
        return false;
    _buildingLatLong = [];
    _buildingLatLong.push(_abliableDataForBuildingLat[_abliableDataForBuildingNumber.indexOf($('#ddlbuildinginputCustomer').val())], _abliableDataForBuildingLong[_abliableDataForBuildingNumber.indexOf($('#ddlbuildinginputCustomer').val())]);
}


// To search customers by selected state,city,street,building,customernumber and companyname.
function SearchCustomers() {
    var state, city, street, building, customerNumber, companyName;
    customerNumber = $('#txtcustomernoInput').val();
    companyName = $('#txtcompanynameInputCustomer').val();
    buildingNumber = $('#ddlbuildinginputCustomer').val();

    if ($('#ddlcityinputCustomer').val() == "") {
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
                            $("#tblmapsearchgridCustomer").append("<tr class='customerRow' id='" + response[i].CustomerId + "' rel='" + response[i].LastName + "' FirstName='" + response[i].FirstName + "'><td class='tg-dx8v'><input type='checkbox' class='chk' name='chkCustomer' onclick='ChkcustomerChange(this)'/></td><td class='tg-dx8v'>" + response[i].CustomerId + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].CityName + "</td><td class='tg-dx8v'>" + response[i].StreetName + "</td></tr>");
                        }
                    }
                }
            }
        })
    }
    else {
        if (_abliableDataForStreetId[_abliableDataForStreetName.indexOf($('#ddlstreetinputCustomer').val())] != undefined && _stateIds[_stateNames.indexOf($('#ddlstateinputcustomer').val())] != undefined && _abliableDataForCityesIds[_abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())] != undefined && _abliableDataForBuildingId[_abliableDataForBuildingNumber.indexOf($('#ddlbuildinginputCustomer').val())] != undefined) {
            $.ajax({
                type: "POST",
                url: "/Map/GetCustomersForMap",
                data: { state: _stateIds[_stateNames.indexOf($('#ddlstateinputcustomer').val())], city: _abliableDataForCityesIds[_abliableDataForCityesName.indexOf($('#ddlcityinputCustomer').val())], street: _abliableDataForStreetId[_abliableDataForStreetName.indexOf($('#ddlstreetinputCustomer').val())], buildingNumber: buildingNumber, customerNumber: customerNumber, companyName: companyName },
                dataType: "json",
                success: function (response) {
                    $("#tblmapsearchgridCustomer").html('');
                    if (response.length > 0) {
                        if (response != null) {
                            for (var i = 0; i < response.length; i++) {
                                $("#tblmapsearchgridCustomer").append("<tr class='customerRow' id='" + response[i].CustomerId + "' rel='" + response[i].LastName + "' FirstName='" + response[i].FirstName + "'><td class='tg-dx8v'><input type='checkbox' class='chk' name='chkCustomer' onclick='ChkcustomerChange(this)'/></td><td class='tg-dx8v'>" + response[i].CustomerId + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].CityName + "</td><td class='tg-dx8v'>" + response[i].StreetName + "</td></tr>");
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
        else {
            $("#tblmapsearchgridCustomer").html('');
            $("#selectedCustomer").html('');
            alert('No Records Founded');
        }
    }
};

// To show selected customers on map 
function ShowCustomerDataOnMap() {
    _checkedCustomersforMap = '';
    GetSelectedCustomersIdsForMap();
    if (_checkedCustomersforMap != '') {
        $.ajax({
            type: "POST",
            url: "/Map/GetCustomerForMapByCustomerID",
            data: { checkedcustomers: _checkedCustomersforMap },
            dataType: "json",
            success: function (response) {
                if (response != null || response[0].Lat != null) {
                    _customerPositionArrayWithEmployee = [];
                    if (response[0].Lat != null && response[0].Lat != null) {
                        var map = new google.maps.Map(document.getElementById('mapMainDiv'), {
                            zoom: 8,
                            center: new google.maps.LatLng(response[0].Lat, response[0].Long),
                            mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                        });
                    }
                    else {
                        var map = new google.maps.Map(document.getElementById('mapMainDiv'), {
                            zoom: 8,
                            center: _liverpool,
                            mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
                        });
                    }
                    var custMarker, i;
                    for (i = 0; i < response.length; i++) {
                        _custTitle = response[i].FirstName + " " + response[i].LastName;
                        if (response[i].Lat != null && response[i].Long != null) {
                            _customerPositionArrayWithEmployee.push({ lat: response[i].Lat, long: response[i].Long, title: response[i].FirstName + " " + response[i].LastName });
                            custMarker = new google.maps.Marker({
                                position: new google.maps.LatLng(response[i].Lat, response[i].Long),
                                map: map,
                                icon: "/images/img/Home-321.png",
                                title: _custTitle
                            });
                        }
                        _custMarkers.push(custMarker);
                    }
                }
            }

        })
    }
    else {
        alert("Must Select a Customer");
        for (var i = 0; i < _custMarkers.length; i++) {
            _custMarkers[i].setMap(null);
        }
    }
}

// To adding selected employee in selected employee div 
function AddToSelectedEmployeeDiv(obj) {
    $("#selectedemployeeDiv").html('');
    $("#selectedemployeeDiv").append("<tr><td>" + obj.attr('id') + " </td><td>" + obj.attr('rel') + " </td></tr>");
    $("#selectedemployeeDiv").css('display', 'block');

}

// To adding selected customers in selected customers div
function AddToSelectedCustomerDiv(obj) {
    $("#selectedCustomer").html('');
    $("#selectedCustomer").append("<tr id=" + obj.attr('id') + "><td>" + obj.attr('id') + " </td><td>" + obj.attr('FirstName') + " </td><td>" + obj.attr('rel') + " </td></tr>");
    $("#selectedCustomer").css('display', 'block');
}

// To adding multipal customers in selected customers div by check box checked 
function ChkcustomerChange(obj) {
    debugger;
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

// To adding selected customers id in array so can get all data by customers ids
function GetSelectedCustomersIdsForMap() {
    if ($('#selectedCustomer tr').length > 0) {
        for (var i = 0, l = $('#selectedCustomer tr').length; i < l; i++) {
            _checkedCustomersforMap += $('#selectedCustomer tr')[i].id + ",";
        }
    }
}


// To showing loggend employee on map with last point of current date.
function ShowEmployeeById(employeeID) {
    $.ajax({
        type: "POST",
        url: "/Map/GetEmployeeByIdOnLoad",
        data: { employeeID: employeeID },
        dataType: "json",
        success: function (response) {
            $("#tblmapsearchgridEmployee").html('');
            if (response != null) {
                for (var i = 0; i < response.length; i++) {
                    $("#tblmapsearchgridEmployee").append("<tr rel='" + response[i].LastName + "' id='" + response[i].EmployeeID + "' class='active'><td class='tg-dx8v'>" + response[i].Number + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].FirstName + "</td></tr>");
                }
                ShowEmployeeDataOnMap();
            }
        },

    });
}

function showCustomerById(customerID) {
    $.ajax({
        type: "POST",
        url: "/Map/GetCustomerByIdOnLoad",
        data: { customerID: customerID },
        dataType: "json",
        success: function (response) {
            if (response.length > 0) {
                $("#tblmapsearchgridCustomer").html('');
                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        $("#tblmapsearchgridCustomer").append("<tr class='customerRow active' id='" + response[i].CustomerId + "' rel='" + response[i].LastName + "' FirstName='" + response[i].FirstName + "' ><td class='tg-dx8v'><input type='checkbox' class='chk' name='chkCustomer' onclick='ChkcustomerChange(this)'/></td><td class='tg-dx8v'>" + response[i].CustomerId + "</td><td class='tg-dx8v'>" + response[i].LastName + "</td><td class='tg-dx8v'>" + response[i].CityName + "</td><td class='tg-dx8v'>" + response[i].StreetName + "</td></tr>");
                    }
                    $("#tblmapsearchgridCustomer tr").find('.chk').click();
                    ShowCustomerDataOnMap();
                }
            }
        }
    })
}


