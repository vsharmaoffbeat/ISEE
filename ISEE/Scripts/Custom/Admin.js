//Employee Section
function ManufactureTypes(obj) {
    $('#ddlphoneType').empty();
    $.ajax({
        type: "POST",
        url: "/Admin/GetPhoneTypes",
        data: { id: parseInt($('#ddlmanufacture :selected').val()) },
        dataType: "json",
        success: function (response) {
            $(response).each(function () {
                $("<option />", {
                    val: this.PhoneTypeCode,
                    text: this.PhoneTypeDesc
                }).appendTo($('#ddlphoneType'));
            });
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}


var stateNames = [];
var stateIds = [];
var availableCityName = [];
var availableCityIds = [];
var availableStreetName = [];
var availableStreetId = [];
var availableBuildingNumber = [];
var availableBuildingId = [];
var availableBuildingLat = [];
var availableBuildingLong = [];


function GetAllStatesByCountry() {
    stateNames = [];
    $.ajax({
        type: "POST",
        url: "/Admin/GetAllStatesByCountry",
        success: function (response) {
            var appElement = document.querySelector('[ng-controller=SearchCtrl]');
            var $scope = angular.element(appElement).scope();
            $scope.$apply(function () {
                if (response.length == 1) {
                    $scope.HasStateActive = "false";
                } else {
                    $scope.HasStateActive = "true";
                }
            });
            $(response).each(function () {
                $("<option />", {
                    val: this.StateCode,
                    text: this.StateDesc
                }).appendTo($('#inputState'));
                stateNames.push(this.StateDesc);
                stateIds.push(this.StateCode);
            });

            if (response.length <= 1) {
                _selChangeStateCode = response[0].StateCode
                $("#inputState option[value='" + response[0].StateCode + "']").prop("selected", true);
                GetAllCitysByState();
            }
            $("#inputState").autocomplete({
                source: stateNames,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}


function GetAllCitysByState() {

    if (stateIds[stateNames.indexOf($('#inputState').val())] == undefined)
        return false;

    _selChangeStateCode = 0;
    _selChangeCityCode = 0;
    _selChangeStreetCode = 0;

    _selChangeStateCode = $('#inputState').val();

    $.ajax({
        type: "POST",
        url: "/Admin/GetAllCitysByState",
        data: { stateID: stateIds[stateNames.indexOf($('#inputState').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                $(response).each(function () {
                    $("<option />", {
                        val: this.CityCode,
                        text: this.CityDesc
                    }).appendTo($('#inputCity'));
                    availableCityName.push(this.CityDesc);
                    availableCityIds.push(this.CityCode);
                });
            }
            $("#inputCity").autocomplete({
                source: availableCityName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}


function GetAllStreetByCity() {
    if (stateIds[stateNames.indexOf($('#inputState').val())] == undefined || availableCityIds[availableCityName.indexOf($('#inputCity').val())] == undefined)
        return false;

    _selChangeStreetCode = 0;
    _selChangeCityCode = 0;

    _selChangeCityCode = $('#inputCity').val();

    $.ajax({
        type: "POST",
        url: "/Admin/GetAllStreetByCity",
        data: { cityID: availableCityIds[availableCityName.indexOf($('#inputCity').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                $(response).each(function () {
                    $("<option />", {
                        val: this.StreetCode,
                        text: this.Streetdesc
                    }).appendTo($('#inputStreet'));
                    availableStreetName.push(this.Streetdesc);
                    availableStreetId.push(this.StreetCode);
                });
            }
            $("#inputStreet").autocomplete({
                source: availableStreetName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}



function GetAllBuildingsByCity() {
    availableBuilding = [];
    if (availableStreetId[availableStreetName.indexOf($('#inputStreet').val())] == undefined || stateIds[stateNames.indexOf($('#inputState').val())] == undefined || availableCityIds[availableCityName.indexOf($('#inputCity').val())] == undefined)
        return false;

    _selChangeStreetCode = $('#inputStreet').val();

    $.ajax({
        type: "POST",
        url: "/Admin/GetAllBuildingsByCity",
        data: { streetID: availableStreetId[availableStreetName.indexOf($('#inputStreet').val())], cityID: availableCityIds[availableCityName.indexOf($('#inputCity').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                $(response).each(function () {
                    $("<option />", {
                        val: this.BuildingCode,
                        text: this.BuildingNumber
                    }).appendTo($('#inputBuldingNumber'));
                    availableBuildingNumber.push(this.BuildingNumber);
                    availableBuildingId.push(this.BuildingCode);
                    availableBuildingLat.push(this.BuildingLat);
                    availableBuildingLong.push(this.BuldingLong);
                });
            }
            $("#inputBuldingNumber").autocomplete({
                source: availableBuildingNumber,
            });
            $('#inputBuldingNumber').val(availableBuildingNumber);
            GetSelectedBuildingLatLong();
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}
var buildingLatLong = [];
function GetSelectedBuildingLatLong() {
    if (availableBuildingLat[availableBuildingNumber.indexOf($('#inputBuldingNumber').val())] == undefined, availableBuildingLong[availableBuildingNumber.indexOf($('#inputBuldingNumber').val())] == undefined)
        return false;
    buildingLatLong = [];
    buildingLatLong.push(availableBuildingLat[availableBuildingNumber.indexOf($('#inputBuldingNumber').val())], availableBuildingLong[availableBuildingNumber.indexOf($('#inputBuldingNumber').val())]);
}



//End Employee Section

function initializeStreatview(obj) {
    var fenway = { lat: obj[0].Lat, lng: obj[0].Long };
    var map = new google.maps.Map(document.getElementById("map_canvas"), {
        center: fenway,
        zoom: 14
    });
    var panorama = new google.maps.StreetViewPanorama(
        document.getElementById("map_canvas"), {
            position: fenway,
            pov: {
                heading: 34,
                pitch: 10
            }
        });
    map.setStreetView(panorama);
}


var _selChangeStateCode = 0;
var _selChangeCityCode = 0;
var _selChangeStreetCode = 0;

function InsertAddress() {
    var stateID, cityID, streetID, buildingNumber, zipCode, visitTime, entry, visitInterval, nextVisit;
    buildingNumber = $('#inputBuldingNumber').val();
    zipCode = $('#inputEntry').val();
    visitTime = $('#inputZipCode').val();
    entry = $('#inputVisitInterval').val();
    visitInterval = $('#inputVisitTime').val();
    nextVisit = $('#inputNextVisit').val();

    stateID = $('#inputState').val();
    cityID = $('#inputCity').val();
    streetID = $('#inputStreet').val();



    if (cityID == '' || streetID == '' || buildingNumber == '') {
        var appElement = document.querySelector('[ng-controller=SearchCtrl]');
        var $scope = angular.element(appElement).scope();
        $scope.$apply(function () {
            $scope.ShowMessageBox('Message', 'Must select address first.')
        });

        return false;
    }

    var num;

    if (buildingNumber != undefined && buildingNumber != '')
        num = buildingNumber.trim();

    $.ajax({
        url: "/Admin/InsertAddress",
        data: { stateID: stateID, cityID: cityID, streetID: streetID },
        success: function (response) {
            $("#MapHeaderGrid").html("<tr><th class='tg-z1n2'>Country</th><th class='tg-z1n2'>State</th> <th class='tg-z1n2'>City</th>  <th class='tg-z1n2'>Street</th> <th class='tg-z1n2'>Building Number</th><th class='tg-z1n2'>Full Address</th></tr>");
            for (var i = 0; i < response.length; i++) {
                $("#MapHeaderGrid").append("<tr id='" + response[i].Number + "' class='" + response[i].Lat + "' rel='" + response[i].Long + "'><td class='tg-dx8v'>" + response[i].CountryName + "</td><td class='tg-dx8v'>" + response[i].StateName + "</td><td class='tg-dx8v'>" + response[0].CityName + "</td><td class='tg-dx8v'>" + response[0].StreetName + "</td><td class='tg-dx8v'>" + response[0].BuldingNumber + "</td><td class='tg-dx8v'>" + response[i].CountryName + "," + response[i].StateName + "," + response[i].CityName + "," + response[i].StreetName + "," + response[0].BuldingNumber + "</td></tr>");
            }
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });

    if (stateID != "" || cityID != "" || streetID != "") {
        $("#popup_div").dialog("open");
        if (buildingLatLong[0] == null || buildingLatLong[1] == undefined) {
            LoadMapByFactoryID();
        }
        else {
            Initialize(buildingLatLong);
        }
    }
}


function LoadMapByFactoryID() {
    $.ajax({
        url: "/Admin/GetCurrentLogedUserCountery", success: function (result) {
            google.maps.visualRefresh = true;
            var Liverpool = new google.maps.LatLng(result[0].Lat, result[0].Long);
            var mapOptions = {
                zoom: 14,
                center: Liverpool,
                mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
            };
            var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
        }
    });
}

function Initialize(obj) {
    google.maps.visualRefresh = true;
    var Liverpool = new google.maps.LatLng(obj[0], obj[1]);
    var mapOptions = {
        zoom: 14,
        center: Liverpool,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };
    var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
    var marker = new google.maps.Marker({
        position: Liverpool,
        map: map,
    });
};



//function SaveCustomerForm() {
//    var buildingNumber = $('#inputBuldingNumber').val();
//    debugger;


//    var data = {
//        //inputCompanyName: $('#inputCompanyName').val(),                                     

//        //inputMobile: $('#inputMobile').val(),                     
//        //state: $('#inputState').val(),
//        //city: $('#inputCity').val(),
//        //street: $('#inputStreet').val(),                       
//        //zipCode: $('#inputZipCode').val(),
//        //visitTime: $('#inputVisitTime').val(),
//        //entry: $('#inputEntry').val(),

//        ContactName: $('#inputContactName').val(),
//        inputApartment: $('#inputApartment').val(),
//        Phone2: $('#inputPhone22').val(),
//        Mail: $('#inputMail').val(),
//        Fax: $('#inputFax').val(),
//        CustomerNumber: $('#inputCustomerNumber').val(),
//        Floor: $('#inputFloor').val(),
//        Phone1: $('#inputPhone11').val(),
//        inputPhoneArea1: $('#inputPhoneOne').val(),
//        inputPhoneArea2: $('#inputPhoneTwo').val(),
//        VisitInterval: $('#inputVisitInterval').val(),
//        VisitTime: $('#inputVisitTime').val(),
//        NextVisit: $('#inputNextVisit').val(),
//        buldingCode: (availableBuildingId[availableBuildingNumber.indexOf(buildingNumber)]),
//    };
//    if (buildingNumber != "") {
//        if ($('#inputCompanyName').val() != '') {
//            debugger;
//            $.ajax({
//                url: "/Admin/SaveCustomerForm",
//                type: "post",
//                contentType: "application/json",
//                data: JSON.stringify({ objCustomerData: data }),
//                dataType: "json",
//                success: function (result) {
//                    debugger;
//                }
//            });
//            return false;
//        }
//    }
//}
//End Customer section

//Start Tree View Section

//this will hold reference to the tr we have dragged and its helper
var c = {};

var tableClickedLatLong = [];
$(document).ready(function () {
    $(document).on('click', '#MapHeaderGrid tr', function () {
        if ($(this).children("th").length == 0) {
            tableClickedLatLong = [];
            debugger;
            $("#MapHeaderGrid tr").removeClass('active');
            $(this).addClass('active');
            tableClickedLatLong.push(($(this).attr('class').split(' ')[0]), ($(this).attr('rel')));
            Initialize(tableClickedLatLong);
            $('#inputBuldingNumber').val(($(this).attr('id')));
        }
    });

    GetAllStatesByCountry();
});



//End Tree View Section