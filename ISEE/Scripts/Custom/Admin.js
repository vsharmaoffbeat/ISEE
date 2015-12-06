var buildingCode = 0;
var _factoryId = -1;
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
            if (response != null) {
                var appElement = document.querySelector('[ng-controller=SearchCtrl]');
                var $scope = angular.element(appElement).scope();
                $scope.employeeInfo.phoneTypeChoice = response[0].PhoneTypeCode.toString()
            }
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


//Country tab impilimations 
var countryId = [];
var countryArray = [];


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
        url: "/Data/GetAllStatesByCountry",
        success: function (response) {
            var appElement = document.querySelector('[ng-controller=SearchCtrl]');
            var $scope = angular.element(appElement).scope();
            $scope.$apply(function () {
                if (response.length <= 1) {
                    $scope.HasStateActive = "true";
                    $('#inputState,#treeState').prop("disabled", true)
                } else {
                    $scope.HasStateActive = "false";
                    $('#inputState,#treeState').prop("disabled", false)
                }
            });
            $(response).each(function () {
                if (stateNames.indexOf(this.StateDesc.trim()) == -1) {
                    stateNames.push(this.StateDesc.trim());
                }
                stateIds.push(this.StateCode);
                statesArray.push({ id: this.StateCode, name: this.StateDesc })
            });

            if (response.length <= 1) {
                GetAllCitysByState(response[0].StateDesc);
            }
            $("#inputState,#treeState").autocomplete({
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
        url: "/Data/GetAllCitysByState",
        data: { stateID: GetIdByName(statesArray, state) },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                cityArray = [];

                $(response).each(function () {
                    if (availableCityName.indexOf(this.CityDesc.trim()) == -1) {
                        availableCityName.push(this.CityDesc.trim());
                    }
                    availableCityIds.push(this.CityCode);
                    cityArray.push({ id: this.CityCode, name: this.CityDesc })
                });
            }
            $("#inputCity,#treeCity").autocomplete({
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
                    if (availableStreetName.indexOf(this.Streetdesc.trim()) == -1) {
                        availableStreetName.push(this.Streetdesc.trim());
                    }
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

            $("#treeStreet").autocomplete({
                source: availableStreetName,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    GetAllBuildingsByCity(ui.item.label, $('#treeCity').val());
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
            $("#inputBuldingNumber,#treeBuldingNumber").autocomplete({
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
        url: "/Data/InsertAddress",
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
                    $scope.ShowMessageBox('Message', response.ErrorMessage)
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
    GetAllCountrys();

    GetAllCompanyDesc();

    //google.maps.event.addListener(map, 'click', function (event) {
    //    debugger;
    //    var myLatLng = event.latLng;
    //    var lat = myLatLng.lat();
    //    var lng = myLatLng.lng();
    //});

    $('#inputCountry_State').prop("disabled", true);
    $("#stateChk").prop('checked', 'checked');

});





//End Tree View Section





//Country tab start
function GetAllCountrys() {
    countryNames = [];
    $.ajax({
        type: "POST",
        url: "/Data/GetAllCountrys",
        success: function (response) {
            var appElement = document.querySelector('[ng-controller=SearchCtrl]');
            var $scope = angular.element(appElement).scope();
            countryArray = [];
            $(response).each(function () {
                if (countryNames.indexOf(this.CountryDesc.trim()) == -1) {
                    countryNames.push(this.CountryDesc.trim());
                }
                countryId.push(this.CountryCode);
                countryArray.push({ id: this.CountryCode, name: this.CountryDesc, NameEN: this.CountryDescEN, UTC: this.CurrentGmt.toString() });
            });

            if (response.length <= 1) {
                GetAllCitysByState(response[0].CountryDesc);
            }
            $("#inputCountry").autocomplete({
                source: countryNames,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    var nameEN = GetNameENByName(countryArray, ui.item.label);
                    var UTC = GetUTCByName(countryArray, ui.item.label);
                    $("#inputCountryEN").val(nameEN);
                    $("#inputUTC").val(UTC);
                    //  GetAllStatesByCountryID(ui.item.label);
                }
            });
            $("#inputCountryDesc").autocomplete({
                source: countryNames,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;

                    // GetExistingCountry(ui.item.label);
                }
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}





function GetAllStatesByCountryID(country) {
    if (country == undefined) {
        country = '';
    }
    if (GetIdByName(countryArray, country) == 0) {
        $('#inputCountry_City').val('');
        $('#inputCountry_Street').val('');

        return false;
    }
    stateNames = [];
    $.ajax({
        type: "POST",
        url: "/Data/GetAllStatesByCountryId",
        data: { countryID: GetIdByName(countryArray, country) },
        dataType: "json",
        success: function (response) {
            var appElement = document.querySelector('[ng-controller=SearchCtrl]');
            var $scope = angular.element(appElement).scope();
            $scope.$apply(function () {
                if (response.length <= 1) {
                    $scope.HasCountry_StateActive = "true";
                    $('#inputCountry_State').prop("disabled", true)
                    $('#inputCountry_State').val('');
                    $('#inputCountry_StateEN').val('');
                    $("#stateChk").prop('checked', 'checked');
                } else {
                    $scope.HasCountry_StateActive = "false";
                    $('#inputCountry_State').prop("disabled", false);
                    $("#stateChk").prop('checked', '');
                }
            });
            statesArray = [];
            $(response).each(function () {
                if (stateNames.indexOf(this.StateDesc.trim()) == -1) {
                    stateNames.push(this.StateDesc.trim());
                }
                stateIds.push(this.StateCode);
                statesArray.push({ id: this.StateCode, name: this.StateDesc, NameEN: this.StateDescEn })
            });

            if (response.length <= 1) {
                GetAllCitysByStateAndCountry(response[0].StateDesc, country);
            }
            $("#inputCountry_State").autocomplete({
                source: stateNames,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    var nameEN = GetNameENByName(statesArray, ui.item.label);
                    $("#inputCountry_StateEN").val(nameEN);
                    GetAllCitysByStateAndCountry(ui.item.label, country);
                }
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

function GetAllCitysByStateAndCountry(state, country) {
    availableCityName = [];
    cityArray
    if (state == undefined || country == undefined) {
        state = '';
        country = '';
    }
    if (GetIdByName(statesArray, state) == 0 && GetIdByName(countryArray, country) == 0) {
        $('#inputCountry_Street').val('');

        return false;
    }

    $.ajax({
        type: "POST",
        url: "/Data/GetAllCitysByStateAndCountry",
        data: { stateID: GetIdByName(statesArray, state), countyID: GetIdByName(countryArray, country) },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                cityArray = [];
                $(response).each(function () {
                    if (availableCityName.indexOf(this.CityDesc.trim()) == -1) {
                        availableCityName.push(this.CityDesc.trim());
                    }
                    availableCityIds.push(this.CityCode);
                    cityArray.push({ id: this.CityCode, name: this.CityDesc, NameEN: this.CityDescEn })
                });
            }

            $("#inputCountry_City").autocomplete({
                source: availableCityName,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    var nameEN = GetNameENByName(cityArray, ui.item.label);
                    $("#inputCountry_CityEN").val(nameEN);
                    GetAllStreetsByCityStateAndCountry(ui.item.label, country);
                }
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

function GetAllStreetsByCityStateAndCountry(city, country) {
    if (GetIdByName(cityArray, city) == 0 || GetIdByName(countryArray, country) == 0) {

        streetArray = [];
        $('#inputCountry_Street').val('');

        return false;
    }

    $.ajax({
        type: "POST",
        url: "/Data/GetAllStreetsByCityByCountry",
        data: { cityID: GetIdByName(cityArray, city), countyID: GetIdByName(countryArray, country) },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                streetArray = [];
                $(response).each(function () {
                    if (availableStreetName.indexOf(this.Streetdesc.trim()) == -1) {
                        availableStreetName.push(this.Streetdesc.trim());
                    }
                    availableStreetId.push(this.StreetCode);
                    streetArray.push({ id: this.StreetCode, name: this.Streetdesc, NameEN: this.StreetDescEn })
                });
            }
            $("#inputCountry_Street").autocomplete({
                source: availableStreetName,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    var nameEN = GetNameENByName(streetArray, ui.item.label);
                    $("#inputCountry_StreetEN").val(nameEN);
                }
            });



        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}


function StateChkClick(obj) {
    if ($("#stateChk").prop('checked') == false) {
        $('#inputCountry_State').prop("disabled", false)
    }
    else {
        $('#inputCountry_State').prop("disabled", true)
        $('#inputCountry_State').val('');
        $('#inputCountry_StateEN').val('');
    }
}

function SaveCountry() {
    var appElement = document.querySelector('[ng-controller=SearchCtrl]');
    var $scope = angular.element(appElement).scope();

    var countryID = GetIdByName(countryArray, $('#inputCountry').val());
    var countryDesc = $('#inputCountry').val();
    var countryDescEN = $('#inputCountryEN').val();
    var countryUTC = $('#inputUTC').val();

    if (countryID != 0 && countryUTC != "" && countryDesc != "") {
        if (countryID == 0) {
            $.ajax({
                url: "/Admin/SaveCountry",
                type: "post",
                data: { Countrycode: 0, CountryNameEN: countryDescEN, UTC: countryUTC, CountryDesc: countryDesc },
                success: function (result) {
                    $scope.$apply(function () {
                        if (result.Message == 'Success') {
                            $scope.ShowMessageBox('Save Message', 'Country save sucessfully.');
                            GetAllCountrys();
                        } else {
                            $scope.ShowMessageBox('Error', result.ErrorDetails)
                        }

                    });
                }
            });
        }
        else {
            $.ajax({
                url: "/Admin/SaveCountry",
                type: "post",
                data: { Countrycode: countryID, CountryNameEN: countryDescEN, UTC: countryUTC, CountryDesc: countryDesc },
                success: function (result) {
                    $scope.$apply(function () {
                        if (result.Message == 'Success') {
                            $scope.ShowMessageBox('Save Message', 'Country update sucessfully.');
                            GetAllCountrys();
                        } else {
                            $scope.ShowMessageBox('Error', result.ErrorDetails)
                        }

                    });
                }
            });
        }
    } else {
        alert('Select country, UTC first');
        return false;
    }
}

function SaveState() {
    var appElement = document.querySelector('[ng-controller=SearchCtrl]');
    var $scope = angular.element(appElement).scope();

    var countryID = GetIdByName(countryArray, $('#inputCountry').val());
    var stateDesc = $('#inputCountry_State').val();
    var stateCode = GetIdByName(statesArray, stateDesc == "" ? null : stateDesc);
    var stateDescEN = $('#inputCountry_StateEN').val();
    if ($('#inputCountry_State').prop("disabled") == true) {
        stateDesc = '';
        stateDescEN = '';
    }

    if (countryID != 0) {
        if (stateCode == 0) {
            $.ajax({
                url: "/Admin/SaveState",
                type: "post",
                data: { Countrycode: countryID, StateCode: 0, StateDescEn: stateDescEN, StateDesc: stateDesc },
                success: function (result) {
                    $scope.$apply(function () {
                        if (result.Message == 'Success') {
                            $scope.ShowMessageBox('Save Message', 'State save sucessfully.');
                            GetAllStatesByCountryID($('#inputCountry').val());
                            $scope.StateInfo = '';
                        } else {
                            $scope.ShowMessageBox('Error', 'State already exist')
                        }

                    });
                }
            });
        }
        else {
            $.ajax({
                url: "/Admin/SaveState",
                type: "post",
                data: { Countrycode: countryID, StateCode: stateCode, StateDescEn: stateDescEN, StateDesc: stateDesc },
                success: function (result) {
                    $scope.$apply(function () {
                        if (result.Message == 'Success') {
                            $scope.ShowMessageBox('Save Message', 'State Update sucessfully.');
                            GetAllStatesByCountryID($('#inputCountry').val());
                        } else {
                            $scope.ShowMessageBox('Error', result.ErrorDetails)
                        }

                    });
                }
            });
        }
    }
    else {
        alert('Select country first');
        return false;
    }
}

function SaveCity() {
    var appElement = document.querySelector('[ng-controller=SearchCtrl]');
    var $scope = angular.element(appElement).scope();

    var stateDesc = $('#inputCountry_State').val();
    var countryID = GetIdByName(countryArray, $('#inputCountry').val());
    var cityDesc = $('#inputCountry_City').val();
    var cityCode = GetIdByName(cityArray, cityDesc == "" ? null : cityDesc);
    var stateCode = GetIdByName(statesArray, stateDesc == "" ? null : stateDesc);
    var cityDescEN = $('#inputCountry_CityEN').val();

    if (countryID != 0 && cityDesc != "") {
        if (cityCode == 0) {
            $.ajax({
                url: "/Admin/SaveCity",
                type: "post",
                data: { Countrycode: countryID, CityCode: 0, CityDescEN: cityDescEN, CityDesc: cityDesc, StateCode: stateCode },
                success: function (result) {
                    $scope.$apply(function () {
                        if (result.Message == 'Success') {
                            $scope.ShowMessageBox('Save Message', 'City save sucessfully.');
                            GetAllCitysByStateAndCountry(stateDesc, $('#inputCountry').val())
                            $scope.Cityinfo = '';
                        } else {
                            $scope.ShowMessageBox('Error', result.ErrorDetails)
                        }

                    });
                }
            });
        }
        else {
            $.ajax({
                url: "/Admin/SaveCity",
                type: "post",
                data: { Countrycode: countryID, CityCode: cityCode, CityDescEN: cityDescEN, CityDesc: cityDesc, StateCode: stateCode },
                success: function (result) {
                    $scope.$apply(function () {
                        if (result.Message == 'Success') {
                            $scope.ShowMessageBox('Save Message', 'City Update sucessfully.');
                            GetAllCitysByStateAndCountry(stateDesc, $('#inputCountry').val())
                        } else {
                            $scope.ShowMessageBox('Error', result.ErrorDetails)
                        }

                    });
                }
            });
        }
    }
    else {
        alert('Select country and state');
        return false;
    }

}

function SaveStreet() {
    var appElement = document.querySelector('[ng-controller=SearchCtrl]');
    var $scope = angular.element(appElement).scope();

    var stateDesc = $('#inputCountry_State').val();
    var cityDesc = $('#inputCountry_City').val();
    var countryID = GetIdByName(countryArray, $('#inputCountry').val());
    var cityCode = GetIdByName(cityArray, cityDesc == "" ? null : cityDesc);
    var stateCode = GetIdByName(statesArray, stateDesc == "" ? null : stateDesc);

    var streetDesc = $('#inputCountry_Street').val();
    var streetCode = GetIdByName(streetArray, streetDesc == "" ? null : streetDesc);
    var streetDescEN = $('#inputCountry_StreetEN').val();

    if (countryID != 0 && cityCode != 0 && streetDesc != "") {
        if (streetCode == 0) {
            $.ajax({
                url: "/Admin/SaveStreet",
                type: "post",
                data: { Countrycode: countryID, CityCode: cityCode, StreetCode: 0, StreetDescEN: streetDescEN, StreetDesc: streetDesc, StateCode: stateCode },
                success: function (result) {
                    $scope.$apply(function () {
                        if (result.Message == 'Success') {
                            $scope.ShowMessageBox('Save Message', 'Street save sucessfully.');
                            GetAllStreetsByCityStateAndCountry(cityDesc, $('#inputCountry').val());
                            $scope.Streetinfo = '';
                        } else {
                            $scope.ShowMessageBox('Error', result.ErrorDetails)
                        }

                    });
                }
            });
        }
        else {
            $.ajax({
                url: "/Admin/SaveStreet",
                type: "post",
                data: { Countrycode: countryID, CityCode: cityCode, StreetCode: streetCode, StreetDescEN: streetDescEN, StreetDesc: streetDesc, StateCode: stateCode },
                success: function (result) {
                    $scope.$apply(function () {
                        if (result.Message == 'Success') {
                            $scope.ShowMessageBox('Save Message', 'Street Update sucessfully.');
                            GetAllStreetsByCityStateAndCountry(cityDesc, $('#inputCountry').val());
                        } else {
                            $scope.ShowMessageBox('Error', result.ErrorDetails)
                        }

                    });
                }
            });
        }
    }
    else {
        alert('Select country ,state and city');
        return false;
    }

}




function GetNameENByName(arr, name) {
    var item = $.grep(arr, function (v) { return v.name === name; })
    if (item.length > 0) {
        return item[0].NameEN;
    } else {
        return 0;
    }
}
function GetUTCByName(arr, name) {
    var item = $.grep(arr, function (v) { return v.name === name; })
    if (item.length > 0) {
        return item[0].UTC;
    } else {
        return 0;
    }
}

//Country tab end

//Company Tab
var companyNames = [];
var factoryId = [];
var factoryArray = [];
function GetAllCompanyDesc() {
    companyNames = [];
    factoryId = [];
    factoryArray = [];
    $.ajax({
        type: "POST",
        url: "/Data/GetAllCompanyDesc",
        success: function (response) {
            var appElement = document.querySelector('[ng-controller=SearchCtrl]');
            var $scope = angular.element(appElement).scope();
            $(response).each(function () {
                if (companyNames.indexOf(this.FactoryDesc.trim()) == -1) {
                    companyNames.push(this.FactoryDesc.trim());
                }
                factoryId.push(this.FactoryId);
                factoryArray.push({ id: this.FactoryId, name: this.FactoryDesc });
            });

            $("#inputCompanyDesc").autocomplete({
                source: companyNames,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    // GetExistingCountry(ui.item.label);
                }
            });

        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}
function GetFactoryData(companyDesc) {
    _factoryId = GetIdByNameData(factoryArray, $('#inputCompanyDesc').val());
    if (_factoryId < 0) {
        // $('#inputCountry_Street').val('');
        return false;
    }
    else {
        $.ajax({
            type: "POST",
            url: "/Data/GetAllFactoryDataId",
            data: { factoryId: _factoryId },
            dataType: "json",
            success: function (response) {
                debugger;
                var appElement = document.querySelector('[ng-controller=SearchCtrl]');
                var $scope = angular.element(appElement).scope();
                $scope.CompanyInfo = response.Data[0];
                $scope.CompanyInfo.CountryDesc = GetNameById(countryArray, response.Data[0].Country.toString());
                $scope.$apply();
                var obj = [];
                obj.push(response.Data[0].Lat);
                obj.push(response.Data[0].Long);
                Initialize1(obj, response.Data[0].Zoom);
                $('#companyHours tr:gt(0)').remove();
                for (var i = 0; i < response.Profiles.length; i++) {
                    $('<tr class="visiablity"><td class="left_column_design"></td><td data-title="Day Status" class="dayStatus">' + response.Profiles[i].Day + '</td><td data-title="Start Time"><input type="text" name="ct" class="timepicker time_pik hasWickedpicker start1" value="' + response.Profiles[i].Start1 + '" onkeypress="return false;"></td><td data-title="End Time"><input type="text" name="ct" class="timepicker time_pik hasWickedpicker end1" value="' + response.Profiles[i].End1 + '" onkeypress="return false;"></td><td data-title="Start Time"><input type="text" name="ct" class="timepicker time_pik hasWickedpicker start2" value="' + response.Profiles[i].Start2 + '" onkeypress="return false;"></td><td data-title="End Time"><input type="text" name="ct" class="timepicker time_pik hasWickedpicker end2" value="' + response.Profiles[i].End2 + '" onkeypress="return false;"></td><td data-title="Display Order"><input type="number" class="order " name="c1" value="' + response.Profiles[i].Order + '"></td></tr>"').appendTo($('#companyHours'))
                }
                //$('<tr class="visiablity"><td class="left_column_design"></td><td data-title="Day Status">' + days[i] + '</td><td data-title="Start Time"><input type="text" name="ct" class="timepicker time_pik hasWickedpicker" value="" onkeypress="return false;"></td><td data-title="End Time"><input type="text" name="ct" class="timepicker time_pik hasWickedpicker" value="" onkeypress="return false;"></td><td data-title="Start Time"><input type="text" name="ct" class="timepicker time_pik hasWickedpicker" value="" onkeypress="return false;"></td><td data-title="End Time"><input type="text" name="ct" class="timepicker time_pik hasWickedpicker" value="" onkeypress="return false;"></td><td data-title="Start Time"><input type="text" name="ct" class="timepicker time_pik hasWickedpicker" value="" onkeypress="return false;"></td><td data-title="End Time"><input type="text" name="ct" class="timepicker time_pik hasWickedpicker" value="" onkeypress="return false;"></td><td data-title="Display Order"><input type="number" name="c1" value="0"></td></tr>"').appendTo($('#companyHours'))

                $('.timepicker').timepicker({ 'timeFormat': 'h:i A' });

            }
        });
    }

}
function GetExistingCountry(countryDesc) {
    var vv = GetIdByNameData(countryArray, $("#inputCountryDesc").val());
    if (GetIdByName(countryArray, $("#inputCountryDesc").val()) == 0) {
        // $('#inputCountry_Street').val('');
        $("#inputCountryDesc").val('');
        return false;
    }
    else {

    }
}
function GetIdByNameData(arr, name) {
    var item = $.grep(arr, function (v) { return v.name === name; })
    if (item.length > 0) {
        return item[0].id;
    } else {
        return -1;
    }
}

//get hours grid data
function getHourData() {
    //var data = { day: '', start1: '', start2: '', end1: '', end2: '' };
    var dataList = [];
    $("#companyHours tr").each(function () {
        //if ($(this).find('input').length > 0) {
        //    $(this).find('input').attr('start') == 'Start1'
        //    '';// data.sta

        //}
        debugger;
        var data = {};
        var tdData = $(this).find('td')[1];
        data.dayStatus = $(tdData).text();
       $(this).find('td').find('input').each(function () {


            if ($(this).hasClass('start1')) {
                if (this.value != 'null' && this.value != "")
                    data.start1 = this.value;
                else
                    data.start1 = null
            }
            if ($(this).hasClass('start2')) {
                if (this.value != 'null' && this.value != "")
                    data.start2 = this.value;
                else
                    data.start2 = null
            }
            if ($(this).hasClass('end1')) {
                if (this.value != 'null' && this.value != "")
                    data.end1 = this.value;
                else
                    data.end1 = null
            }
            if ($(this).hasClass('end2')) {
                if (this.value != 'null' && this.value != "")
                    data.end2 = this.value;
                else
                    data.end2 = null
            }
            if ($(this).hasClass('order')) {
                if (this.value != 'null' && this.value != "")
                    data.order = this.value;
                else
                    data.order = null
            }
        })
        if ($(this).find('td').find('input').length > 0) {
            data.Day = this.id;
            dataList.push(data);
        }
    })
    return JSON.stringify(dataList);
}



function LoadMapByFactoryForCompanyID() {
    $.ajax({
        url: "/Data/GetCurrentLogedUserCountery", success: function (result) {
            google.maps.visualRefresh = true;
            var Liverpool = new google.maps.LatLng(result[0].Lat, result[0].Long);
            var mapOptions = {
                zoom: 14,
                center: Liverpool,
                mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
            };
            var map = new google.maps.Map(document.getElementById("map_canvas1"), mapOptions);
            google.maps.event.addListener(map, 'dblclick', function (event) {
                var appElement = document.querySelector('[ng-controller=SearchCtrl]');
                var $scope = angular.element(appElement).scope();
                $scope.CompanyInfo.Lat = event.latLng.lat();
                $scope.CompanyInfo.Long = event.latLng.lng();
                $scope.$apply();

            })
            map.addListener('zoom_changed', function () {
                var appElement = document.querySelector('[ng-controller=SearchCtrl]');
                var $scope = angular.element(appElement).scope();
                $scope.CompanyInfo.Zoom = map.getZoom();
                $scope.$apply();
            });
        }
    });
}

function Initialize1(obj, zoom) {

    if (zoom == undefined || zoom == null)
        zoom = 14;
    google.maps.visualRefresh = true;
    var Liverpool = new google.maps.LatLng(obj[0], obj[1]);
    var mapOptions = {
        zoom: zoom,
        center: Liverpool,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };
    var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
    var marker = new google.maps.Marker({
        position: Liverpool,
        map: map,
    });
    google.maps.event.addListener(map, 'dblclick', function (event) {
        var appElement = document.querySelector('[ng-controller=SearchCtrl]');
        var $scope = angular.element(appElement).scope();
        $scope.CompanyInfo.Lat = event.latLng.lat();
        $scope.CompanyInfo.Long = event.latLng.lng();
        $scope.$apply();

    })
    map.addListener('zoom_changed', function () {
        var appElement = document.querySelector('[ng-controller=SearchCtrl]');
        var $scope = angular.element(appElement).scope();
        $scope.CompanyInfo.Zoom = map.getZoom();
        $scope.$apply();
    });
};

function saveCompany() {
    var appElement = document.querySelector('[ng-controller=SearchCtrl]');
    var $scope = angular.element(appElement).scope();
    if ($scope.CompanyInfo.companyDesc == '' || $scope.CompanyInfo.CountryDesc == '' || $scope.CompanyInfo.UserName == '' || $scope.CompanyInfo.Password == '') {
        $scope.ShowMessageBox('Message', 'Data not correct');
        return false;
    }
    if (_factoryId <= -1) {

    }
    var data = {};
    data.countryId = GetIdByNameData(countryArray, $scope.CompanyInfo.CountryDesc);
    data.currentGmt = $scope.CompanyInfo.CurrentGmt
    data.customerLinkDistanceThreshold = $scope.CompanyInfo.CustomerLinkDistanceThreshold
    data.factoryDesc = $scope.CompanyInfo.FactoryDesc;
    data.factoryId = _factoryId;
    data.lat = $scope.CompanyInfo.Lat
    data.lang = $scope.CompanyInfo.Long
    data.mapProvider = $scope.CompanyInfo.MapProvider
    data.password = $scope.CompanyInfo.Password
    data.phoneAreaCode = $scope.CompanyInfo.PhoneAreaCode
    data.radiusSearch = 0;//$scope.CompanyInfo.RadiusSearch
    data.smsProvider = $scope.CompanyInfo.SmsProvider
    data.splitTime = true;
    data.stopEmployeeTime = $scope.CompanyInfo.StopEmployeeTime
    data.userName = $scope.CompanyInfo.UserName

    data.zoom = $scope.CompanyInfo.Zoom

    ////

    $.ajax({
        type: "POST",
        url: "/Admin/SaveCompany",
        data: data,
        dataType: "json",
        success: function (response) {

            debugger;
        }
    });

}
//end Company tab


function CountryDescENAndUTCOnBlur() {
    var nameEN = GetNameENByName(countryArray, $('#inputCountry').val());
    var UTC = GetUTCByName(countryArray, $('#inputCountry').val());
    $("#inputCountryEN").val(nameEN);
    $("#inputUTC").val(UTC);
    if (nameEN == 0) {
        $("#inputCountryEN").val('');
    }
}
function StateDescENOnBlur() {
    var nameEN = GetNameENByName(statesArray, $('#inputCountry_State').val());
    $("#inputCountry_StateEN").val(nameEN);
    if (nameEN == 0) {
        $("#inputCountry_StateEN").val('');
    }
}
function CityDescENOnBlur() {

    var nameEN = GetNameENByName(cityArray, $('#inputCountry_City').val());
    $("#inputCountry_CityEN").val(nameEN);
    if (nameEN == 0) {
        $("#inputCountry_CityEN").val('');
    }
}
function StreetDescENOnBlur() {
    var nameEN = GetNameENByName(streetArray, $('#inputCountry_Street').val());
    $("#inputCountry_StreetEN").val(nameEN);
    if (nameEN == 0) {
        $("#inputCountry_StreetEN").val('');
    }
}
//Country tab end

