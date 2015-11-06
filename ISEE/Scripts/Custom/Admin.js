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
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
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
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
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
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

var abliableDataForBuildingNumber = [];
var abliableDataForBuildingId = [];
var buildingLatLong = [];
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

                    buildingLatLong.push(this.BuildingCode, this.BuildingNumber, this.BuildingLat, this.BuldingLong);
                });
            }
            debugger;
            $("#inputBuldingNumber").autocomplete({
                source: abliableDataForBuildingNumber,
            });
            $('#inputBuldingNumber').val(abliableDataForBuildingNumber);

        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
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
    debugger;
    var state, city, street, buildingNumber, zipCode, visitTime, entry, visitInterval, nextVisit;
    state = $('#inputState').val();
    city = $('#inputCity').val();
    street = $('#inputStreet').val();
    buildingNumber = $('#inputBuldingNumber').val();
    zipCode = $('#inputEntry').val();
    visitTime = $('#inputZipCode').val();
    entry = $('#inputVisitInterval').val();
    visitInterval = $('#inputVisitTime').val();
    nextVisit = $('#inputNextVisit').val();
    $("#popup_div").dialog("open");
    Initialize(buildingLatLong);
    //debugger;
    //$.ajax({
    //    type: "POST",
    //    url: "/Admin/InsertAddress",
    //    data: { buildingNumber: buildingNumber, zipCode: zipCode, visitTime: visitTime, entry: entry, visitInterval: visitInterval, nextVisit: nextVisit },
    //    dataType: "json",
    //    success: function (response) { alert(response); },
    //    error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    //});
}

function LoadMapByFactoryID() {
    $.ajax({
        url: "/Admin/GetCurrentLogedUserCountery", success: function (result) {
            Initialize(result);
        }
    });
}

function Initialize(obj) {
    debugger;
    google.maps.visualRefresh = true;
    var Liverpool = new google.maps.LatLng(obj[0].Lat, obj[0].Long);
    var mapOptions = {
        zoom: 14,
        center: Liverpool,
        mapTypeId: google.maps.MapTypeId.G_NORMAL_MAP
    };
    var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);
};



function SaveCustomerForm() {

    //var inputCustomerNumber, inputFloor, inputPhone1, inputCompanyName, inputApartment, inputPhone2, inputContactName, inputMail, inputMobile, inputFax, inputState;
    //var state, city, street, buildingNumber, zipCode, visitTime, entry, visitInterval, nextVisit;

    data = {
        inputCustomerNumber: $('#inputCustomerNumber').val(),
        inputFloor: $('#inputFloor').val(),
        inputPhone1: $('#inputPhone1').val(),
        inputCompanyName: $('#inputCompanyName').val(),
        inputApartment: $('#inputApartment').val(),
        inputPhone2: $('#inputPhone2').val(),
        inputContactName: $('#inputContactName').val(),
        inputMail: $('#inputMail').val(),
        inputMobile: $('#inputMobile').val(),
        inputFax: $('#inputFax').val(),

        state: $('#inputState').val(),
        city: $('#inputCity').val(),
        street: $('#inputStreet').val(),
        buildingNumber: $('#inputBuldingNumber').val(),
        zipCode: $('#inputEntry').val(),
        visitTime: $('#inputZipCode').val(),
        entry: $('#inputVisitInterval').val(),
        visitInterval: $('#inputVisitTime').val(),
        nextVisit: $('#inputNextVisit').val()
    }

    if ($('#inputCompanyName').val() != '') {
        $.ajax({
            url: "/Admin/SaveCustomerForm", data: data, dataType: "json", success: function (result) {
                // Initialize(result);
            }
        });
        return false;
        //type: "POST",
        //url: "/Admin/SaveEmployeeData",
        //data: data,
        //dataType: "json",
        //$.ajax-({
        //    url: "/Admin/SaveCustomerForm",
        //    data: { state: state, city: city, street: street, buildingnumber: buildingnumber, zipcode: zipcode, visittime: visittime, entry: entry, visitinterval: visitinterval, nextvisit: nextvisit },
        //    success: function (response) { alert(response); },
        //    error: function (xhr, ajaxoptions, thrownerror) { alert(xhr.responsetext); }
        //})
    }
}
//End Customer section

//Start Tree View Section

//var treeJsonData = [
//       { "id": "ajsonte", "parent": "#", "text": "Simple root node", "icon": "/images/Home.png" },
//      { "id": "ajsontest1", "parent": "ajsonte", "text": "Simple root node", "icon": "/images/Home.png" },
//  { "id": "ajsontest2", "parent": "ajsonte", "text": "Root node 2", "icon": "/images/Home.png" },
//  { "id": "ajsontest3", "parent": "ajsontest2", "text": "Child 1" },
//  { "id": "ajsontest4", "parent": "ajsontest2", "text": "Child 2" },
//];

//this will hold reference to the tr we have dragged and its helper
var c = {};

$(document).ready(function () {
    //$("#jstree_demo_div").jstree({

    //    'core': {
    //        "check_callback": true,
    //        'data': treeJsonData
    //    },
    //    "plugins": ["dnd"],
    //    'types': {
    //    'types' : {
    //        'file' : {
    //            'icon' : {
    //                'image' : ''
    //            }
    //        },
    //        'default' : {
    //            'icon' : {
    //                'image' : ''
    //            },
    //            'valid_children' : 'default'
    //        }
    //    }

    //}
    //});
    GetStaresByFactoryID();
    var notinprogress = true;
    document.getElementById("jstree_demo_div").addEventListener('mouseover', function (e) {
        //This will be the top-most DOM element under cursor
        var target = e.target;
        if (notinprogress && c.helper != undefined && target.tagName == "A" && target.parentElement != undefined && target.parentElement.tagName == "LI" && e.target.parentElement.className.indexOf("jstree-node") >= 0) {
            notinprogress = false;
            var newNode = { "id": "ajson" + c.helper.data("id"), "parent": target.parentElement.getAttribute("id"), "text": c.helper.data("name"), "objectid": c.helper.data("id"), "objecttype": c.helper.data("type") };

            treeJsonData.push(newNode);
            resfreshJSTree();
            notinprogress = true;
            c = {};
        }
    });

});

var draggedDivElement;



$("#employeeGrid tr.tree-drop").draggable({
    helper: "clone",
    start: function (event, ui) {
        c.tr = this;
        c.helper = ui.helper;
    }
});

var prevTarget = null;

function resfreshJSTree() {
    $('#jstree_demo_div').jstree(true).settings.core.data = treeJsonData;
    $('#jstree_demo_div').jstree(true).refresh();
}














function demo_create() {

    var ref = $('#jstree_demo_div').jstree(true),
        sel = ref.get_selected();
    if (!sel.length) { return false; }
    sel = sel[0];
    sel = ref.create_node(sel, { "type": "file" });
    if (sel) {
        ;
        treeJsonData.push(ref.get_node(sel));
        ref.edit(sel);
    }
};
function demo_rename() {
    var ref = $('#jstree_demo_div').jstree(true),
        sel = ref.get_selected();
    if (!sel.length) { return false; }
    sel = sel[0];
    ref.edit(sel);
};
function demo_delete() {
    var ref = $('#jstree_demo_div').jstree(true),
        sel = ref.get_selected();
    if (!sel.length) { return false; }
    ref.delete_node(sel);
};
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