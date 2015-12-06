var _customerId = 0;

var stateNames = [];
var stateIds = [];
$(document).ready(function () {
    $("#datepicker2,#datepickerEndDay,#nextVisitDatePicker,#datepicker3,#datepicker4,#datepicker5,#datepicker6").datepicker({
        autoclose: true,
        todayHighlight: true
    }).datepicker('update', new Date());;
    $("#datepicker2").datepicker({
        onSelect: function () {
            datepicker2 = $(this).datepicker('getDate');
        }
    });
    $("#datepicker1").datepicker({
        onSelect: function () {
            datepicker1 = $(this).datepicker('getDate');
        }
    });
    setDatePickerValuesDefault();
    // $(".right_main_employee :input").prop("disabled", true);
    GetStaresByFactoryID();
    BindClassificationDdl();
    $("#datepicker5 input").val('')

    $("#datepickerEndDay,#nextVisitDatePicker").datepicker('remove');
    $('#datepickerEndDay input').val('');
    $('#nextVisitDatePicker input').val('');

    //set viewbag property
    $('#showMap').click(function () {

        if (parseInt(_customerId) <= 0)
            alert("Select Employee");
        data = { empId: 0, cusId: _customerId }
        $.ajax({
            type: "POST",
            url: "/Data/SetViewBagProperty",
            data: data,
            dataType: "json",
            success: function (response) {

            },
            error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
        });
    })
});

function setDatePickerValuesDefault() {
    $('#datepicker1 input').val('');
    $('#datepicker2 input').val('');
    $('#datepicker3 input').val('');
    $('#datepicker4 input').val('');
}
function setDatePickerValuesSelection() {
    var d = new Date();
    d.setMonth(d.getMonth() - 3);
    $("#datepicker1").datepicker('setDate', d);
    $("#datepicker3").datepicker('setDate', d);
    $("#datepicker2").datepicker('setDate', new Date());
    $("#datepicker4").datepicker('setDate', new Date());
    $('#tblrequest tr:gt(0)').remove();
    $('#tblVisiting tr:gt(0)').remove();
}

//Classification();
function Classification() {

    $('#secondClassification').empty();

    $("<option value='-1' />").appendTo($('#secondClassification'));
    if ($('#mainClassificationDdl :selected').val() == "")
        return false;
    getSecondaryResults(parseInt($('#mainClassificationDdl :selected').val()), false);


}
function popUpClassification() {

    $('#addSecondary').empty();

    $("<option value='-1' />").appendTo($('#addSecondary'));
    if ($('#addmanufacture :selected').val() == "")
        return false;
    getSecondaryResults(parseInt($('#addmanufacture :selected').val()), true);

}


//get secondary results
function getSecondaryResults(id, isPopup) {


    data = { id: id };
    $.ajax({
        type: "POST",
        url: "/Customer/BindSecondClassificationDdl",
        data: data,
        success: function (response) {
            debugger;
            if (response != null) {
                if (!isPopup) {
                    $(response).each(function () {
                        $("<option />", {
                            val: this.RequestSysIdLevel2,
                            text: this.RequestDescCodeLevel2
                        }).appendTo($('#secondClassification'));
                    });
                }
                else {
                    $(response).each(function () {
                        $("<option />", {
                            val: this.RequestSysIdLevel2,
                            text: this.RequestDescCodeLevel2
                        }).appendTo($('#addSecondary'));
                    });
                }
            }

        }
    })
}

//Bind Second Classification 
function BindClassificationDdl() {
    $('#mainClassificationDdl').empty();
    $('#addmanufacture').empty();
    $("<option value='-1' />").appendTo($('#mainClassificationDdl'));
    $("<option value='-1' />").appendTo($('#addmanufacture'));
    $.ajax({
        type: "POST",
        url: "/Customer/BindClassificationDdl",
        dataType: "json",
        success: function (response) {
            if (response == null)
                return false;
            $(response).each(function () {
                $("<option />", {
                    val: this.RequestSysIdLevel1,
                    text: this.RequestDescCodeLevel1
                }).appendTo($('#mainClassificationDdl'));
                $("<option />", {
                    val: this.RequestSysIdLevel1,
                    text: this.RequestDescCodeLevel1
                }).appendTo($('#addmanufacture'));
            });

        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}
//Search request customer data 

function searchRequestCustData() {
    if (_customerId <= 0)
        return false;
    var main = 0;
    var second = 0
    if ($('#mainClassificationDdl :selected').val() != '')
        main = $('#mainClassificationDdl :selected').val()
    if ($('#secondClassification :selected').val() != '')
        second = $('#secondClassification :selected').val()

    data = { customerID: _customerId, fromyear: 0, from: $("#datepicker1 input").val(), to: $("#datepicker2 input").val(), level1: main, level2: second };



    $.ajax({
        type: "POST",
        url: "/Customer/GetRequestCustomerByDate",
        data: data,
        success: function (response) {
            debugger;
            if (response != null) {

                $(response).each(function () {
                    $('<tr class="tg-dx8v"> <td class="tg-dx8v"></td><td class="tg-dx8v">' +
                        this.CreateDate
                        + '   </td><td class="tg-dx8v">' +
                        this.RequestSysIdLevel1 +
                        '</td<td class="tg-dx8v">' +
                        this.RequestSysIdLevel2 +
                        '</td><td class="tg-dx8v">'
                        + this.Request + '</td><td class="tg-dx8v">'
                        + this.Treatment + '</td><td class="tg-dx8v">'
                        + this.TreatmentDate
                        + '</td></tr>').appendTo($('#tblrequest'));
                });
            }

        }
    })
}

//Get visiting data 
function GetEmployeesToCustomerFilter() {
    if (_customerId <= 0) {
        alert('Must select customer');
        return false;
    }
    var main = 0;
    var second = 0
    if ($('#mainClassificationDdl :selected').val() != '')
        main = $('#mainClassificationDdl :selected').val()
    if ($('#secondClassification :selected').val() != '')
        second = $('#secondClassification :selected').val()

    data = { customerID: _customerId, dtFrom: $("#datepicker3 input").val(), dtTo: $("#datepicker4 input").val(), level1: main, level2: second };



    $.ajax({
        type: "POST",
        url: "/Customer/GetEmployeesToCustomerFilter",
        data: data,
        success: function (response) {
            debugger;
            if (response != null) {

                $(response).each(function () {
                    $('<tr class="tg-dx8v"> <td class="tg-dx8v"></td><td class="tg-dx8v">' +
                        this.CreateDate
                        + '   </td><td class="tg-dx8v">' +
                        this.Time +
                        '</td><td class="tg-dx8v">' +
                        this.EmployeeNum + '</td><td class="tg-dx8v">' +
                    this.LastName + ' ' + this.FirstName +
                        '</td><td class="tg-dx8v">'
                        + this.InsertStatus + '</td></tr>').appendTo($('#tblVisiting'));
                });
            }

        }
    })
}

function GetStaresByFactoryID() {
    stateNames = [];
    stateIds = [];
    $.ajax({
        type: "POST",
        url: "/Data/GetAllStatesByCountry",
        success: function (response) {
            if (response.length == 1 && response[0].CountryDescEng == null) {

                stateNames.push('');
                stateIds.push(response[0].CountryCode);
                $('#custState').val('');
                GetCitysByState();
            }
            else if (response != null && response.length > 0 && response[0].CityDesc != null) {
                $(response).each(function () {
                    stateNames.push(this.CountryDescEng);
                    stateIds.push(this.CountryCode);
                });
            }
            $("#custState").autocomplete({
                source: stateNames,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

//Get cities based on state
var abliableDataForCityesName = [];
var abliableDataForCityesIds = [];
function GetCitysByState() {
    if (stateIds[stateNames.indexOf($('#custState').val())] == undefined)
        return false;

    abliableDataForCityesName = [];
    abliableDataForCityesIds = [];
    $.ajax({
        type: "POST",
        url: "/Data/GetCitysByState",
        data: { stateID: stateIds[stateNames.indexOf($('#custState').val())] },
        dataType: "json",
        success: function (response) {
            if (response.length == 1 && response[0].CityDesc == null) {
                abliableDataForCityesName.push('');
                abliableDataForCityesIds.push(response[0].CityCode);
                $('#custCity').val('');
                GetStreetByCity();
            }
            else if (response != null && response.length > 0 && response[0].CityDesc != null) {
                $(response).each(function () {
                    abliableDataForCityesName.push(this.CityDesc);
                    abliableDataForCityesIds.push(this.CityCode);
                });
            }
            $("#custCity").autocomplete({
                source: abliableDataForCityesName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

//Get street based on state and city
var abliableDataForStreetName = [];
var abliableDataForStreetId = [];
function GetStreetByCity() {
    if (stateIds[stateNames.indexOf($('#custState').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Data/GetStreetByCity",
        data: { stateID: stateIds[stateNames.indexOf($('#custState').val())], cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())] },
        dataType: "json",
        success: function (response) {
            if (response.length == 1 && response[0].Streetdesc == null) {

                abliableDataForStreetName.push('');
                abliableDataForStreetId.push(response[0].StreetCode);

                $('#custStreet').val('');
                GetBuildingsByCity();
            }
            else if (response != null && response.length > 0 && response[0].Streetdesc != null) {
                $(response).each(function () {
                    abliableDataForStreetName.push(this.Streetdesc);
                    abliableDataForStreetId.push(this.StreetCode);
                });
            }
            $("#custStreet").autocomplete({
                source: abliableDataForStreetName,
            });
        },
        //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

//Get Building  based on state and city and street
var abliableDataForBuildingNumber = [];
var abliableDataForBuildingId = [];
var abliableDataForBuildingLat = [];
var abliableDataForBuildingLong = [];

function GetBuildingsByCity() {
    abliableDataForBuilding = [];
    abliableDataForBuildingId = [];
    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#custStreet').val())] == undefined || stateIds[stateNames.indexOf($('#custState').val())] == undefined || abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())] == undefined)
        return false;
    $.ajax({
        type: "POST",
        url: "/Data/GetBuildingsByCity",
        data: { streetID: abliableDataForStreetId[abliableDataForStreetName.indexOf($('#custStreet').val())], stateID: stateIds[stateNames.indexOf($('#custState').val())], cityID: abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())] },
        dataType: "json",
        success: function (response) {
            if (response.length == 1 && response[0].BuildingNumber == null) {

                abliableDataForBuildingNumber.push('');
                abliableDataForBuildingId.push(response[0].BuildingCode);

                $('#custBuldingNumber').val('');
                //   GetBuildingsByCity();
            }
            if (response != null && response.length > 0 && response[0].BuildingNumber != null) {
                $(response).each(function () {
                    abliableDataForBuildingNumber.push(this.BuildingNumber);
                    abliableDataForBuildingId.push(this.BuildingCode);
                });
            }
            $("#custBuldingNumber").autocomplete({
                source: abliableDataForBuildingNumber,
            });
            //  $('#inputBuldingNumber').val(abliableDataForBuildingNumber);

            //error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
        }
    });
}

//Search function for customer 
function searchCustomerData() {
    debugger;
    $('#left_employee_window').empty();
    var state = 0;
    var city = 0;
    //   var building = 0;
    var street = 0;

    if (stateIds[stateNames.indexOf($('#custState').val())] != undefined)
        state = stateIds[stateNames.indexOf($('#custState').val())];
    if (abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())] != undefined)
        city = abliableDataForCityesIds[abliableDataForCityesName.indexOf($('#custCity').val())];
    if (abliableDataForStreetId[abliableDataForStreetName.indexOf($('#custStreet').val())] != undefined)
        street = abliableDataForStreetId[abliableDataForStreetName.indexOf($('#custStreet').val())];

    //if (abliableDataForBuildingId[abliableDataForBuildingNumber.indexOf($('#custBuldingNumber').val())] != undefined)
    //    building = abliableDataForBuildingId[abliableDataForBuildingNumber.indexOf($('#custBuldingNumber').val())];


    var data = { state: state, city: city, street: street, building: $('#custBuldingNumber').val(), custNumber: $('#custNumber').val().trim(), firstName: $('#custName').val().trim(), lastName: $('#custCompany').val().trim(), phone: $('#custPhone').val().trim(), phone1: $('#custPhone1').val().trim(), isActive: $('#isActive').is(':checked') }


    // { manufacture: $("#ddlmanufacture").val(), lastName: $('#empLastname').val(), firstName: $('#empFirstname').val(), empNumber: $('#empNumber').val(), phoneType: $('#ddlphoneType').val(), isActive: $('#isActive').is(':checked') }
    $.ajax({
        type: "POST",
        url: "/Customer/GetCustomerSarch",
        data: data,
        dataType: "json",
        success: function (response) {
            if (response == null) {
                $('#left_employee_window').text('No records found.');
                return false;
            }
            debugger;
            if (response.length <= 0) {


                $('<div class="row" > <div class="col-md-12 col-xs-12 tab_box">No records found.</div></div>').appendTo($('#left_employee_window'));
                return false;
            }
            var setAttr = ''
            $(response).each(function () {
                $('<div class="col-sm-12 col-xs-12 tab_box" onclick="selectedCustomer(this)"' +

                     ' CustomerId=' + stringCreation(this.CustomerId) + ' CustomerNumber=' + stringCreation(this.CustomerNumber) + ' FirstName=' + stringCreation(this.FirstName) +
                       ' LastName=' + stringCreation(this.LastName) + ' Floor=' + stringCreation(this.Floor) + ' Apartment=' + stringCreation(this.Apartment) +
                       ' Phone2=' + stringCreation(this.Phone2) + ' AreaFax=' + stringCreation(this.AreaFax) + ' Fax=' + stringCreation(this.Fax) +
                       ' AreaPhone2=' + stringCreation(this.AreaPhone2) + ' Phone1=' + stringCreation(this.Phone1) + ' AreaPhone1=' + stringCreation(this.AreaPhone1) +
  ' Mail=' + stringCreation(this.Mail) + ' CustomerRemark1=' + stringCreation(this.CustomerRemark1) + ' CustomerRemark2=' + this.CustomerRemark2 +
                          ' VisitInterval=' + stringCreation(this.VisitInterval) + ' NextVisit=' + stringCreation(this.NextVisit) + ' LastName=' + stringCreation(this.LastName) +

                           ' VisitDate=' + this.VisitDate + ' VisitTime=' + stringCreation(this.VisitTime) + ' ZipCode=' + stringCreation(this.ZipCode) +

                            ' BuildingCode=' + stringCreation(this.BuildingCode) + ' BuildingNumber=' + stringCreation(this.BuildingNumber) +

                            ' StreetName=' + stringCreation(this.StreetName) + ' StreetId=' + this.StreetId + ' CityName=' + this.CityName +
                           ' CityId=' + this.CityId + ' StateName=' + stringCreation(this.StateName) + ' StateId=' + this.StateId +
                           ' EndDate=' + this.EndDate +

                                            '>Company Name: '
                   + stringValidation(this.LastName) + ' </br>Contact Name: ' + stringValidation(this.FirstName) + '</br>City: ' + stringValidation(this.CityName) + '</br>Street: ' + stringValidation(this.StreetName) + ' , ' + stringValidation(this.BuildingNumber) + '</br>Phone1: ' + stringValidation(this.AreaPhone1) + '-' + stringValidation(this.Phone1)
                   + '        </div>').appendTo($('#left_employee_window'));
            });


        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });

}
function selectedEmployee(obj) {
    var d = new Date();
    d.setMonth(d.getMonth() - 3);
    $("#datepicker1").datepicker('setDate', d);
    d = new Date();
    $("#datepicker2").datepicker('setDate', d);
    var data = $(obj).attr('EmployeeId').split('|');
    _employeeId = $(obj).attr('EmployeeId');
    //get messgae history
    getMessageHistory(_employeeId, $("#datepicker1 input").val(), $("#datepicker2 input").val());
    //get Employeefill hours
    getEmployeeTimeTemplate(_employeeId);
    //get Employee history template
    getEmployeeTimeHistoryDiary();
    //Set employee data
    setInputValues(obj);



}

function selectedCustomer(obj) {
    _customerId = $(obj).attr('CustomerId');
    removeChange();
    //setInputValues(obj);
    //$(".disabledClass").prop("disabled", true);
    setDatePickerValuesSelection();
    $('#showMap').attr('href', '/map/map');

}

function setInputValues(obj) {

    // $(".right_main_employee :input").prop("disabled", false);

    $('#inputCustomerNumber').val(stringValidation($(obj).attr('CustomerNumber')));
    $('#inputCompanyName').val(stringValidation($(obj).attr('lastName')));
    $('#inputContactName').val(stringValidation($(obj).attr('firstName'))); 1
    $('#inputFloor').val(stringValidation($(obj).attr('Floor')));

    $('#inputApartment').val(stringValidation($(obj).attr('Apartment')));
    $('#inputMail').val(stringValidation($(obj).attr('inputMail')));
    $('#inputPhoneOne').val(stringValidation($(obj).attr('AreaPhone1')));
    $('#inputPhone11').val(stringValidation($(obj).attr('Phone1')));
    $('#inputPhoneTwo').val(stringValidation($(obj).attr('AreaPhone2')));
    $('#inputPhone22').val(stringValidation($(obj).attr('Phone2')));
    $('#inputFax').val(stringValidation($(obj).attr('AreaFax')));
    $('#inputFax1').val(stringValidation($(obj).attr('Fax')));
    $('#customerRemarks1').val(stringValidation($(obj).attr('CustomerRemark1')));
    $('#customerRemarks2').val(stringValidation($(obj).attr('CustomerRemark2')));
    $('#stateId').val(stringValidation($(obj).attr('StateName')));
    $('#cityId').val(stringValidation($(obj).attr('CityName')));
    $('#streetID').val(stringValidation($(obj).attr('StreetName')));
    $('#buildingCode').attr('BuildingCode', stringValidation($(obj).attr('BuildingCode')));
    $('#buildingCode').val(stringValidation($(obj).attr('BuildingNumber')));
    $('#zipcode').val(stringValidation($(obj).attr('ZipCode')));
    $('#visitInterval').val(stringValidation($(obj).attr('VisitInterval')));
    $('#datepickerEndDay').val(stringValidation($(obj).attr('EndDate')));
    $('#visitTime').val(stringValidation($(obj).attr('VisitTime')));
    $('#nextVisitDatePicker').val(stringValidation($(obj).attr('NextVisit')));
}

//clear search fields
function clearSearchFields() {

    $('.container.main_customer_tab input').val('');
}
//Update Customer
function updateCustomer() {
    debugger;
    var visit = 0;
    var cvisitInterval = 0;
    if ($('#visitInterval').val() != '')
        cvisitInterval = $('#visitInterval').val();
    if ($('#visitTime').val() != '')
        visit = $('#visitTime').val();
    data = {
        customerID: _customerId,
        cNumber: $('#inputCustomerNumber').val(),
        cCompanyName: $('#inputCompanyName').val(),
        cContactName: $('#inputContactName').val(),
        cFloor: $('#inputFloor').val(),

        cApartment: $('#inputApartment').val(),
        cMail: $('#inputMail').val(),
        cPhoneOne: $('#inputPhoneOne').val(),
        cPhone11: $('#inputPhone11').val(),
        cPhoneTwo: $('#inputPhoneTwo').val(),
        cPhone22: $('#inputPhone22').val(),
        cFax: $('#inputFax').val(),
        cFax1: $('#inputFax1').val(),
        cRemarks1: $('#customerRemarks1').val(),
        cRemarks2: $('#customerRemarks2').val(),
        cbuildingCode: $('#buildingCode').attr('buildingCode'),
        cbuildingNumber: $('#buildingCode').val(),

        cZipCode: $('#zipcode').val(),
        cvisitInterval: cvisitInterval,
        cEndDate: $('#datepickerEndDay input').val(),
        cNextVisit: $('#nextVisitDatePicker input').val(),
        cvisitTime: visit

    }
    $.ajax({
        type: "POST",
        url: "/Customer/UpdateCustomer",
        data: data,
        dataType: "json",
        success: function (response) {
            $('#msgHistory tr:gt(0)').remove();
            if (response) {
                alert("Customer Updated successfully.");
                $('#left_employee_window div').each(function () {
                    if ($(this).attr('customerid') == _customerId) {
                        $(this).attr('CustomerId', data.customerID);
                        $(this).attr('CustomerNumber', data.cNumber);
                        $(this).attr('FirstName', data.cContactName);
                        $(this).attr('LastName', data.cCompanyName);
                        $(this).attr('Floor', data.cFloor);
                        $(this).attr('Apartment', data.cApartment);
                        $(this).attr('Mail', data.cMail);
                        $(this).attr('AreaPhone1', data.cPhoneOne);
                        $(this).attr('Phone1', data.cPhone11);
                        $(this).attr('AreaPhone2', data.cPhoneTwo);

                        $(this).attr('Phone2', data.cPhone22);
                        $(this).attr('AreaFax', data.cFax);
                        $(this).attr('Fax', data.cFax1);
                        $(this).attr('CustomerRemark1', data.cRemarks1);
                        $(this).attr('CustomerRemark2', data.cRemarks2);
                        $(this).attr('VisitInterval', data.cvisitInterval);
                        $(this).attr('VisitTime', data.cvisitTime);
                        $(this).attr('Mail', data.cMail);

                        $(this).empty();
                        $('<div class="col-md-12 col-xs-12 tab_box">Company Name: ' + data.cCompanyName + ' </br>Contact Name: ' + data.cContactName + '</br>City: ' + $(this).attr('CityName') + '</br>Street: ' + $(this).attr('StreetName') + ' , ' + data.cbuildingNumber + '</br>Phone1: ' + data.cPhoneOne + '-' + data.cPhone11 + '  </div>').appendTo($(this));
                        removeChange();
                    }
                })
            }
            else
                return alert("Failed to Update.");
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });

}




//Get Customer Request Data

//Get sms history and bind grid
function getCustomerRequestData(id, start, end) {
    data = { employeeId: id, start: start, end: end }
    $.ajax({
        type: "POST",
        url: "/Data/GetMessageHistory",
        data: data,
        dataType: "json",
        success: function (response) {
            $('#msgHistory tr:gt(0)').remove();
            if (response == null) {
                return true;
            }
            $(response).each(function () {
                $('<tr><td class="tg-dx8v"></td><td class="tg-dx8v">' + this.SmsCreatDate + '</td><td class="tg-dx8v">' + this.SmsStatus + '</td><td class="tg-dx8v">' + this.SmsMsg + '</td><td class="tg-dx8v">' + this.SmsCount + '</td><td class="tg-dx8v"></td></tr>').appendTo($('#msgHistory'));;
            });
            debugger;

        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

//Save Classification



//set details click
function setDetails() {
    if (parseInt(_customerId) > 0) {
        setDatePicker();
        $(".disabledClass").prop("disabled", false);
        $('.main_employee_save').first().hide();
        $('.main_employee_save').last().show()
    }
    else
        alert("Select Customer");
}

//Cancel click
function removeChange() {
    if (parseInt(_customerId) > 0)
        $('#left_employee_window div').each(function () {
            if ($(this).attr('customerid') == _customerId) {
                setInputValues(this);
                //$("#datepickerStartDay,#datepickerEndDay,#datepickerLastApp").datepicker('remove');
                $('.main_employee_save').first().show();
                $(".disabledClass").prop("disabled", true);
                $('.main_employee_save').last().hide();
            }
        })
}

//Common Methods TODO in common file
function stringValidation(val) {
    if (val == '' || val == 'null' || val == '!@#$')
        return '';
    return val;
}
//Common Methods TODO in common file
function stringCreation(val) {
    if (val == '' || val == 'null')
        return '!@#$';
    return val;
}
function setDatePicker() {

    $("#datepickerEndDay").datepicker('add')
    $("#nextVisitDatePicker").datepicker('add')
    //.on('changeDate', function (selected) {
    //    var minDate = new Date(selected.date.valueOf());
    //    //     $('#datepickerStartDay').datepicker('setEndDate', minDate);
    //});
}




// Map snippets
