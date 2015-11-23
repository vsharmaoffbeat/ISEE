var schedulerModule = angular.module('Scheduler', [])

schedulerModule.controller('SchedulerController', function ($scope, SchedulerService) {

}).service('SchedulerService', function ($http) {
    //to create unique contact id
    var contacts = {};

    contacts.GetCustomersNew = function (objData) {

        return $http({
            url: '/Admin/GetCustomersNew',
            data: objData,
            method: 'POST',
            headers: { 'content-type': 'application/json' }
        });
    }

});