//Employee Section
function ManufactureTypes(obj) {
    $('#ddlphoneType').empty();
  //  $("<option value=''/>");
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

var statesArray = []
var cityArray = []
var streetArray = []
var buildingArray = []
var buildinCode = 0;


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

//Will work on arrary of type [{id:1,name:abc},{id:2,name:abtc}]
function GetIdByName(arr, name) {
    var item = $.grep(arr, function (v) { return v.name === name; })
    if (item.length > 0) {
        return item[0].id;
    } else {
        return 0;
    }
}

function GetNameById(arr, id) {
    var item = $.grep(arr, function (v) { return v.id === id; });
    if (item.length > 0) {
        return item[0].name;
    } else {
        return 0;
    }
}

function GetAllStatesByCountry() {
    stateNames = [];
    $.ajax({
        type: "POST",
        url: "/Admin/GetAllStatesByCountry",
        success: function (response) {
            var appElement = document.querySelector('[ng-controller=SearchCtrl]');
            var $scope = angular.element(appElement).scope();
            $scope.$apply(function () {
                if (response.length <= 1) {
                    $scope.HasStateActive = "true";
                    $('#inputState').prop("disabled", true)
                } else {
                    $scope.HasStateActive = "false";
                    $('#inputState').prop("disabled", false)
                }
            });
            $(response).each(function () {
                stateNames.push(this.StateDesc);
                stateIds.push(this.StateCode);
                statesArray.push({ id: this.StateCode, name: this.StateDesc })
            });

            if (response.length <= 1) {
                GetAllCitysByState(response[0].StateDesc);
            }
            $("#inputState").autocomplete({
                source: stateNames,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    GetAllCitysByState(ui.item.label);
                }
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}


function GetAllCitysByState(state) {
    if (state == undefined) {
        state = '';
    }
    if (GetIdByName(statesArray, state) == 0) {
        $('#inputCity').val('');
        $('#inputStreet').val('');
        $('#inputBuldingNumber').val('');

        return false;
    }

    $.ajax({
        type: "POST",
        url: "/Admin/GetAllCitysByState",
        data: { stateID: GetIdByName(statesArray, state) },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                cityArray = [];

                $(response).each(function () {
                    availableCityName.push(this.CityDesc);
                    availableCityIds.push(this.CityCode);
                    cityArray.push({ id: this.CityCode, name: this.CityDesc })
                });
            }
            $("#inputCity").autocomplete({
                source: availableCityName,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    GetAllStreetByCity(ui.item.label);
                }
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}


function GetAllStreetByCity(city) {
    if (GetIdByName(cityArray, city) == 0) {

        streetArray = [];
        $('#inputStreet').val('');
        $('#inputBuldingNumber').val('');

        return false;
    }

    $.ajax({
        type: "POST",
        url: "/Admin/GetAllStreetByCity",
        data: { cityID: GetIdByName(cityArray, city) },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                $(response).each(function () {
                    availableStreetName.push(this.Streetdesc);
                    availableStreetId.push(this.StreetCode);
                    streetArray.push({ id: this.StreetCode, name: this.Streetdesc })
                });
            }
            $("#inputStreet").autocomplete({
                source: availableStreetName,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    GetAllBuildingsByCity(ui.item.label, $('#inputCity').val());
                }
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}



function GetAllBuildingsByCity(street, city) {
    availableBuilding = [];
    //if ( || stateIds[stateNames.indexOf($('#inputState').val())] == undefined || availableCityIds[availableCityName.indexOf($('#inputCity').val())] == undefined)
    //    return false;
    if (GetIdByName(streetArray, street) == 0 || GetIdByName(cityArray, city) == 0) {
        return false;
    }

    $.ajax({
        type: "POST",
        url: "/Admin/GetAllBuildingsByCity",
        data: { streetID: GetIdByName(streetArray, street), cityID: GetIdByName(cityArray, city) },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                $(response).each(function () {
                    buildingArray.push({ id: this.BuildingCode, name: this.BuildingNumber, lat: this.BuildingLat, long: this.BuldingLong })
                    if (availableBuildingNumber.indexOf(this.BuildingNumber.trim()) == -1) {
                        availableBuildingNumber.push(this.BuildingNumber.trim());
                    }
                    availableBuildingId.push(this.BuildingCode);
                    availableBuildingLat.push(this.BuildingLat);
                    availableBuildingLong.push(this.BuldingLong);
                });
            }
            $("#inputBuldingNumber").autocomplete({
                source: availableBuildingNumber,
            });
            //  GetSelectedBuildingLatLong();
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}


//Unused Method
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
    buildinCode = 0;
    var appElement = document.querySelector('[ng-controller=SearchCtrl]');
    var $scope = angular.element(appElement).scope();


    var visitTime, visitInterval, nextVisit;


    visitTime = $('#inputZipCode').val();

    visitInterval = $('#inputVisitTime').val();
    nextVisit = $('#inputNextVisit').val();


    //Address Data
    var stateId = GetIdByName(statesArray, $('#inputState').val());
    var cityId = GetIdByName(cityArray, $('#inputCity').val());
    var streetId = GetIdByName(streetArray, $('#inputStreet').val());

    var state = $('#inputState').val();
    var city = $('#inputCity').val();
    var street = $('#inputStreet').val();

    var buildingNumber = $('#inputBuldingNumber').val();
    var zipCode = $('#inputEntry').val();
    var entry = $('#inputVisitInterval').val();
    //Address Data End

    if (stateId == 0 || buildingNumber == '' || street == '' || city == '') {
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
        data: { stateID: stateId, cityID: cityId, streetID: streetId, buildingNumber: buildingNumber, entry: entry, zipCode: zipCode, state: state, city: city, street: street },
        success: function (response) {
            if (response.IsSuccess == true) {
                if (response.IsOpenMap == true) {
                    $("#MapHeaderGrid").html("<tr><th class='tg-z1n2'>Country</th><th class='tg-z1n2'>State</th> <th class='tg-z1n2'>City</th>  <th class='tg-z1n2'>Street</th> <th class='tg-z1n2'>Building Number</th><th class='tg-z1n2'>Full Address</th></tr>");
                    for (var i = 0; i < response.Result.ServiceResponseAddresses.length; i++) {
                        var currentAddress = response.Result.ServiceResponseAddresses[i];
                        $("#MapHeaderGrid").append("<tr data-state='" + currentAddress.State + "' data-streetdesc='" + currentAddress.Street + "'  data-citydesc='" + currentAddress.City + "'  data-building='" + currentAddress.Building + "' data-latitude='" + currentAddress.Latitude + "' data-longitude='" + currentAddress.Longitude + "'><td class='tg-dx8v'>" + currentAddress.Country + "</td><td class='tg-dx8v'>" + currentAddress.State + "</td><td class='tg-dx8v'>" + currentAddress.City + "</td><td class='tg-dx8v'>" + currentAddress.Street + "</td><td class='tg-dx8v'>" + currentAddress.Building + "</td><td class='tg-dx8v'>" + currentAddress.Country + "," + currentAddress.State + "," + currentAddress.City + "," + currentAddress.Street + "," + currentAddress.Building + "</td></tr>");
                    }
                    $("#popup_div").dialog("open");
                    LoadMapByFactoryID();
                } else {
                    buildingCode = response.BuildingCode;
                    $scope.$apply(function () {
                        $scope.ShowMessageBox('Message', 'Address change was successful.Save customer information to comlete the update.')
                    });
                }
            } else {
                $scope.$apply(function () {
                    $scope.ShowMessageBox('Message',response.ErrorMessage)
                });
            }
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });

    // if (IsOpenMap) {
    //if (stateID != "" || cityID != "" || streetID != "") {
    //    $("#popup_div").dialog("open");
    //    if (buildingLatLong[0] == null || buildingLatLong[1] == undefined) {
    //  LoadMapByFactoryID();
    //}
    //else {
    //    Initialize(buildingLatLong);
    //}
    //}
}

function GetAddressBuildingCode(state, citydesc, city, street, streetdesc, number, Lat, Long, entry, zipcode) {
    var appElement = document.querySelector('[ng-controller=SearchCtrl]');
    var $scope = angular.element(appElement).scope();
    var buildingCode = 0;
    $.ajax({
        url: "/Admin/GetAddressBuildingCode",
        data: { state: state, citydesc: citydesc, city: city, street: street, streetdesc: streetdesc, number: number, Lat: Lat, Long: Long, entry: entry, zipcode: zipcode },
        success: function (response) {
            if (parseInt(response.BuildingCode) > 0) {
                //Set Address Values
                $scope.$apply(function () {
                    $scope.ShowMessageBox('Message', 'Address change was successful.Save customer information to comlete the update')
                });
                buildingCode = parseInt(response.BuildingCode);
            } else if (parseInt(response.BuildingCode) == -1) {
                $scope.$apply(function () {
                    $scope.ShowMessageBox('Message', 'Error in the change of the address')
                });
            }
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });

    return buildingCode;
}

function OnInsertAddressOkClick() {
    var appElement = document.querySelector('[ng-controller=SearchCtrl]');
    var $scope = angular.element(appElement).scope();

    var selectedRow = $('#MapHeaderGrid tr.active')
    if (selectedRow.length > 0) {
        var statedesc = selectedRow.data('state');
        var state = GetIdByName(statesArray, statedesc);
        var citydesc = selectedRow.data('citydesc');
        var city = GetIdByName(cityArray, citydesc);
        var streetdesc = selectedRow.data('streetdesc');
        var street = GetIdByName(streetArray, streetdesc);
        var number = selectedRow.data('building')
        var Lat = selectedRow.data('latitude')
        var Long = selectedRow.data('longitude')
        var zipcode = $('#inputEntry').val();
        var entry = $('#inputVisitInterval').val();

        if (streetdesc == '' || citydesc == '' || number == '') {
            $scope.$apply(function () {
                $scope.ShowMessageBox('Message', 'Must select Street,City and Building Number.')
            });
            return false;
        }

        //Get Building Code 
        buildingCode = GetAddressBuildingCode(state, citydesc, city, street, streetdesc, number, Lat, Long, entry, zipcode)
        closeDialog();

    } else {
        $scope.$apply(function () {
            $scope.ShowMessageBox('Message', 'Must select Street,City and Building Number.')
        });
    }

}

function closeDialog() {
    $("#popup_div").dialog("close");
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
            tableClickedLatLong.push(($(this).data('latitude')), ($(this).data('longitude')));
            Initialize(tableClickedLatLong);
            $('#inputBuldingNumber').val(($(this).data('building')));
        }
    });

    GetAllStatesByCountry();
});





//End Tree View Section