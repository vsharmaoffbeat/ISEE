
$(document).ready(function () {
    $(".disabledClass").prop("disabled", true);

    //bind date pickers
    $("#datepickerLastApp,#datepicker1,#datepicker2").datepicker({
        autoclose: true,
        todayHighlight: true
    }).datepicker('update', new Date());

    $("#txtphone11,#txtphone22").keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            return false;
        }
    });
    $("#txtphone1,#txtphone2").keypress(function (event) {
        var regex = new RegExp("^[0-9?=.*!@#$%^&*]+$");
        var key = String.fromCharCode(event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }

    });

    $("#datepickerStartDay,#datepickerEndDay,#datepickerLastApp").datepicker('remove');

    setDefaultValues();
    clearInputFields()

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

    var d = new Date();
    d.setMonth(d.getMonth() - 3);
    $("#datepicker1").datepicker('setDate', d);
    setDefaultDDl();

});
function setDefaultDDl() {
    var date = new Date;
    $('#ddlMonthname [value="' + date.getMonth() + '"]').attr('selected', true);
    $('#ddlYearValue [value="' + date.getFullYear() + '"]').attr('selected', true);
    date.setMonth(date.getMonth() - 3);
    $("#datepicker1").datepicker('setDate', date);

}
function setDatePicker() {
    $("#datepickerStartDay").datepicker({
        todayBtn: 1,
        autoclose: true,
    }).on('changeDate', function (selected) {
        var minDate = new Date(selected.date.valueOf());
        $('#datepickerEndDay').datepicker('setStartDate', minDate);
    });

    $("#datepickerEndDay").datepicker()
        .on('changeDate', function (selected) {
            var minDate = new Date(selected.date.valueOf());
            //     $('#datepickerStartDay').datepicker('setEndDate', minDate);
        });
}
// check before  redirect to map
$('#showMap').click(function () {
    if (parseInt(_employeeId) <= 0)
        alert("Select Employee");
    data = { empId: _employeeId, cusId: 0 }
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
//set details click
function setDetails() {
    if (parseInt(_employeeId) > 0) {
        createTree(treeJsonData, true);
        setDatePicker();
        $('#tblemployeeHours input').attr('disabled', false);
        $(".disabledClass").prop("disabled", false);
        $('.employee_save_header').first().hide();
        $('.employee_save_header').last().show()
    }
    else
        alert("Select Employee");
}
//Cancel click
function removeChange() {
    if (parseInt(_employeeId) > 0) {
        $('#left_employee_window div').each(function () {
            if ($(this).attr('EmployeeId') == _employeeId) {
                setInputValues(this);
                $("#datepickerStartDay,#datepickerEndDay,#datepickerLastApp").datepicker('remove');
                $('.employee_save_header').first().show();
                $(".disabledClass").prop("disabled", true);
                $('.employee_save_header').last().hide();
                $('#tblemployeeHours input').attr('disabled', true);

            }
        })
        createTree(defaultTreeJsonData, false)
    }
}

//Set input fields default values
function setDefaultValues() {
    $('#datepickerStartDay input').val('');
    $('#datepickerEndDay input').val('');
    $('#datepickerLastApp input').val('');
    $('#newemployeeGrid').hide();
}

//Bind Phone Types drop-down
function ManufactureTypes(obj, valMan) {
    $('#ddlphonetype').empty();

    if ($('#ddlmanufacture :selected').val() == "")
        return false;

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
                }).appendTo($('#ddlphonetype'));
            });
            if (obj)
                $('#ddlphonetype [value="' + valMan + '"]').attr('selected', true)
            //$('#ddlphonetype :selected').val(valMan);
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}
//Bind Phone Types drop-down for filter
function ManufactureTypes1() {
    $('#ddlphoneType1').empty();

    if ($('#ddlmanufacture1 :selected').val() == "")
        return false;
    $("<option />").appendTo($('#ddlphoneType1'));
    $.ajax({
        type: "POST",
        url: "/Admin/GetPhoneTypes",
        data: { id: parseInt($('#ddlmanufacture1 :selected').val()) },
        dataType: "json",
        success: function (response) {
            $(response).each(function () {
                $("<option />", {
                    val: this.PhoneTypeCode,
                    text: this.PhoneTypeDesc
                }).appendTo($('#ddlphoneType1'));
            });
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}
//clear input fields
function clearInputFields() {
    // $("#ddlmanufacture").val('');
    $('#empLastname').val('');
    $('#empFirstname').val('');
    $('#empNumber').val('');
    $('#ddlmanufacture1 [value=""]').attr('selected', true)
    $('#ddlphoneType1').empty();

}

//Search for employee's based on filters
function searchEmployeeData() {
    $('#left_employee_window').empty();
    var data = { manufacture: $("#ddlmanufacture1").val(), lastName: $('#empLastname').val(), firstName: $('#empFirstname').val(), empNumber: $('#empNumber').val(), phoneType: $('#ddlphoneType1').val(), isActive: $('#isActive').is(':checked') }
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

            if (response.length <= 0) {


                $('<div class="row" > <div class="col-md-12 col-xs-12 tab_box">No records found.</div></div>').appendTo($('#left_employee_window'));
                return false;
            }
            var setAttr = ''

            $(response).each(function () {
                $('<div class="row" onclick="selectedEmployee(this)" EmployeeId="' + this.EmployeeId + '" EmployeeNum="' + this.EmployeeNum +
                    '" Mail="' + this.Mail + '" FirstName="' + this.FirstName + '" LastName="' + this.LastName +
                    '" LastSendApp="' + this.LastSendApp + '" EndDay="' + this.EndDay + '" PhoneManufactory="' + this.PhoneManufactory +
                    '" PhoneType="' + this.PhoneType +
                    '" MainPhone="' + this.MainPhone + '" MainAreaPhone="' + this.MainAreaPhone +
                    '" SecondAreaPhone="' + this.SecondAreaPhone + '" SecondPhone="' + this.SecondPhone +
                    '" StartDay="' + this.StartDay +

                    '"> <div class="col-md-12 col-xs-12 tab_box">First Name: '
                   + this.FirstName + ' <p>Last Name: ' + this.LastName + '</p><p>Phone1: ' + this.MainAreaPhone + '-' + this.MainPhone
                   + '</p></div></div>').appendTo($('#left_employee_window'));
            });


        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });

}

//Set input fields based on selecetd employee
function selectedEmployee(obj) {
    if (_employeeId == $(obj).attr('EmployeeId'))
        return false;
    $('#ddlmanufacture').find('option[value=""]').remove();
    _employeeId = $(obj).attr('EmployeeId');
    setDefaultDDl();
    removeChange();

    //get messgae history
    //    getMessageHistory(_employeeId, $("#datepicker1 input").val(), $("#datepicker2 input").val());
    //get Employeefill hours
    getEmployeeTimeTemplate(_employeeId);
    //get Employee history template
    // getEmployeeTimeHistoryDiary();
    //Set employee data
    $('#newemployeeGrid').show();
    $('#showMap').attr('href', '/map/map');

}

//Set input fields data
function setInputValues(obj) {

    $('#newemployeeGrid').show();
    $('#newemployeeGrid').html('')
    $('<tr data-oid=' + _employeeId + ' data-id=' + _employeeId + ' data-name=' + $(obj).attr('LastName') + ' ' + $(obj).attr('FirstName') + ' data-type="employee" class="easytree-draggable"><td class="tg-dx8v_category"><i></i><span style="display:none;" id=' + _employeeId + '>' + _employeeId + '</span> </td><td class="tg-dx8v_category" style="text-align:left !important;">' + $(obj).attr('LastName') + ' ' + $(obj).attr('FirstName') + '</td><td class="tg-dx8v_category" style="text-align:left !important;">' + $(obj).attr('MainAreaPhone') + '-' + $(obj).attr('MainPhone') + '</td></tr>').appendTo($('#newemployeeGrid'));


    //  $("#employeeData :input").prop("disabled", false);
    $("#sendApp").prop("disabled", true);
    $('#txtnumber').val($(obj).attr('EmployeeNum'));
    $('#txtmail').val($(obj).attr('Mail'));
    $('#txtfirstname').val($(obj).attr('FirstName'));

    $('#txtlastname').val($(obj).attr('LastName'));

    $('#txtphone1').val($(obj).attr('MainAreaPhone').trim());
    $('#txtphone11').val($(obj).attr('MainPhone').trim());
    $('#txtphone2').val($(obj).attr('SecondAreaPhone').trim());
    $('#txtphone22').val($(obj).attr('SecondPhone').trim());
    $('#txtStart').val($(obj).attr('StartDay'));

    //$('#ddlmanufacture').val($(obj).attr('PhoneManufactory'));
    $('#ddlmanufacture [value="' + $(obj).attr('PhoneManufactory') + '"]').attr('selected', true)
    // bindDdlphonetype($(obj).attr('PhoneManufactory'));

    ManufactureTypes(true, $(obj).attr('PhoneType'))
    //  $('#ddlphonetype').val($(obj).attr('PhoneType'));
    $('#txtapplication').val($(obj).attr('LastSendApp'));
    $('#txtend').val($(obj).attr('EndDay'));

    //Employee tree changes sunday
    //$('#employoeeDrag').empty();
    // $('<table class="tg"><tr id="' + _employeeId + '"><td class="tg-dx8v">' + $(obj).attr("FirstName") + '</td><td class="tg-dx8v">' + $(obj).attr("MainAreaPhone") + ' - ' + $(obj).attr("MainPhone") + '</td></tr></table>').appendTo($('#employoeeDrag'));




}

//message history data
function searchMessageHistory() {
    if (parseInt(_employeeId) <= 0) {
        alert("Select Employee");
        return false;
    }
    getMessageHistory(_employeeId, $("#datepicker1 input").val(), $("#datepicker2 input").val())
}

//Get sms history and bind grid
function getMessageHistory(id, start, end) {
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


        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}
//Send Sms
function sendSms() {
    if (parseInt(_employeeId) <= 0)
        alert("Select Employee");
    if ($('#txtArea').val().trim() == "" || $('#txtphone1').val().trim() == "" || $('#txtphone11').val().trim() == "")
        return false;
    data = { employeeId: _employeeId, msg: $('#txtArea').val().trim(), phoneNumber: $('#txtphone1').val().trim() + $('#txtphone11').val().trim() }
    $.ajax({
        type: "POST",
        url: "/Data/SendMessage",
        data: data,
        dataType: "json",
        success: function (response) {
            if (response) {
                getMessageHistory(_employeeId, $("#datepicker1 input").val(), $("#datepicker2 input").val());
                $('#txtArea').val('')
                alert('Message sent');
            }
            else
                alert('Failed to sent');
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.responseText);
        }
    });
}
//End Sms Tab

//Start Time tab
// data for Employee Time Template
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

            $('#tblemployeeHours tr:gt(0)').remove();
            var counter = 0;
            var ids = '';
            $(response).each(function () {
                $('<tr id="' + this.DayStatus + '"><td class="tg-dx8v"></td><td class="tg-dx8v">' + this.Day + '</td><td class="tg-dx8v"><input id="Start1' + counter + '" value="' + this.Start1 + '" class="inputClass start1" Start="Start1"></input></td><td class="tg-dx8v"><input id="end1' + counter + '"  value="' + this.End1 + '" class="inputClass end1" end="end1"></input></td><td class="tg-dx8v"><input id="Start2' + counter + '"  value="' + this.Start2 + '" class="inputClass start2" Start="Start2"></input></td><td class="tg-dx8v"><input id="end2' + counter + '"  value="' + this.End2 + '" class="inputClass end2" end="end2"></input></td><td class="tg-dx8v"></td></tr>').appendTo($('#tblemployeeHours'));;
                ids += '#Start1' + counter + ',' + '#end1' + counter + ',' + '#Start2' + counter + ',' + '#end2' + counter + ','
                counter++;
            });
            ids = ids.substring(0, ids.length - 1)
            $(ids).timepicker({
                minTime: '00',
                maxTime: '23:30',
                scrollbar: true,
            });

            $('#tblemployeeHours input').attr('disabled', true);
        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}

//data for Employee Time History Diary
function getEmployeeTimeHistoryDiary() {
    // ddlMonthname
    if (parseInt(_employeeId) <= 0) {
        alert("Select Employee");
        return false;
    }
    data = { employeeId: _employeeId, month: $('#ddlMonthname :selected').val(), year: $('#ddlYearValue :selected').val() }
    $.ajax({
        type: "POST",
        url: "/Data/GetEmployeeTimeHistoryDiary",
        data: data,
        dataType: "json",
        success: function (response) {
            if (response == null) {
                return true;
            }
            $('#tblEmpDiaryHistory tr:gt(0)').remove();
            $(response).each(function () {
                $('<tr><td class="tg-dx8v"></td><td class="tg-dx8v">' + this.Day + '</td><td class="tg-dx8v">' + replaceNullWithEmpty(this.Start1) + ' ' + replaceNullWithEmpty(this.End1) + '</td><td class="tg-dx8v">' + replaceNullWithEmpty(this.Start2) + ' ' + replaceNullWithEmpty(this.End2) + '</td><td class="tg-dx8v">' + replaceNullWithEmpty(this.Start3) + ' ' + replaceNullWithEmpty(this.End3) + '</td><td class="tg-dx8v"></td></tr>').appendTo($('#tblEmpDiaryHistory'));;
            });


        },
        error: function (xhr, ajaxOptions, thrownError) { alert(xhr.responseText); }
    });
}
//validation methods
function replaceNullWithEmpty(obj) {
    if (obj == null)
        return "";
    return obj
}
//End Time tab

//Update Employee
function updateEmployee() {
    if (parseInt(_employeeId) > 0) {
        if (!ValidateEmail($("#txtmail").val())) {
            alert("Invalid email address.");
            return false;
        }
        var houlyFilled = getHourData();
        d = {
            employeeId: _employeeId,
            number: $('#txtnumber').val(),
            mail: $('#txtmail').val(),
            firstName: $('#txtfirstname').val(),
            lastName: $('#txtlastname').val(),
            phone1: $('#txtphone1').val().trim(),
            phone11: $('#txtphone11').val().trim(),
            phone2: $('#txtphone2').val().trim(),
            phone22: $('#txtphone22').val().trim(),
            Start: $('#txtStart').val(),
            manufacture: $('#ddlmanufacture').val(),
            // bindDdlphonetype($(obj).attr('PhoneManufactory'));

            //ManufactureTypes(true, $(obj).attr('PhoneType'))
            phoneType: $('#ddlphonetype').val(),
            //   $('#txtapplication').val();
            end: $('#txtend').val(),
            hourlyData: houlyFilled
        }

        $.ajax({
            type: "POST",
            url: "/Data/UpdateEmployee",
            data: d,
            dataType: "json",
            success: function (response) {
                if (response) {

                    saveTree();
                    $('#left_employee_window div').each(function () {
                        if ($(this).attr('EmployeeId') == _employeeId) {
                            $(this).attr('EmployeeNum', d.number);
                            $(this).attr('Mail', d.mail);
                            $(this).attr('FirstName', d.firstName);
                            $(this).attr('LastName', d.lastName);
                            $(this).attr('EndDay', d.end);
                            $(this).attr('PhoneType', d.phoneType);
                            $(this).attr('MainPhone', d.phone11);
                            $(this).attr('MainAreaPhone', d.phone1);
                            $(this).attr('StartDay', d.Start);
                            $(this).attr('SecondAreaPhone', d.phone2);
                            $(this).attr('SecondPhone', d.phone22);

                            $(this).attr('PhoneManufactory', d.manufacture);
                            $(this).empty();
                            $('<div class="col-md-12 col-xs-12 tab_box">First Name: ' + d.firstName + ' <p>Last Name: ' + d.lastName + '</p><p>Phone1: ' + d.phone1 + '-' + d.phone11 + '</p></div>').appendTo($(this));
                        }
                    })
                    removeChange();
                }
                else
                    alert("Failed to update.");
            },
            error: function (xhr, ajaxOptions, thrownError) {


                alert(xhr.responseText);
            }
        });


        //$('#txtend').val();
    }
}

//get hours grid data
function getHourData() {
    //var data = { day: '', start1: '', start2: '', end1: '', end2: '' };
    var dataList = [];
    $("#tblemployeeHours tr").each(function () {
        //if ($(this).find('input').length > 0) {
        //    $(this).find('input').attr('start') == 'Start1'
        //    '';// data.sta
        //}
        var data = {};
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
        })
        if ($(this).find('td').find('input').length > 0) {
            data.Day = this.id;
            dataList.push(data);
        }
    })
    return JSON.stringify(dataList);
}


//Employee Scheduler
function showEmployeeScheduler() {
    if (parseInt(_employeeId) > 0) {
        $("#employeeSchdulerDialog").dialog({
            width: 1000,
            height: 600,
            close: function (event, ui) {
                $('#googleMap').dialog('close');
            },
            open: function (event, ui) {

                var appElement = document.querySelector('[ng-controller=SchedulerController]');
                var $scope = angular.element(appElement).scope();

                scheduler.clearAll()
                scheduler.config.serverLists = {};
                scheduler.config.details_on_create = true;
                scheduler.config.details_on_dblclick = true;
                scheduler.config.prevent_cache = true;
                scheduler.config.start_on_monday = false;
                scheduler.config.first_hour = new Date("1/1/2001 " + $scope.schdulerStartTime).getHours();
                scheduler.config.last_hour = new Date("1/1/2001 " + $scope.schdulerEndTime).getHours();
                scheduler.config.dblclick_create = false; //false to create new event on double click
                scheduler.config.details_on_dblclick = false;
                scheduler.config.readonly = true;
                scheduler.config.show_loading = true;
                scheduler.config.api_date = "%Y-%m-%d %h:%i %A";
                scheduler.config.drag_create = false;//false to create new event on drag
                scheduler.init('scheduler_here', new Date(todayDate));
                dp = scheduler.dataProcessor = new dataProcessor("/Calendar/Save");
                dp.init(scheduler);
                dp.setTransactionMode("POST", false);

                var getEventsUrl = "/Calendar/Data?ID=" + _employeeId + "&startTime=" + $scope.schdulerStartTime + "&endTime=" + $scope.schdulerEndTime
                scheduler.setLoadMode("week");
                scheduler.load(getEventsUrl, "json");

                /*Click of Event Rendered on Schdulerd*/
                scheduler.attachEvent("onClick", function (id, e) {
                    //any custom logic here
                    $("#searchSection").flip('show');
                    if (parseInt(selectedEventID) > 0 && selectedEventID !== id) {
                        scheduler.load(getEventsUrl, "json");
                    }
                    var appElement = document.querySelector('[ng-controller=SchedulerController]');
                    var $scope = angular.element(appElement).scope();
                    $scope.$apply(function () {
                        $scope.SelectedEventDate = scheduler.getEvent(id).start_date.toLocaleDateString("en-US") + ' ' + scheduler.getEvent(id).start_date.toLocaleTimeString("en-US") + '-' + scheduler.getEvent(id).end_date.toLocaleTimeString("en-US")
                    });

                    selectedEventID = id;
                    if ($('#chkMap').prop('checked')) {
                        var lat = scheduler.getEvent(id).latitude;
                        var long = scheduler.getEvent(id).longtitude;

                        if ($('#googleMap').length > 0) {
                            $('#googleMap').remove();
                        }


                        $("<div id='googleMap' style'width:500px;height:380px;'></div>").dialog(
                            {
                                width: 500,
                                height: 380,
                                open: function (event, ui) {
                                    initialize(lat, long)
                                },
                                close: function (event, ui) {
                                    $('#googleMap').remove();
                                }
                            });
                    }
                    return false;
                });


            }
        });
    }
}


//initialize map
function initialize(lat, long) {

    var mapProp = {
        center: new google.maps.LatLng(lat, long),
        zoom: 5,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

    var marker = new google.maps.Marker({
        position: new google.maps.LatLng(lat, long),
        map: map,
    });
}


function saveTree() {
    var treeViewData = JSON.stringify(objEmployeeTree.getAllNodes());
    $.ajax({
        type: "POST",
        url: "/Admin/SaveTreeViewData", data: { treeViewData: treeViewData }, dataType: "json", success: function (result) {
            //$scope.$apply(function () {
            if (result.Message == "Success") {
                treeJsonData = JSON.parse(result.NewTreeJson)
                treeEmployeeJsonData = JSON.parse(result.NewTreeJson)
                treeCustomerJsonData = JSON.parse(result.NewTreeJson)
                alert("Updated Employee.");
                // $scope.ShowMessageBox("Message", "Tree saved successfully.")
            }
            //else {
            //    //$scope.ShowMessageBox("Error", result.ErrorDetails)
            //}
            //});
        }
    });
}


////create tree
function createTree(treeJson, isDnd) {
    objEmployeeTree = $('#jstree_Employee_div').easytree(
        {
            data: treeJson,
            enableDnd: isDnd,
            canDrop: canDropEmployee,
            dropped: droppedEmployee,
            dropping: droppingEmployee
        });
}
function ValidateEmail(email) {
    if (email == '')
        return true;
    var expr = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    return expr.test(email);
}