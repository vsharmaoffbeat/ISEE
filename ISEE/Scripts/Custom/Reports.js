var reportModule = angular.module('Reports', [])

reportModule.controller('ReportController', function ($scope, ReportService) {

    $scope.SelectedReport = null;

    $scope.Reports = [
        { text: "List Of Customer", ReportName: "ListCustomers" },
        { text: "Customer Request", ReportName: "CustomerRequests" },
        { text: "Employee Sms", ReportName: "EmployeeSms" }
    ];

    $scope.SelectReport = function (objReport) {
        if (objReport == null || objReport == undefined) {
            $scope.SelectedReport = null;
        } else {
            $scope.SelectedReport = objReport.ReportName;
        }
    }


}).service('ReportService', function ($http) {
    //to create unique contact id
    var contacts = {};

    contacts.GetCustomersNew = function (objData) {
        return $http({
            url: '/Employee/GetCustomersNew',
            data: objData,
            method: 'POST',
            headers: { 'content-type': 'application/json' }
        });
    }

    return contacts;
});