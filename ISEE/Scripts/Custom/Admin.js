//Employee Section
//function SaveEmployeeData() {


//    data = {
//        number: $('#txtNumber').val(), mail: $('#txtmail').val(),
//        firstName: $('#txtfirstName ').val(),
//        lastName: $('#txtlastName').val(),
//        startDay: $('#datepicker1 input').val(),
//        endDay: $('#datepicker2 input').val(),
//        phone1: $('#inputPhone').val(),
//        phone11: $('#inputPhone1').val(),
//        phone2: $('#inputPhone2').val(),
//        phone22: $('#inputPhone21').val(),
//        manufacture: $('#ddlmanufacture').val(),
//        phoneType: $('#ddlphoneType').val()
//    };
//    $.ajax({
//        type: "POST",
//        url: "/Admin/SaveEmployeeData",
//        data: data,
//        dataType: "json",
//        success: function (response) { alert(response); },
//        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
//    });

//    return false;
//}

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
function GetStaresByFactoryID() {
    stateNames = [];
    $.ajax({
        type: "POST",
        url: "/Admin/GetStaresByFactoryID",
        success: function (response) {
            $(response).each(function () {
                $("<option />", {
                    val: this.CountryCode,
                    text: this.CountryDescEng
                }).appendTo($('#inputState'));
                if (this.CountryDescEng == null) {
                    $('#inputState').attr('disabled', 'disabled');
                }
                stateNames.push(this.CountryDescEng);
                stateIds.push(this.CountryCode);
            });
            $("#inputState").autocomplete({
                source: stateNames,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var abliableDataForCityesName = [];
var abliableDataForCityesIds = [];
function GetCitysByState() {
    if (stateIds[stateNames.indexOf($('#inputState').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Admin/GetCitysByState",
        data: { stateID: stateIds[stateNames.indexOf($('#inputState').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                $(response).each(function () {
                    $("<option />", {
                        val: this.CityCode,
                        text: this.CityDesc
                    }).appendTo($('#inputCity'));
                    abliableDataForCityesName.push(this.CityDesc);
                    abliableDataForCityesIds.push(this.CityCode);
                });
            }
            $("#inputCity").autocomplete({
                source: abliableDataForCityesName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var abliableDataForStreetName = [];
var abliableDataForStreetId = [];
function GetStreetByCity() {
    if (stateIds[stateNames.indexOf($('#inputState').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#inputCity').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Admin/GetStreetByCity",
        data: { stateID: stateIds[stateNames.indexOf($('#inputState').val())], cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#inputCity').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                $(response).each(function () {
                    $("<option />", {
                        val: this.StreetCode,
                        text: this.Streetdesc
                    }).appendTo($('#inputStreet'));
                    abliableDataForStreetName.push(this.Streetdesc);
                    abliableDataForStreetId.push(this.StreetCode);
                });
            }
            $("#inputStreet").autocomplete({
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
    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#inputStreet').val())] == undefined || stateIds[stateNames.indexOf($('#inputState').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#inputCity').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Admin/GetBuildingsByCity",
        data: { streetID: abliableDataForStreetId[abliableDataForStreetName.indexOf($('#inputStreet').val())], stateID: stateIds[stateNames.indexOf($('#inputState').val())], cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#inputCity').val())] },
        dataType: "json",
        success: function (response) {
            if (response != null) {
                $(response).each(function () {
                    $("<option />", {
                        val: this.BuildingCode,
                        text: this.BuildingNumber
                    }).appendTo($('#inputBuldingNumber'));
                    abliableDataForBuildingNumber.push(this.BuildingNumber);
                    abliableDataForBuildingId.push(this.BuildingCode);
                    abliableDataForBuildingLat.push(this.BuildingLat);
                    abliableDataForBuildingLong.push(this.BuldingLong);
                });
            }
            $("#inputBuldingNumber").autocomplete({
                source: abliableDataForBuildingNumber,
            });
            $('#inputBuldingNumber').val(abliableDataForBuildingNumber);
            GetSelectedBuildingLatLong();
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var buildingLatLong = [];
function GetSelectedBuildingLatLong() {
    if (abliableDataForBuildingLat[abliableDataForBuildingNumber.indexOf($('#inputBuldingNumber').val())] == undefined, abliableDataForBuildingLong[abliableDataForBuildingNumber.indexOf($('#inputBuldingNumber').val())] == undefined)
        return false;
    buildingLatLong = [];
    buildingLatLong.push(abliableDataForBuildingLat[abliableDataForBuildingNumber.indexOf($('#inputBuldingNumber').val())], abliableDataForBuildingLong[abliableDataForBuildingNumber.indexOf($('#inputBuldingNumber').val())]);
}



//End Employee Section

//Start Customer section
//$(function () {
//    //TODO Remove
//    var header = $('.main_header');
//    for (var i = 0; i < header.length; i++) {
//        if (i > 0)
//            $(header[i]).hide();
//    }

//    $("#datepicker1,#datepicker2").datepicker({
//        autoclose: true,
//        todayHighlight: true
//    }).datepicker('update', new Date());;
//    $("#datepicker2").datepicker({
//        onSelect: function () {
//            datepicker2 = $(this).datepicker('getDate');
//        }
//    });
//    $("#datepicker1").datepicker({
//        onSelect: function () {
//            datepicker1 = $(this).datepicker('getDate');
//        }
//    });
//    $("#popup_div").dialog({ autoOpen: false });
//    $('.LoadTreeParital').click(function (data) {
//        $('#bindPartial_AdminTree').show();
//        $('#bindPartial_Category').hide();
//        $('#bindPartial_NewEmployee').hide();
//        $('#bindPartial_NewCustomer').hide();
//    });
//    $('.LoadCategoryParital').click(function (data) {
//        $('#bindPartial_AdminTree').hide();
//        $('#bindPartial_Category').show();
//        $('#bindPartial_NewEmployee').hide();
//        $('#bindPartial_NewCustomer').hide();

//    });
//    $('.LoadEmployeeParital').click(function (data) {
//        $('#bindPartial_AdminTree').hide();
//        $('#bindPartial_Category').hide();
//        $('#bindPartial_NewEmployee').show();
//        $('#bindPartial_NewCustomer').hide();

//    });
//    $('.LoadCustomerParital').click(function (data) {
//        $('#bindPartial_AdminTree').hide();
//        $('#bindPartial_Category').hide();
//        $('#bindPartial_NewEmployee').hide();
//        $('#bindPartial_NewCustomer').show();
//    });
//});
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



function InsertAddress() {
    var stateID, cityID, streetID, buildingNumber, zipCode, visitTime, entry, visitInterval, nextVisit;
    stateID = stateIds[stateNames.indexOf($('#inputState').val())] == undefined ? "" : stateIds[stateNames.indexOf($('#inputState').val())];
    cityID = abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#inputCity').val())] == undefined ? "" : abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#inputCity').val())];
    streetID = abliableDataForStreetId[abliableDataForStreetName.indexOf($('#inputStreet').val())] == undefined ? "" : abliableDataForStreetId[abliableDataForStreetName.indexOf($('#inputStreet').val())];
    buildingNumber = $('#inputBuldingNumber').val();
    zipCode = $('#inputEntry').val();
    visitTime = $('#inputZipCode').val();
    entry = $('#inputVisitInterval').val();
    visitInterval = $('#inputVisitTime').val();
    nextVisit = $('#inputNextVisit').val();
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



function SaveCustomerForm() {
    var buildingNumber = $('#inputBuldingNumber').val();
    debugger;


    var data = {
        //inputCompanyName: $('#inputCompanyName').val(),                                     

        //inputMobile: $('#inputMobile').val(),                     
        //state: $('#inputState').val(),
        //city: $('#inputCity').val(),
        //street: $('#inputStreet').val(),                       
        //zipCode: $('#inputZipCode').val(),
        //visitTime: $('#inputVisitTime').val(),
        //entry: $('#inputEntry').val(),

        ContactName: $('#inputContactName').val(),
        inputApartment: $('#inputApartment').val(),
        Phone2: $('#inputPhone22').val(),
        Mail: $('#inputMail').val(),
        Fax: $('#inputFax').val(),
        CustomerNumber: $('#inputCustomerNumber').val(),
        Floor: $('#inputFloor').val(),
        Phone1: $('#inputPhone11').val(),
        inputPhoneArea1: $('#inputPhoneOne').val(),
        inputPhoneArea2: $('#inputPhoneTwo').val(),
        visitInterval: $('#inputVisitInterval').val(),
        VisitTime: $('#inputVisitTime').val(),
        NextVisit: $('#inputNextVisit').val(),
        buldingCode: (abliableDataForBuildingId[abliableDataForBuildingNumber.indexOf(buildingNumber)]),
    };
    if (buildingNumber != "") {
        if ($('#inputCompanyName').val() != '') {
            debugger;
            $.ajax({
                url: "/Admin/SaveCustomerForm",
                type: "post",
                contentType: "application/json",
                data: JSON.stringify({ objCustomerData: data }),
                dataType: "json",
                success: function (result) {
                    debugger;
                }
            });
            return false;
        }
    }
}
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

    GetStaresByFactoryID();
});


function saveTree() {
    //var treeViewData = JSON.stringify($("#jstree_demo_div").jstree(true).get_json('#', { 'flat': true }));
    var treeViewData = JSON.stringify(treeJsonData);

    $.ajax({
        type: "POST",
        url: "/Admin/SaveTreeViewData", data: { treeViewData: treeViewData }, dataType: "json", success: function (result) {

            // Initialize(result);
        }
    });
}
//End Tree View Section