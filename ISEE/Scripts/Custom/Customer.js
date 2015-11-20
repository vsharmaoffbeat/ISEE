var _customerId = 0;

var stateNames = [];
var stateIds = [];
$(document).ready(function () {
    $("#datepicker2,#datepicker1,#datepicker3,#datepicker4,#datepicker5,#datepicker6").datepicker({
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
    $(".right_main_employee :input").prop("disabled", true);
    GetStaresByFactoryID();
    BindClassificationDdl();
    $("#datepicker5 input").val('')
});

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

//data = { id: parseInt($('#mainClassificationDdl :selected').val()) };
//  $.ajax({
//      type: "POST",
//      url: "/Customer/BindSecondClassificationDdl",
//      data: data,
//      success: function (response) {
//          debugger;
//          if (response != null) {

//              $(response).each(function () {
//                  $("<option />", {
//                      val: this.RequestSysIdLevel2,
//                      text: this.RequestDescCodeLevel2
//                  }).appendTo($('#secondClassification'));
//              });
//          }

//      }
//  })

//}
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
    if (_customerId <= 0)
        return false;
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
        url: "/Data/GetStatesByFactoryID",
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


    var data = { state: state, city: city, street: street, building: $('#custBuldingNumber').val(), custNumber: $('#custNumber').text().trim(), firstName: $('#custName').text().trim(), lastName: $('#custCompany').text().trim(), phone: $('#custPhone').text().trim(), phone1: $('#custPhone1').text().trim(), isActive: $('#isActive').is(':checked') }


    // { manufacture: $("#ddlmanufacture").val(), lastName: $('#empLastname').val(), firstName: $('#empFirstname').val(), empNumber: $('#empNumber').val(), phoneType: $('#ddlphoneType').val(), isActive: $('#isActive').is(':checked') }
    $.ajax({
        type: "POST",
        url: "/Customer/GetCustomerSarch",
        data: data,
        dataType: "json",
        success: function (response) {
            if (response == null) {
                $('#left_employee_window').text('No records found.');
                return true;
            }
            debugger;
            var setAttr = ''
            $(response).each(function () {
                $('<div class="col-sm-12 col-xs-12 tab_box" onclick="selectedCustomer(this)"' +

                     ' CustomerId=' + this.CustomerId + ' CustomerNumber=' + this.CustomerNumber + ' FirstName=' + this.FirstName +
                       ' LastName=' + this.LastName + ' Floor=' + this.Floor + ' Apartment=' + this.Apartment +
                       ' Phone2=' + this.Phone2 + ' AreaFax=' + this.AreaFax + ' Fax=' + this.Fax +
                       ' AreaPhone2=' + this.AreaPhone2 + ' Phone1=' + this.Phone1 + ' AreaPhone1=' + this.AreaPhone1 +
  ' Mail=' + this.Mail + ' CustomerRemark1=' + this.CustomerRemark1 + ' CustomerRemark2=' + this.CustomerRemark2 +
                          ' VisitInterval=' + this.VisitInterval + ' NextVisit=' + this.NextVisit + ' LastName=' + this.LastName +

                           ' VisitDate=' + this.VisitDate + ' VisitTime=' + this.VisitTime + ' ZipCode=' + this.ZipCode +

                            ' BuildingCode=' + this.BuildingCode + ' BuildingNumber=' + this.BuildingNumber +

                            ' StreetName=' + this.StreetName + ' StreetId=' + this.StreetId + ' CityName=' + this.CityName +
                           ' CityId=' + this.CityId + ' StateName=' + this.StateName + ' StateId=' + this.StateId +


                                            '>Company Name: '
                   + this.FirstName + ' </br>Contact Name: ' + this.LastName + '</br>City: ' + this.CityName + '</br>Street: ' + this.StreetName + ' , ' + this.BuildingNumber + '</br>Phone1: ' + this.AreaPhone1 + '-' + this.Phone1
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
    setInputValues(obj);
}

function setInputValues(obj) {

    $(".right_main_employee :input").prop("disabled", false);

    $('#inputCustomerNumber').val(defaultValues($(obj).attr('CustomerNumber')));
    $('#inputCompanyName').val(defaultValues($(obj).attr('lastName')));
    $('#inputContactName').val(defaultValues($(obj).attr('firstName'))); 1
    $('#inputFloor').val(defaultValues($(obj).attr('Floor')));

    $('#inputApartment').val(defaultValues($(obj).attr('Apartment')));
    $('#inputMail').val(defaultValues($(obj).attr('inputMail')));
    $('#inputPhoneOne').val(defaultValues($(obj).attr('AreaPhone1')));
    $('#inputPhone11').val(defaultValues($(obj).attr('Phone1')));
    $('#inputPhoneTwo').val(defaultValues($(obj).attr('AreaPhone2')));
    $('#inputPhone22').val(defaultValues($(obj).attr('Phone2')));
    $('#inputFax').val(defaultValues($(obj).attr('AreaFax')));
    $('#inputFax1').val(defaultValues($(obj).attr('Fax')));
    $('#customerRemarks1').val(defaultValues($(obj).attr('CustomerRemark1')));
    $('#customerRemarks2').val(defaultValues($(obj).attr('CustomerRemark1')));
    $('#stateId').val(defaultValues($(obj).attr('StreetName')));
    $('#cityId').val(defaultValues($(obj).attr('CityName')));
    $('#streetID').val(defaultValues($(obj).attr('StreetName')));
    $('#buildingCode').attr('BuildingCode', defaultValues($(obj).attr('BuildingCode')));
    $('#buildingCode').val(defaultValues($(obj).attr('BuildingNumber')));
    $('#zipcode').val(defaultValues($(obj).attr('ZipCode')));
    $('#visitInterval').val(defaultValues($(obj).attr('VisitInterval')));
}

//clear search fields
function clearSearchFields() {

    $('.container.main_customer_tab input').val('');
}
//Update Customer
function updateCustomer() {
    debugger;
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
        cbuildingNumber: $('#buildingCode').val(),
        cbuildingCode: $('#buildingCode').attr('buildingCode'),
       cZipCode:  $('#zipcode').val(defaultValues($(obj).attr('ZipCode'))),
       cvisitInterval : $('#visitInterval').val(defaultValues($(obj).attr('VisitInterval')))
    }
    $.ajax({
        type: "POST",
        url: "/Customer/UpdateCustomer",
        data: data,
        dataType: "json",
        success: function (response) {
            $('#msgHistory tr:gt(0)').remove();
            if (response)
                return alert("Customer Updated successfully.");
            else
                return alert("Failed to Update.");
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });

}


function defaultValues(val) {
    if (val == '' || val == 'null' || val == '!@#$')
        return '';
    return val;
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

