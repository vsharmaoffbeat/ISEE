
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
    LoadMapByFactoryID();
    stateNames = [];
    stateIds = [];
    statesArray = [];
    $.ajax({
        type: "POST",
        url: "/Data/GetAllStatesByCountry",
        success: function (response) {
            //var appElement = document.querySelector('[ng-controller=SearchCtrl]');
            //var $scope = angular.element(appElement).scope();
            //$scope.$apply(function () {
            if (response.length <= 1) {
                //   $scope.HasStateActive = "true";
                $('#inputState').prop("disabled", true)
            } else {
                // $scope.HasStateActive = "false";
                $('#inputState').prop("disabled", false)
            }
            //});
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
    availableCityName = [];
    availableCityIds = [];
    cityArray = [];
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
        url: "/Data/GetAllCitysByState",
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
        url: "/Data/GetAllStreetByCity",
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
        url: "/Data/GetAllBuildingsByCity",
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

function LoadMapByFactoryID() {
    $.ajax({
        url: "/Data/GetCurrentLogedUserCountery", success: function (result) {
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