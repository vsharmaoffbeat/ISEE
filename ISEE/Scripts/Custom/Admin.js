//Employee Section
function SaveEmployeeData() {
    debugger;

    data = {
        number: $('#txtNumber').val(), mail: $('#txtmail').val(),
        firstName: $('#txtfirstName ').val(),
        lastName: $('#txtlastName').val(),
        startDay: $('#datepicker1 input').val(),
        endDay: $('#datepicker2 input').val(),
        phone1: $('#inputPhone').val(),
        phone11: $('#inputPhone1').val(),
        phone2: $('#inputPhone2').val(),
        phone22: $('#inputPhone21').val(),
        manufacture: $('#ddlmanufacture').val(),
        phoneType: $('#ddlphoneType').val()
    };
    $.ajax({
        type: "POST",
        url: "/Admin/SaveEmployeeData",
        data: data,
        dataType: "json",
        success: function (response) { alert(response); },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });

    return false;
}
function ManufactureTypes(obj) {
    $('#ddlphoneType').empty();
    debugger
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
    LoadMapByFactoryID();
    //$.ajax({
    //    type: "POST",
    //    url: "/Admin/Test",
    //    data: { state: state, city: city, street: street, buildingNumber: buildingNumber, zipCode: zipCode, visitTime: visitTime, entry: entry, visitInterval: visitInterval, nextVisit: nextVisit },
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

