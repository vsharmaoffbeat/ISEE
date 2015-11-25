var schedulerModule = angular.module('Scheduler', [])
var selectedEventID;
schedulerModule.controller('SchedulerController', function ($scope, SchedulerService) {

    $scope.SelectedEventDate = ''

    $scope.CustomerSearchData = {
        State: '',
        City: '',
        Street: '',
        BuildingNumber: '',
        ContactName: '',
        CustomerNumber: '',
    }
    $scope.Customers = null;


    //Get Customer By Search Criteria
    $scope.GetCustomersNew = function () {
        if ($scope.CustomerSearchData.State == '') {
            $scope.CustomerSearchData.State = '0'
        }
        if ($scope.CustomerSearchData.City == '') {
            $scope.CustomerSearchData.City = '0'
        }
        if ($scope.CustomerSearchData.Street == '') {
            $scope.CustomerSearchData.Street = '0'
        }
        $scope.Customers = null;
        SchedulerService.GetCustomersNew($scope.CustomerSearchData).then(function (d) {
            if (d.data.Customers.length > 0) {
                $scope.Customers = d.data.Customers;
            }
            else {
                $scope.ShowMessageBox('Message', 'No Records Found');
            }

            //Reset Search
            if ($scope.CustomerSearchData.State == '0') {
                $scope.CustomerSearchData.State = ''
            }
            if ($scope.CustomerSearchData.City == '0') {
                $scope.CustomerSearchData.City = ''
            }
            if ($scope.CustomerSearchData.Street == '0') {
                $scope.CustomerSearchData.Street = ''
            }
        }, function (error) {
            $scope.ShowMessageBox('Message', 'An Error has been occured....');
        });
    }

    //Update Selected Event On Clander
    $scope.updatecalendarEvent = function (objCustomer) {
        if (selectedEventID > 0) {
            SchedulerService.GetDistance($scope.CustomerSearchData).then(function (d) {
                if (d.data.IsSuccess == true) {
                    UpdateEvent(selectedEventID, d.data.color, objCustomer)
                }
                else {
                    $scope.ShowMessageBox('Message', 'No Records Found');
                }
            }, function (error) {
                $scope.ShowMessageBox('Message', 'An Error has been occured....');
            });

        }
    };

    //Save Calendar Data
    $scope.SaveCalendarEventsToDatabase = function () {
        var eventsData = scheduler.getEvents();

        SchedulerService.SaveEvents(eventsData).then(function (d) {
            if (d.data.IsSuccess == true) {
            }
            else {
                $scope.ShowMessageBox('Message', 'An Error has been occured....');
            }
        }, function (error) {
            $scope.ShowMessageBox('Message', 'An Error has been occured....');
        });
    }

}).service('SchedulerService', function ($http) {
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

    contacts.SaveEvents = function (objData) {
        return $http({
            url: '/Employee/SaveEvents',
            data: objData,
            method: 'POST',
            headers: { 'content-type': 'application/json' }
        });
    }

    contacts.GetDistance = function (objCustomer) {
        return $http({
            url: '/Employee/GetDistance',
            data: objCustomer,
            method: 'POST',
            headers: { 'content-type': 'application/json' }
        });
    }


    return contacts;
});