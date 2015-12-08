var reportModule = angular.module('Reports', [])

reportModule.controller('ReportController', function ($scope, ReportService) {

    $scope.SelectedReport = null;
    $scope.HideSearchButton = true;
    $scope.HideParamSearchButton = true;

    $scope.Reports = [
        { text: "List Of Customer", ReportName: "ListCustomers" },
        { text: "Customer Request", ReportName: "CustomerRequests" },
        { text: "Employee Sms", ReportName: "EmployeeSms" }
    ];

    $scope.Customers = null;
    $scope.Employees = null;

    $scope.ReportFilterCriteria = {
        FilterType: '1',
        FirstName: '',
        LastName: '',
        CustomerNumber: '',
        EmployeeNumber: '',
        Active: true
    }

    //$scope.EmployeeSMSReportFilter = {
    //    startDate: '',
    //    EndDate: '',
    //    FilterType: '1',
    //    FirstName: '',
    //    LastName: '',
    //    CustomerNumber: '',
    //    Active: true
    //}


    $scope.SelectReport = function (objReport) {
        if (objReport == null || objReport == undefined) {
            $scope.ReportSearchParams.ReportName = 'Empty'
            $scope.SelectedReport = null;
            $scope.HideSearchButton = true;
        } else {
            $scope.ReportSearchParams.ReportName = objReport.ReportName
            $scope.SelectedReport = objReport.ReportName;
            $scope.HideSearchButton = false;
        }
    }

    $scope.ShowFilterDialog = function () {
        if ($scope.ReportSearchParams.ReportName == "ListCustomers") {
            $('#divReportFilter').dialog({
                width: 1000,
                height: 600,
                open: function (event, ui) {
                    $scope.ReportFilterCriteria.FilterType = '1'
                }
            })
        }
        else if ($scope.ReportSearchParams.ReportName == "EmployeeSms") {
            $('#divEmployeeSMSReportFilter').dialog({
                width: 1000,
                height: 600,
                open: function (event, ui) {
                    $scope.ReportFilterCriteria.FilterType = '1'
                }
            })
        }
    }
    $scope.CloseDialogEmployeeSMS = function () {
        $('#divEmployeeSMSReportFilter').dialog('close');
    }

    $scope.CloseDialog = function () {
        $('#divReportFilter').dialog('close');
    }

    //Show/Hide search button in Parameter Dialog Box
    $scope.$watch('ReportFilterCriteria.FilterType', function () {
        if ($scope.ReportFilterCriteria.FilterType == '1') {
            $scope.HideParamSearchButton = true;
        } else {
            $scope.HideParamSearchButton = false;
        }

    }, true);

    $scope.DoSearch = function () {
        ReportService.GetCustomers($scope.ReportFilterCriteria).then(function (d) {
            if (d.data.IsSuccess == true) {
                $scope.Customers = d.data.Customers;
            }
            else {
                $scope.Customers = null;
                $scope.ShowMessageBox(d.data.ErrorMessageBoxTitle, d.data.ErrorMessageText);
            }
        }, function (error) {
            $scope.ShowMessageBox('Message', 'An Error has been occured....');
        });
    }


    $scope.DoSearchOnEmployeeSMS = function () {
        ReportService.GetEmployees($scope.ReportFilterCriteria).then(function (d) {
            if (d.data.IsSuccess == true) {
                $scope.Employees = d.data.Employees;
            }
            else {
                $scope.Employees = null;
                $scope.ShowMessageBox(d.data.ErrorMessageBoxTitle, d.data.ErrorMessageText);
            }
        }, function (error) {
            $scope.ShowMessageBox('Message', 'An Error has been occured....');
        });
    }


    $scope.toggleAll = function () {
        var toggleStatus = $scope.isAllSelected;
        if ($scope.ReportSearchParams.ReportName == "ListCustomers") {
            angular.forEach($scope.Customers, function (itm) {
                itm.IsSelected = toggleStatus;
            });
        } else if ($scope.ReportSearchParams.ReportName == "EmployeeSms") {
            angular.forEach($scope.Employees, function (itm) {
                itm.IsSelected = toggleStatus;
            });
        }
    }

    $scope.optionToggled = function () {
        if ($scope.ReportSearchParams.ReportName == "ListCustomers") {
            $scope.isAllSelected = $scope.Customers.every(function (itm) { return itm.IsSelected; })
        } else if ($scope.ReportSearchParams.ReportName == "EmployeeSms") {
            $scope.isAllSelected = $scope.Employees.every(function (itm) { return itm.IsSelected; })
        }
    }

    $scope.ReportSearchParams = {
        StartDate: '',
        EndDate: '',
        ReportName: 'Empty',
        FilterSearch: ''
    }



})
.service('ReportService', function ($http) {
    //to create unique contact id
    var contacts = {};

    contacts.GetCustomers = function (objData) {
        return $http({
            url: '/Reports/GetCustomers',
            data: objData,
            method: 'POST',
            headers: { 'content-type': 'application/json' }
        });
    }

    contacts.GetEmployees = function (objData) {
        return $http({
            url: '/Reports/GetEmployees',
            data: objData,
            method: 'POST',
            headers: { 'content-type': 'application/json' }
        });
    }

    return contacts;
})

.directive('ngUpdateHidden', function () {
    return function (scope, el, attr) {
        var model = attr['ngModel'];
        scope.$watch(model, function (nv) {
            el.val(nv);
        });

    };
})


wuSubmit.$inject = ['$timeout'];
function wuSubmit() {
    return {
        require: 'form',
        restrict: 'A',
        link: function (scope, element, attributes) {
            var scope = element.scope();
            if (attributes.name && scope[attributes.name]) {
                scope[attributes.name].$submit = function () {
                    // Parse the handler of submit & execute that.
                    var fn = $parse(attr.ngSubmit);
                    $scope.$apply(function () {
                        fn($scope, { $event: e });
                    });
                };
            }
        }
    };
}

var submitForm = function () {
    var appElement = document.querySelector('[ng-controller=ReportController]');
    var $scope = angular.element(appElement).scope();
    $scope.$apply(function () {
        if ($scope.ReportFilterCriteria.FilterType == '1') {
            $scope.ReportSearchParams.FilterSearch = '';
        } else {
            angular.forEach($scope.Customers, function (itm) {
                if (itm.IsSelected) {
                    $scope.ReportSearchParams.FilterSearch += itm.Id + ","
                }
            });
        }
    });
    $('#reportForm').submit();
}

var submitFormEmployeeSMS = function () {
    var appElement = document.querySelector('[ng-controller=ReportController]');
    var $scope = angular.element(appElement).scope();
    $scope.$apply(function () {
        if ($scope.ReportFilterCriteria.FilterType == '1') {
            $scope.ReportSearchParams.FilterSearch = '';
        } else {
            angular.forEach($scope.Employees, function (itm) {
                if (itm.IsSelected) {
                    $scope.ReportSearchParams.FilterSearch += itm.Id + ","
                }
            });
        }
    });
    $('#reportForm').submit();
}