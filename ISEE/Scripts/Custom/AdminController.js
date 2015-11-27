var module = angular.module('TreeDetails', [])

module.controller('SearchCtrl', function ($scope, ContactService) {
    //  $scope.contacts = ContactService.list();
    var SysIdLevel1 = {};
    var SysIdLevel1max = {};
    var OverallSecondarys = [];
    $scope.SelectedSysIdLevel1;

    //Common PopUP
    $scope.MessageBoxModal = {
        HeaderTitle: '',
        Content: ''
    };

    $scope.ShowMessageBox = function (headerTitle, content) {
        $scope.MessageBoxModal.HeaderTitle = headerTitle;
        $scope.MessageBoxModal.Content = content;
        $('#myModal').modal('show');
    };

    //Common PopUP End


    //Admin Tree Tab variable
    $scope.SelectedAdminTreeEmployee;
    $scope.SelectedAdminTreeCustomer;
    //Admin Tree Tab variable End


    //Admin Tree Tab Methods
    $scope.setVariable = function (gridOption) {
        if (gridOption != undefined) {
            if ($scope.choice == "2") {
                $scope.SelectedAdminTreeCustomer = gridOption.id;
            } else {
                $scope.SelectedAdminTreeEmployee = gridOption.id;
            }
        }
    }

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
                phoneArea: '',
                phone1: ''
            };
        }
        // Here i get always the same value
        // console.log("Selected goalType, text: " + value);//
    });

    $scope.selectType = function () {
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

    //Clear Employee Controls
    $scope.clearControlsEmployee = function () {
        $scope.EmployeeSearchData = {
            FirstName: '',
            LastName: '',
            phone: ''
        };
    }

    //Clear Customer Controls
    $scope.clearControlsCustomers = function () {
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

    //Employee Search Model
    $scope.EmployeeSearchData = {
        FirstName: '',
        LastName: '',
        phone: ''
    };

    //Customer Search Model
    $scope.CustomerSearchData = {
        state: '',
        city: '',
        street: '',
        buldingNumber: '',
        customerNumber: '',
        contactName: '',
        companyName: '',
        phoneArea: '',
        phone1: ''
    };

    //Search Function
    $scope.search = function () {
        if ($scope.choice == "" || $scope.choice == undefined) {
            ContactService.GetEmployeeData($scope.EmployeeSearchData).then(function (d) {
                if (d.data.length > 0) {
                    $scope.gridOptions = d.data;
                }
                else {
                    $scope.ShowMessageBox('Message', 'No Records Founded');
                }
            }, function (error) {
                $scope.ShowMessageBox('Message', 'An Error has been occured....');
            });
        }
        else if ($scope.choice == "2") {

            ContactService.GetCustomersData($scope.CustomerSearchData).then(function (d) {
                if ($scope.CustomerSearchData.state == '0') {
                    $scope.CustomerSearchData.state = '';
                }

                if ($scope.CustomerSearchData.city == '0') {
                    $scope.CustomerSearchData.city = '';
                }
                if ($scope.CustomerSearchData.street == '0') {
                    $scope.CustomerSearchData.street = '';
                }
                if (d.data.length > 0) {
                    $scope.gridCustOptions = d.data;
                }
                else {
                    $scope.ShowMessageBox('Message', 'No Records Founded');
                }
            }, function (error) {
                $scope.ShowMessageBox('Message', 'An Error has been occured....');
            });
        }
    }
    //Admin Tree Tab Methods End


    //Admin Category Tab Methods

    data = [{ 'id': '0', 'name': 'Active' }, { 'id': '-1', 'name': 'InActive' }, { 'id': '', 'name': 'Select All' }]
    $scope.DDLTypeList = data;
    $scope.DDLType = $scope.DDLTypeList[0];
    $scope.SetSelectedType = function () {
        $scope.contacts = null;
        ContactService.getList($scope.DDLType.id).then(function (d) {
            $scope.contacts = $.makeArray(d.data);
            if ($scope.contacts.length > 0) {
                SysIdLevel1max = d.data[d.data.length - 1].RequestSysIdLevel1 + 1;
                $scope.BindSecondary($scope.contacts[0]);
            }
        }, function (error) {
            $scope.ShowMessageBox('Error', 'An Error has been occured ....');
        });
    }

    ContactService.getList($scope.DDLType.id).then(function (d) {
        $scope.contacts = $.makeArray(d.data);

        if ($scope.contacts.length > 0) {
            SysIdLevel1max = d.data[d.data.length - 1].RequestSysIdLevel1 + 1;
            $scope.BindSecondary($scope.contacts[0]);
        }
    }, function (error) {
        $scope.ShowMessageBox('Error', 'An Error has been occured ....');
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

        if (!isVAlExisting) {
            ContactService.getSecondarylist(SysIdLevel1, $scope.DDLType.id).then(function (d) {
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
            newcontact.StatusCode = 0
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
            newSecondary.StatusCode = 0;
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
            $scope.ShowMessageBox('Message', 'Save SuccessFully.')
            //  $window.alert("Save SuccessFully");
        })
    }

    $scope.editEmployee = function (employee) {
        employee.editing = true;
    }
    $scope.doneEditingEmployee = function (employee) {
        if (timeParseExact(employee.Start1) > timeParseExact(employee.End1)) {
            $scope.ShowMessageBox('Message', ' Stop1 date is more then start1')
        }
        else {
            employee.editing = false;
        }

        if (timeParseExact(employee.Start2) > timeParseExact(employee.End2)) {
            $scope.ShowMessageBox('Message', ' Stop2 date is more then start2')
        }
        else {
            employee.editing = false;
        }

    };

    $scope.editEmployeeHour = function (employeeHour, propName) {
        switch (propName) {
            case 'Start1':
                employeeHour.editingStart1 = true;
                break;
            case 'End1':
                employeeHour.editingEnd1 = true;
                break;
            case 'Start2':
                employeeHour.editingStart2 = true;
                break;
            case 'End2':
                employeeHour.editingEnd2 = true;
                break;
        }

    }
    $scope.doneEditingEmployeeHour = function (employeeHour, propName) {
        if (timeParseExact(employeeHour.Start1) > timeParseExact(employeeHour.End1)) {

            if (employeeHour.End1 != undefined)
                $scope.ShowMessageBox('Message', ' Stop1 date is more then start1')
        }
        else if (timeParseExact(employeeHour.Start2) > timeParseExact(employeeHour.End2)) {

            if (employeeHour.End2 != undefined)
                $scope.ShowMessageBox('Message', ' Stop2 date is more then start2')

        }

        switch (propName) {
            case 'Start1':
                employeeHour.editingStart1 = false;
                break;
            case 'End1':
                employeeHour.editingEnd1 = false;
                break;
            case 'Start2':
                employeeHour.editingStart2 = false;
                break;
            case 'End2':
                employeeHour.editingEnd2 = false;
                break;
        }


    };

    $scope.hideSecondayRow = function (secondary) {
        if ($scope.DDLType.id == '') {
            return falses;
        } else {
            return !($scope.DDLType.id == secondary.StatusCode);
        }
    }

    //Admin Category Tab Methods Ends







    //ContactService.


    // Admin Employee Tab Methods
    $scope.clearEmployeeDetails = function myfunction() {
        $scope.NewAddedEmployee = null;
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
        $scope.EmployeeID = 0;
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

    $scope.EmployeeID = 0;
    $scope.SaveAllEmployeeData = function () {
        var msg = validateEmployee($scope.employeeInfo);
        if (msg.trim() != '') {
            $scope.ShowMessageBox('Message', msg);
            return false;
        }
        if ($scope.EmployeeID == 0) {
            ContactService.saveEmployee($scope.employeeInfo).then(function (d) {
                if (d.data != "0") {
                    $scope.EmployeeID = d.data;
                    ContactService.SaveEmployeeHours($scope.employeeData, d.data).then(function (d) {
                        if (d.data != "0" || d.data == true) {
                            //ContactService.GetEmployeeHours().then(function (d) {
                            //    $scope.employeeData = d.data;
                            //})
                            ContactService.GetEmployeeByEmployeeID($scope.EmployeeID).then(function (responce) {
                                $scope.NewAddedEmployee = responce.data;
                            })

                            $scope.ShowMessageBox('Message', ' Employee Saved')
                        }
                    })
                }
            });
        } else {
            $scope.saveTree();
        }
    }
    // Admin Employee Tab Methods


    // Admin Customer Tab Methods
    $scope.HasStateActive = "true";
    $scope.clearCustomerDetails = function myfunction() {
        $scope.NewCustomerDetails = null;
        $scope.CustomerInfo = {
            ContactName: '',
            CompanyName: '',
            Apartment: '',
            Phone2: '',
            Mail: '',
            AreaFax: '',
            Fax: '',
            CustomerNumber: '',
            Floor: '',
            Phone1: '',
            PhoneArea1: '',
            PhoneArea2: '',
            VisitInterval: '',
            VisitTime: '',
            NextVisit: '',
            BuldingCode: '',
            MobileOne: '',
            MobileTwo: '',
            BuildingNumber: '',
            State: '',
            City: '',
            Street: '',
            Entry: '',
            ZipCode: ''
        };
        $scope.CustomerID = 0;
    };
    $scope.NewCustomerDetails;
    $scope.CustomerInfo = {
        ContactName: '',
        CompanyName: '',
        Apartment: '',
        Phone2: '',
        Mail: '',
        AreaFax: '',
        Fax: '',
        CustomerNumber: '',
        Floor: '',
        Phone1: '',
        PhoneArea1: '',
        PhoneArea2: '',
        VisitInterval: '',
        VisitTime: '',
        NextVisit: '',
        BuldingCode: '',
        MobileOne: '',
        MobileTwo: '',
        BuildingNumber: '',
        State: '',
        City: '',
        Street: '',
        Entry: '',
        ZipCode: ''
    };
    $scope.CustomerID = 0
    $scope.SaveCustomerForm = function () {
        $scope.CustomerInfo.BuldingCode = buildingCode;

        if ($scope.CustomerID == 0) {
            if ($scope.CustomerInfo.BuldingCode > 0) {
                if ($scope.CustomerInfo.BuildingNumber != '') {
                    if ($scope.CustomerInfo.CompanyName != '') {
                        $.ajax({
                            url: "/Admin/SaveCustomerForm",
                            type: "post",
                            contentType: "application/json",
                            data: JSON.stringify({ objCustomerData: $scope.CustomerInfo }),
                            dataType: "json",
                            success: function (result) {
                                $scope.$apply(function () {
                                    if (result.Message == 'Success') {
                                        $scope.NewCustomerDetails = result.CustomerDetails;
                                        $scope.CustomerID = result.CustomerDetails.CustomerID
                                        $scope.ShowMessageBox('Save Message', 'Customer data saved sucessfully.')
                                    } else {
                                        $scope.ShowMessageBox('Error', result.ErrorDetails)
                                    }
                                });
                            }
                        });
                    }
                    return false;
                }
            } else {

                $scope.ShowMessageBox('Message', 'Must select address first.')

            }
        } else {
            $scope.saveTree();
        }


    };
    // Admin Customer Tab Methods Ends

    //Common Method To Save Tree Data
    $scope.saveTree = function () {
        var treeViewData = JSON.stringify(objTree.getAllNodes());
        $.ajax({
            type: "POST",
            url: "/Admin/SaveTreeViewData", data: { treeViewData: treeViewData }, dataType: "json", success: function (result) {
                $scope.$apply(function () {
                    if (result.Message == "Success") {
                        treeJsonData = JSON.parse(result.NewTreeJson)
                        treeEmployeeJsonData = JSON.parse(result.NewTreeJson)
                        treeCustomerJsonData = JSON.parse(result.NewTreeJson)

                        $scope.ShowMessageBox("Message", "Tree saved successfully.")
                    } else {
                        $scope.ShowMessageBox("Error", result.ErrorDetails)
                    }
                });
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
    contacts.getSecondarylist = function (SysIdLevel1, contactStatus) {
        // var SysIdLevel1Val = { SysIdLevel1: SysIdLevel1 };

        return $http({
            url: '/Admin/GetSecondary',
            data: { SysIdLevel1: SysIdLevel1, ContactStatus: contactStatus },
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
        if (d.state == '') {
            d.state = '0';
        }

        if (d.city == '') {
            d.city = '0';
        }
        if (d.street == '') {
            d.street = '0';
        }
        return $http({
            url: '/Admin/GetCustomersNew',
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
    contacts.GetEmployeeByEmployeeID = function (d) {

        return $http({
            url: '/Admin/GetEmployeeByEmployeeID',
            method: 'POST',
            //data: JSON.stringify(main),
            data: { EmployeeID: d },
        });

    }

    return contacts;
});

module.directive('focusOnShow', function ($timeout) {
    return {
        restrict: 'A',
        link: function ($scope, $element, $attr) {
            if ($attr.ngShow) {
                $scope.$watch($attr.ngShow, function (newValue) {
                    if (newValue) {
                        $timeout(function () {
                            $element.focus();
                            if ($element.data("showtime") == true) {
                                showTime($element)
                                $element.click()
                            }
                        }, 0);
                    }
                })
            }
            if ($attr.ngHide) {
                $scope.$watch($attr.ngHide, function (newValue) {
                    if (!newValue) {
                        $timeout(function () {
                            $element.focus();
                            if ($element.data("showtime") == true) {
                                showTime($element)
                                $element.click()
                            }
                        }, 0);
                    }
                })
            }

        }
    };
})




function timeParseExact(time) {
    if (time != undefined) {
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
    } else {
        return parseFloat("00.00");
    }


}

function validateEmployee(d) {
    var _msg = "Must select"
    var isvalid = false;
    if (d.firstname.trim() == '') {
        _msg += ' first name';
        isvalid = true;
    }
    else if (d.lastname.trim() == '') {
        _msg += ' last name';
        isvalid = true;
    }

    else if (d.phone1.trim() == '' || d.phone11.trim() == '') {
        _msg += ' phone 1';
        isvalid = true;
    }

    else if (d.ManufactureChoice.trim() == '') {
        _msg += ' Manufacture';
        isvalid = true;
    }
    //else if (d.phoneTypeChoice.trim() == '') {
    //    _msg += ' phone type';
    //     isvalid = true;
    //}
    if (isvalid)
        return _msg;
    return '';

}