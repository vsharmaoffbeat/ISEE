//Search methods for customer need to move in common js
var inputCityVal = '';
var inputStreetVal = '';
var inputStateVal = '';
var _customerListArray = [];
var statesArray = []
var _stateNames = [];
var cityArray = [];
var _cityName = [];
var streetArray = []
var _streetName = [];
var buildingArray = []
var _buildingNumber = [];
var buildinCode = 0;


$(document).ready(function () {
    GetAllStatesByCountry('#ddlstateinputcustomer')
});


// To search customers by selected state,city,street,building,customernumber and companyname.
function SearchCustomers() {

    $('#selectedCustomer').html('');
    $('#selectedCustomer').css('display', 'none');
    var state = 0;
    var city = 0;
    var street = 0;
    var building = 0;

    if (GetIdByName(_statesArray, $('#ddlstateinputcustomer').val()) > 0)
        state = GetIdByName(_statesArray, $('#ddlstateinputcustomer').val());

    if (GetIdByName(_cityArray, $('#ddlcityinputCustomer').val()) > 0)
        city = GetIdByName(_cityArray, $('#ddlcityinputCustomer').val());

    if (GetIdByName(_streetArray, $('#ddlstreetinputCustomer').val()) > 0)
        street = GetIdByName(_streetArray, $('#ddlstreetinputCustomer').val());

    //if (GetIdByName(_buildingArray, $('#ddlstreetinputCustomer').val()) > 0) {
    //    building = GetIdByName(_buildingArray, $('#ddlstreetinputCustomer').val());
    //}

    if (street <= 0)
        emptyVal('#ddlstreetinputCustomer');
    if (city <= 0)
        emptyVal('#ddlcityinputCustomer');

    var data = { state: state, city: city, street: street, building: $('#ddlbuildinginputCustomer').val(), custNumber: $('#txtcustomernoInput').val().trim(), firstName: '', lastName: $('#txtcompanynameInputCustomer').val().trim(), phone: '', phone1: '', isActive: true }
    $.ajax({
        type: "POST",
        url: "/Customer/GetCustomerSarch",
        data: data,
        dataType: "json",
        success: function (response) {
            $("#tblmapsearchgridCustomer").html('');
            _customerListArray = [];
            if (response.length > 0) {
                if (response != null) {
                    //  for (var i = 0; i < response.length; i++) {
                    $(response).each(function () {
                        _customerListArray.push(this);

                        $("#tblmapsearchgridCustomer").append("<tr class='customerRow' id='" + response[i].CustomerId
                            + "' rel='" + response[i].LastName + "' FirstName='" + response[i].FirstName
                            + "' StreetName='" + response[i].StreetName
                            + "' CityName='" + response[i].CityName
                            + "' BuildingNumber='" + response[i].BuildingNumber
                            + "' ><td class='tg-dx8v'><input type='checkbox' class='chk' name='chkCustomer' onclick='ChkcustomerChange(this)'/></td><td class='tg-dx8v'>"
                            + response[i].CustomerId + "</td><td class='tg-dx8v'>" + response[i].LastName
                            + "</td><td class='tg-dx8v'>" + response[i].CityName + "</td><td class='tg-dx8v'>"
                            + response[i].StreetName + "</td></tr>");
                    });
                }
            }
            else {
                alert('No Records Founded');
                for (var i = 0; i < _custMarkers.length; i++) {
                    _custMarkers[i].setMap(null);
                }
                $("#tblmapsearchgridCustomer").html('');
                $('#selectedCustomer').html('');
                $('#selectedCustomer').css('display', 'none');
            }
        }
    })
};


//Map search script for customer
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

function GetAllStatesByCountry(inputStateId) {
    stateNames = [];
    $.ajax({
        type: "POST",
        url: "/Data/GetAllStatesByCountry",
        success: function (response) {
            if (response.length <= 1) {
                disableProperty(inputStateId);
            } else {
                removeDisableProperty(inputStateId);
            }
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
            $(inputStateId).autocomplete({
                source: stateNames,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    inputStateVal = ui.item.label;
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
        emptyVal('#ddlcityinputCustomer,#ddlstreetinputCustomer,#ddlbuildinginputCustomer');
        //    $('#ddlcityinputCustomer').val('');
        //$('#ddlstreetinputCustomer').val('');
        //$('#ddlbuildinginputCustomer').val('');

        return false;
    }
    emptyVal('#ddlstreetinputCustomer,#ddlbuildinginputCustomer');
    //$('#ddlstreetinputCustomer').val('');
    //$('#ddlbuildinginputCustomer').val('');
    $.ajax({
        type: "POST",
        url: "/Data/GetAllCitysByState",
        data: { stateID: GetIdByName(statesArray, state) },
        dataType: "json",
        success: function (response) {
            if (response.length <= 1)
                disableProperty('#ddlcityinputCustomer')
            else
                removeDisableProperty('#ddlcityinputCustomer');

            if (response != null) {
                cityArray = [];
                _cityName = [];
                $(response).each(function () {
                    _cityName.push(this.CityDesc);
                    cityArray.push({ id: this.CityCode, name: this.CityDesc })
                });
            }
            $("#ddlcityinputCustomer").autocomplete({
                source: _cityName,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    inputCityVal = ui.item.label;
                    GetAllStreetByCity(ui.item.label);
                }
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

function GetAllStreetByCity(city) {
    if (GetIdByName(_cityArray, city) == 0) {
        _streetArray = [];
        emptyVal('#ddlstreetinputCustomer,#ddlbuildinginputCustomer');
        //$('#ddlstreetinputCustomer').val('');
        //$('#ddlbuildinginputCustomer').val('');

        return false;
    }
    emptyVal('#ddlstreetinputCustomer,#ddlbuildinginputCustomer');
    //$('#ddlstreetinputCustomer').val('');
    //$('#ddlbuildinginputCustomer').val('');

    $.ajax({
        type: "POST",
        url: "/Data/GetAllStreetByCity",
        data: { cityID: GetIdByName(cityArray, city) },
        dataType: "json",
        success: function (response) {
            streetArray = [];
            _streetName = [];
            if (response != null) {
                $(response).each(function () {
                    _streetName.push(this.Streetdesc);
                    //if (availableStreetName.indexOf(this.Streetdesc.trim()) == -1) {
                    //    availableStreetName.push(this.Streetdesc.trim());
                    //}
                    //availableStreetId.push(this.StreetCode);
                    streetArray.push({ id: this.StreetCode, name: this.Streetdesc })
                });
            }
            $("#ddlstreetinputCustomer").autocomplete({
                source: _streetName,
                select: function (event, ui) {
                    var label = ui.item.label;
                    var value = ui.item.value;
                    inputStreetVal = ui.item.label;
                    GetAllBuildingsByCity(ui.item.label, $('#ddlcityinputCustomer').val());
                }
            });


        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

function GetAllBuildingsByCity(street, city) {

    if (GetIdByName(_streetArray, street) == 0 && GetIdByName(_cityArray, city) == 0) {
        _abliableDataForBuildingNumber = [];
        _abliableDataForBuildingId = [];
        _abliableDataForBuildingLat = [];
        _abliableDataForBuildingLong = [];

        return false;
    }
    $('#ddlbuildinginputCustomer').val('');
    $.ajax({
        type: "POST",
        url: "/Data/GetAllBuildingsByCity",
        data: { streetID: GetIdByName(_streetArray, street), cityID: GetIdByName(_cityArray, city) },
        dataType: "json",
        success: function (response) {
            if (response.length < 1) {
                $('#ddlbuildinginputCustomer').prop("disabled", true)
                $('#ddlbuildinginputCustomer').val('');
            } else {
                $('#ddlbuildinginputCustomer').prop("disabled", false);
            }
            if (response != null) {
                _abliableDataForBuildingNumber = [];
                _abliableDataForBuildingId = [];
                _abliableDataForBuildingLat = [];
                _abliableDataForBuildingLong = [];
                $(response).each(function () {
                    if (_abliableDataForBuildingNumber.indexOf(this.BuildingNumber.trim()) == -1) {
                        _abliableDataForBuildingNumber.push(this.BuildingNumber.trim());
                    }
                    _abliableDataForBuildingId.push(this.BuildingCode);
                    _buildingArray.push({ id: this.BuildingCode, name: this.BuildingNumber });
                    _abliableDataForBuildingLat.push(this.BuildingLat);
                    _abliableDataForBuildingLong.push(this.BuldingLong);
                });
                $("#ddlbuildinginputCustomer").autocomplete({
                    source: _abliableDataForBuildingNumber,
                });
                GetSelectedBuildingLatLong();
            }
        },
    });

}


function CheckSelectedState() {
    if ($('#ddlstateinputcustomer').val().trim() != inputStateVal.trim()) {
        $('#ddlcityinputCustomer').val('');
        $('#ddlstreetinputCustomer').val('');
        $('#ddlbuildinginputCustomer').val('');
    }
}
function CheckSelectedCity() {
    if ($('#ddlcityinputCustomer').val().trim() != inputCityVal.trim()) {
        $('#ddlstreetinputCustomer').val('');
        $('#ddlbuildinginputCustomer').val('');
    }
}
function CheckSelectedStreet() {
    if ($('#ddlstreetinputCustomer').val().trim() != inputStreetVal.trim()) {
        $('#ddlbuildinginputCustomer').val('');
    }
}


//set disable property
function disableProperty(objId) {
    $(objId).prop("disabled", true)
    $(objId).val('');
}
function removeDisableProperty(objId) {
    $(objId).prop("disabled", false)
}
function emptyVal(objId) {
    $(objId).val('');
}