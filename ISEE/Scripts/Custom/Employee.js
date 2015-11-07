var _employeeId = 0;
//var manufacture = JSON.parse('@Html.Raw(ViewBag.JsonData)');
$(document).ready(function () {
    $("#datepickerStartDay,#datepickerEndDay,#datepickerLastApp,#datepicker1,#datepicker2").datepicker({
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
    setDefaultValues();
    clearInputFields()
    debugger;
    var date = new Date,
    years = [],
    year = date.getFullYear();
  $('#ddlYearValue').empty();
    for (var i = year; i > year - 5; i--) {
        $("<option />", {
            val: i,
            text: i
        }).appendTo($('#ddlYearValue'));
    }
  
 //   $("<option />").appendTo($('#ddlYearValue'));
    

    $(manufacture).each(function () {
        $("<option />", {
            val: this.PhoneManufacturId,
            text: this.PhoneManufacture1
        }).appendTo($('#ddlmanufacture'));
    });


    $('#ddlmanufacture').empty();
    $("<option />").appendTo($('#ddlmanufacture'));
    $(manufacture).each(function () {
        $("<option />", {
            val: this.PhoneManufacturId,
            text: this.PhoneManufacture1
        }).appendTo($('#ddlmanufacture'));
    });
});

function setDefaultValues() {
    $("#employeeData :input").prop("disabled", true);
    $('#datepickerStartDay input').val('');
    $('#datepickerEndDay input').val('');
    $('#datepickerLastApp input').val('');
    $('#datepicker1 input').val('');
    $('#datepicker2 input').val('');
    // $("#datepickerStartDay,#datepickerEndDay,#datepickerLastApp,#datepicker1,#datepicker2")

}
//Bind Phone Types ddl
function ManufactureTypes(obj) {
    $('#ddlphoneType').empty();
    $("<option />").appendTo($('#ddlphoneType'));
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

//clear input fields
function clearInputFields() {
    $("#ddlmanufacture").val('');
    $('#empLastname').text('');
    $('#empFirstname').text('');
    $('#empNumber').text('');
    $('#ddlphoneType').empty();

}

//Search function for employee
function searchEmployeeData() {
    $('#left_employee_window').empty();
    var data = { manufacture: $("#ddlmanufacture").val(), lastName: $('#empLastname').val(), firstName: $('#empFirstname').val(), empNumber: $('#empNumber').val(), phoneType: $('#ddlphoneType').val() }
    $.ajax({
        type: "POST",
        url: "/Data/GetEmployee",
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
                if (this.LastSendApp == null && this.EndDay != null) {

                    setAttr = this.EmployeeId + '|' + this.EmployeeNum + '|' + this.Mail + '|' + this.FirstName + '|' + this.LastName + '|'
                     + this.StartDay.replace(/\/Date\((-?\d+)\)\//, '$1') + '|' + this.MainAreaPhone + '|' + this.MainPhone + '|' + this.SecondAreaPhone + '|'
                     + this.SecondPhone + '||' + this.EndDay.replace(/\/Date\((-?\d+)\)\//, '$1')

                }
                else if (this.LastSendApp != null && this.EndDay == null) {

                    setAttr = this.EmployeeId + '|' + this.EmployeeNum + '|' + this.Mail + '|' + this.FirstName + '|' + this.LastName + '|'
                     + this.StartDay.replace(/\/Date\((-?\d+)\)\//, '$1') + '|' + this.MainAreaPhone + '|' + this.MainPhone + '|' + this.SecondAreaPhone + '|'
                     + this.SecondPhone + '|' + this.LastSendApp.replace(/\/Date\((-?\d+)\)\//, '$1')
                    '|' + this.LastSendApp.replace(/\/Date\((-?\d+)\)\//, '$1') + '||';

                }
                else if (this.LastSendApp == null && this.EndDay == null)
                    setAttr = this.EmployeeId + '|' + this.EmployeeNum + '|' + this.Mail + '|' + this.FirstName + '|' + this.LastName + '|'
                       + this.StartDay.replace(/\/Date\((-?\d+)\)\//, '$1') + '|' + this.MainAreaPhone + '|' + this.MainPhone + '|' + this.SecondAreaPhone + '|'
                       + this.SecondPhone + '|||';

                else
                    setAttr = this.EmployeeId + '|' + this.EmployeeNum + '|' + this.Mail + '|' + this.FirstName + '|' + this.LastName + '|'
                   + this.StartDay.replace(/\/Date\((-?\d+)\)\//, '$1') + '|' + this.MainAreaPhone + '|' + this.MainPhone + '|' + this.SecondAreaPhone + '|'
                   + this.SecondPhone + '|' + this.LastSendApp.replace(/\/Date\((-?\d+)\)\//, '$1') +
                '|' + this.EndDay.replace(/\/Date\((-?\d+)\)\//, '$1') + '|';


                $('<div class="row" onclick="selectedEmployee(this)" employeeData="' + setAttr + '"> <div class="col-md-12 col-xs-12 tab_box">First Name: '
                   + this.FirstName + ' <p>Last Name: ' + this.LastName + '</p><p>Phone1: ' + this.MainAreaPhone + '-' + this.MainPhone
                   + '</p></div></div>').appendTo($('#left_employee_window'));
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
    var data = $(obj).attr('employeedata').split('|');
    getMessageHistory(data[0], $("#datepicker1 input").val(), $("#datepicker2 input").val())
    _employeeId = data[0];
    setInputValues(data);
    // alert('employee slected;')
}
function getMessageHistory(id, start, end) {
    data = { employeeId: id, start: start, end: end }
    $.ajax({
        type: "POST",
        url: "/Data/GetMessageHistory",
        data: data,
        dataType: "json",
        success: function (response) {
            if (response == null) {
                return true;
            }
            $(response).each(function () {
                $('<tr><td class="tg-dx8v">' + this.SmsCreatDate.replace(/\/Date\((-?\d+)\)\//, '$1') + '</td><td class="tg-dx8v">' + this.SmsStatus + '</td><td class="tg-dx8v">' + this.SmsMsg + '</td><td class="tg-dx8v">' + this.SmsCount + '</td><td class="tg-dx8v"></td></tr>').appendTo($('#msgHistory tr'));;
            });
            debugger;

        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}
//Time tab
function getEmployeeTimeTemplate(id) {
    data = { employeeId: id }
    $.ajax({
        type: "POST",
        url: "/Data/GetEmployeeDiaryTemplate",
        data: data,
        dataType: "json",
        success: function (response) {
            if (response == null) {
                return true;
            }
            $(response).each(function () {
                $('<tr><td class="tg-dx8v">' + this.SmsCreatDate.replace(/\/Date\((-?\d+)\)\//, '$1') + '</td><td class="tg-dx8v">' + this.SmsStatus + '</td><td class="tg-dx8v">' + this.SmsMsg + '</td><td class="tg-dx8v">' + this.SmsCount + '</td><td class="tg-dx8v"></td></tr>').appendTo($('#msgHistory tr'));;
            });
            debugger;

        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

function getEmployeeTimeHistoryDiary(id, month, year) {
    ddlMonthname
   
    data = { employeeId: id, month: $('#ddlMonthname :selected').val(), year: $('#ddlYearValue :selected').val() }
    $.ajax({
        type: "POST",
        url: "/Data/GetEmployeeTimeHistoryDiary",
        data: data,
        dataType: "json",
        success: function (response) {
            if (response == null) {
                return true;
            }
            $(response).each(function () {
                $('<tr><td class="tg-dx8v">' + this.SmsCreatDate.replace(/\/Date\((-?\d+)\)\//, '$1') + '</td><td class="tg-dx8v">' + this.SmsStatus + '</td><td class="tg-dx8v">' + this.SmsMsg + '</td><td class="tg-dx8v">' + this.SmsCount + '</td><td class="tg-dx8v"></td></tr>').appendTo($('#msgHistory tr'));;
            });
            debugger;

        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

function setInputValues(data) {

    $("#employeeData :input").prop("disabled", false);
    $('#txtnumber').val(data[1]);
    $('#txtmail').val(data[2]);
    $('#txtfirstname').val(data[3]);

    $('#txtlastname').val(data[4]);

    $('#txtphone1').val(data[6]);
    $('#txtphone11').val(data[7]);
    $('#txtphone2').val(data[8]);
    $('#txtphone22').val(data[9]);
    $('#txtStart').val(getDateFormat(Date(data[5])));
    if (data[10] != "")
        $('#txtapplication').val(getDateFormat(Date(data[10])));
    if (data[11] != "")
        $('#txtend').val(getDateFormat(Date(data[11])));
}
function getDateFormat(d) {
    return ('' + (1 + d.getMonth()) + '-' + d.getDate() + '-' + d.getFullYear().toString())
}
//$('.menu_tabs').click(function (data) {

//    debugger;
//    $(this).find('a').tab('show');
//    //$('.menu_tabs').removeClass('active');
//    //if ($(this).attr('id').indexOf('Time') != -1){
//    //    $('#employeeHours').addClass('active');
//    //    $(this).addClass('active');
//    //    $('#employeeTree').removeClass('active');
//    //    $('#employeeSms').removeClass('active');
//    //}
//    //else if ($(this).attr('id').indexOf('tree') != -1) {
//    //    $('#employeeSms').addClass('active');
//    //    $(this).addClass('active');
//    //    $('#employeeTree').removeClass('active');
//    //    $('#employeeHours').removeClass('active');
//    //}
//    //else if ($(this).attr('id').indexOf('Sms') != -1) {
//    //    $('#employeeTree').addClass('active');
//    //    $(this).addClass('active');
//    //    $('#employeeHours').removeClass('active');
//    //    $('#employeeSms').removeClass('active');
//    //}
//    //$('#employeeHours')

//});
