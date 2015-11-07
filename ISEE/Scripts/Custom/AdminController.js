var module = angular.module('TreeDetails', [])
module.controller('SearchCtrl', function ($scope, ContactService) {
    //  $scope.contacts = ContactService.list();
    var SysIdLevel1 = {};
    var SysIdLevel1max = {};
    var OverallSecondarys = [];


    $scope.$watch('choice', function (value) {
        if (value == '2') {
            $scope.EmployeeSearchData = null;
            $scope.gridOptions = null;
            $scope.EmployeeSearchData = {
                FirstName: '',
                LastName: '',
                phone: ''
            };


        }
        else {

            $scope.CustomerSearchData = null;
            $scope.gridCustOptions = null;
            $scope.CustomerSearchData = {
                state: '',
                city: '',
                street: '',
                buldingNumber: '',
                customerNumber: '',
                contactName: '',
                companyName: '',
                phone1: ''
            };
        }
        // Here i get always the same value
        // console.log("Selected goalType, text: " + value);//
    });
    $scope.selectType = function () {



    }
    //ContactService.getMaxValue().then(function (d) {
    //    var array = $.makeArray(d.data);
    //    Math.max.apply(Math, array.map(function (maxvalue) {
    //        return SysIdLevel1max = maxvalue.RequestSysIdLevel1 + 1;
    //    }))
    //}, function (error) {
    //    alert('Error !');
    //});

    //Category Active DDl
    //ContactService.DDLType().then(function (d) {


    data = [{ 'id': '0', 'name': 'Active' }, { 'id': '-1', 'name': 'InActive' }, { 'id': '', 'name': 'Select All' }]
    $scope.DDLTypeList = data;
    $scope.DDLType = $scope.DDLTypeList[0];
    // });

    $scope.SetSelectedType = function () {


        $scope.contacts = null;
        ContactService.getList($scope.DDLType.id).then(function (d) {

            $scope.contacts = $.makeArray(d.data);
            SysIdLevel1max = d.data[d.data.length - 1].RequestSysIdLevel1 + 1;
        }, function (error) {
            alert('Error !');
        });
        //ContactService.SetSelectedDDLType($scope.DDLType).then(function (d) {
        //    
        //    window.location.reload();
        //    //        $scope.CountryCodeList = d.data;
        //});


    }
    //END




    ContactService.getList($scope.DDLType.id).then(function (d) {

        $scope.contacts = $.makeArray(d.data);
        SysIdLevel1max = d.data[d.data.length - 1].RequestSysIdLevel1 + 1;
    }, function (error) {
        alert('Error !');
    });

    $scope.BindSecondary = function (contact) {
        $scope.SelectedSysIdLevel1 = contact.RequestSysIdLevel1;
        SysIdLevel1 = contact.RequestSysIdLevel1;

        var isVAlExisting = false;

        $.each(OverallSecondarys, function (index, value) {

            var obj = value[0];
            var SyIDvalue = obj.RequestSysIdLevel1;
            if (SyIDvalue == SysIdLevel1) {
                isVAlExisting = true;
                $scope.Secondarys = OverallSecondarys[index];
            }
        });
        //var result = $.grep(OverallSecondarys, function (e) {
        //    return
        //    e.RequestSysIdLevel1 == SysIdLevel1;
        //});
        //if (result.length <= 0) {
        //    $scope.Secondarys = d.data;
        //    OverallSecondarys.push(d.data);
        //    //if (OverallSecondarys = null) {
        //    //    OverallSecondarys.push(d.data);
        //    //}
        //    //else {
        //    //    OverallSecondarys[0].push(d.data);
        //    //}
        //}
        if (!isVAlExisting) {
            ContactService.getSecondarylist(SysIdLevel1).then(function (d) {
                var array = $.makeArray(d.data);
                if (array.length > 0) {
                    $scope.Secondarys = d.data;
                    OverallSecondarys.push(d.data);
                }
                else {
                    $scope.Secondarys = [];
                }
            });
        }
    }
    $scope.editItem = function (Contact) {
        Contact.editing = true;
    }
    $scope.doneEditing = function (Contact) {
        Contact.editing = false;
        //dong some background ajax calling for persistence...
    };
    $scope.editSecondary = function (Secondary) {
        Secondary.editing = true;
    }
    $scope.doneEditingSecondary = function (Secondary) {
        Secondary.editing = false;
        //dong some background ajax calling for persistence...
    };
    $scope.saveContact = function () {
        if ($scope.newcontact != undefined) {
            var newcontact = $scope.newcontact;
            newcontact.RequestSysIdLevel1 = SysIdLevel1max;
            SysIdLevel1max = SysIdLevel1max + 1;
            $scope.SelectedSysIdLevel1 = newcontact.RequestSysIdLevel1;
            // Save in context class
            //ContactService.SaveMainINContext($scope.newcontact).then(function (d) {
            //    $scope.msg = $.makeArray(d.data);
            //})
            $scope.contacts.push($scope.newcontact);
            $scope.newcontact = {};
            $scope.Secondarys = [];
        }
    }
    $scope.saveSecondary = function () {
        if ($scope.newSecondary != undefined) {
            var newSecondary = $scope.newSecondary;
            newSecondary.RequestSysIdLevel1 = $scope.SelectedSysIdLevel1;
            newSecondary.RequestSysIdLevel2 = 0;
            if ($scope.Secondarys.length == 0) {

                OverallSecondarys.push($.makeArray($scope.newSecondary));
            }
            //  OverallSecondarys.push($scope.newSecondary);
            $scope.Secondarys.push($scope.newSecondary);
            //$scope.Secondarys.push($scope.newSecondary);
            $scope.newSecondary = {};
        }
    }
    // code  for Secondary cases
    $scope.OverallSave = function () {
        ContactService.OverallSave($scope.contacts, OverallSecondarys).then(function (d) {

            $scope.msg = "Save SuccessFully";
            //  $window.alert("Save SuccessFully");
        })
    }

    //Tree Tab Code
    $scope.clear = function () {
        if ($scope.choice == "" || $scope.choice == undefined) {
            $scope.clearControlsEmployee();
            $scope.datareturneds = null;


        }
        else if ($scope.choice == "2") {
            $scope.clearControlsCustomers();
            $scope.datareturneds = null;
        }
    }
    $scope.EmployeeSearchData = {
        FirstName: '',
        LastName: '',
        phone: ''
    };

    $scope.CustomerSearchData = {
        state: '',
        city: '',
        street: '',
        buldingNumber: '',
        customerNumber: '',
        contactName: '',
        companyName: '',
        phone1: ''
    };
    //$scope.GetEmployeeData = function () {
    //    ContactService.GetEmployeeData($scope.)

    //}



    $scope.search = function () {
        if ($scope.choice == "" || $scope.choice == undefined) {

            ContactService.GetEmployeeData($scope.EmployeeSearchData).then(function (d) {
                if (d.data.length > 0) {
                    $scope.gridOptions = d.data;
                }
                else {
                    alert('No Records Founded');
                }
            }, function (error) {
                alert('Error !');
            });
        }
        else if ($scope.choice == "2") {

            ContactService.GetCustomersData($scope.CustomerSearchData).then(function (d) {
                if (d.data.length > 0) {
                    $scope.gridCustOptions = d.data;
                }
                else {
                    alert('No Records Founded');
                }
            }, function (error) {
                alert('Error !');
            });
        }
    }
    //ContactService.

    $scope.editEmployee = function (employee) {
        employee.editing = true;
    }
    $scope.doneEditingEmployee = function (employee) {
        debugger;
        if (timeParseExact(employee.Start1) > timeParseExact(employee.End1)) {
            alert('Stop1 date is more then start1');
        }
        else {
            employee.editing = false;
        }

        if (timeParseExact(employee.Start2) > timeParseExact(employee.End2)) {
            alert('Stop2 date is more then start2');
        }
        else {
            employee.editing = false;
        }

    };
    $scope.employeeInfo = {
        Number: '',
        firstname: '',
        startDay: '',
        mail: '',
        lastname: '',
        enddate: '',
        phone1: '',
        phone11: '',
        phone2: '',
        phone22: '',
        ManufactureChoice: '',
        phoneTypeChoice: '',
    };

    ContactService.GetEmployeeHours().then(function (d) {
        $scope.employeeData = d.data;
    })

    $scope.saveEmphour = function () {
        ContactService.SaveEmployeeHours($scope.employeeData).then(function (d) {
            $scope.employeeInfo = null;
        })
    }
    $scope.SaveAllEmployeeData = function (d) {
        ContactService.saveEmployee($scope.employeeInfo).then(function (d) {
            if (d.data != "0") {
                ContactService.SaveEmployeeHours($scope.employeeData, d.data).then(function (d) {
                    if (d == true) {
                        ContactService.GetEmployeeHours().then(function (d) {
                            $scope.employeeData = d.data;
                        })
                    }
                })
                $scope.employeeInfo = null;
                alert('Employee Saved');
            }
            else {
                alert('Please checked fields');
            }


        });
    }
})



module.service('ContactService', function ($http) {
    //to create unique contact id
    var contacts = {};
    contacts.getList = function (d) {

        return $http({
            url: '/Admin/getAll',
            data: { id: d },
            method: 'POST',
            headers: { 'content-type': 'application/json' }
        });
    }
    contacts.getMaxValue = function () {

        return $http({
            url: '/Admin/getMaxValue' + d,
            method: 'GET',
        });
    }
    contacts.getSecondarylist = function (SysIdLevel1) {
        // var SysIdLevel1Val = { SysIdLevel1: SysIdLevel1 };

        return $http({
            url: '/Admin/GetSecondary',
            data: { SysIdLevel1: SysIdLevel1 },
            method: 'POST',

        });
    }
    //contacts.SaveMainINContext = function (NewContact) {
    //    return $http({
    //        url: '/Data/SaveContext',
    //        data: JSON.stringify(NewContact),
    //        method: 'POST',
    //        headers: { 'content-type': 'application/json' }
    //    });
    //}
    contacts.OverallSave = function (main, Secondarys) {

        return $http({
            url: '/Admin/SaveCategory',
            method: 'POST',
            //data: JSON.stringify(main),
            data: { objcategory: JSON.stringify(main), objSecondary: JSON.stringify(Secondarys) },
            headers: { 'content-type': 'application/json' }
        });
        //$http({
        //    url: '/Data/SaveSecondary',
        //    method: 'POST',
        //    data: JSON.stringify(Secondarys),
        //    headers: { 'content-type': 'application/json' }
        //});
    };
    //Tree Tab control
    contacts.GetEmployeeData = function (d) {

        return $http({
            url: '/Admin/GetEmployee',
            method: 'POST',
            //data: JSON.stringify(main),
            data: d,
            headers: { 'content-type': 'application/json' }
        });
        //$http({
        //    url: '/Data/SaveSecondary',
        //    method: 'POST',
        //    data: JSON.stringify(Secondarys),
        //    headers: { 'content-type': 'application/json' }
        //});
    };
    contacts.GetCustomersData = function (d) {

        return $http({
            url: '/Admin/GetCustomers',
            method: 'POST',
            //data: JSON.stringify(main),
            data: d,
            headers: { 'content-type': 'application/json' }
        });
        //$http({
        //    url: '/Data/SaveSecondary',
        //    method: 'POST',
        //    data: JSON.stringify(Secondarys),
        //    headers: { 'content-type': 'application/json' }
        //});
    };

    //Emp.GetEmployees = function (d) {
    //    return $http({
    //        url: '/Admin/GetEmployees',
    //        method: 'POST',
    //        data: d,
    //        headers: { 'content-type': 'application/json' }
    //    });
    //};

    contacts.GetEmployeeHours = function () {

        return $http({
            url: '/Admin/GetEmployeeHours',
            method: 'POST',
        });

    }
    contacts.SaveEmployeeHours = function (d, id) {
        return $http({
            url: '/Admin/SaveEmployeeHours',
            method: 'POST',
            data: { objhours: JSON.stringify(d), EmployeeID: id },
            headers: { 'content-type': 'application/json' }
        });
    }
    contacts.saveEmployee = function (d) {

        return $http({
            url: '/Admin/SaveEmployeeData',
            method: 'POST',
            //data: JSON.stringify(main),
            data: d,
            headers: { 'content-type': 'application/json' }
        });

    }

    return contacts;
});

<<<<<<< HEAD
var count = 0;
module.directive('draggable', function () {
    return {
        // A = attribute, E = Element, C = Class and M = HTML Comment
        restrict: 'A',
        //The link function is responsible for registering DOM listeners as well as updating the DOM.
        link: function (scope, element, attrs) {
            element.draggable({

                helper: "clone",
                start: function (event, ui) {
                    c.tr = this;
                    c.helper = ui.helper//.find("td:first").append("<img src='/images/img/customer.png' />");
                }, drag: function (event, ui) {
                    if (count % 2 == 0) {
                        ui.helper.find("td:first").text("odd")
                        count++;
                    } else {
                        ui.helper.find("td:first").text("even")
                        count++;
                    }
                }
            });
        }
    };
});
=======
//var count = 0;
//module.directive('draggable', function () {
//    return {
//        // A = attribute, E = Element, C = Class and M = HTML Comment
//        restrict: 'A',
//        //The link function is responsible for registering DOM listeners as well as updating the DOM.
//        link: function (scope, element, attrs) {
//            element.draggable({

//                helper: "clone",
//                start: function (event, ui) {
//                    c.tr = this;
//                    c.helper = ui.helper//.find("td:first").append("<img src='/images/img/customer.png' />");
//                }, drag: function (event, ui) {
//                    if (count % 2 == 0) {
//                        ui.helper.find("td:first").text("odd")
//                        count++;
//                    } else {
//                        ui.helper.find("td:first").text("even")
//                        count++;
//                    }
//                }
//            });
//        }
//    };
//});
>>>>>>> e25a5972eea57fd0c2b675eac6f6af04bb3d8a52

function timeParseExact(time) {
    var hhmm = time.split(' ')[0];
    var tt = time.split(' ')[1].toLowerCase();
    var hh = hhmm.split(':')[0];
    var mm = hhmm.split(':')[1];
    if (tt == "pm") {
        if (hh == "12") {
            hh = "0";
        }
        return parseFloat(hh + "." + mm) + 12;
    }
    else {
        if (hh == "12") {
            hh = "0";
        }
        return parseFloat(hh + "." + mm);
    }


}
