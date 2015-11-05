angular.module("Employee", [])
 .controller("employeeCtl", function ($scope, employeeService) {
     $scope.IsFormValid = false;
     $scope.Submitted = false;
     var person = {};
     $scope.employee = {
         Number:'',
         FirstName:'',
         StartDay:'',
         Mail:'',
         LastName:'',
         EndDate:'',
         Phone1:'',
         Phone:'',
         Phone2:'',

         //phone:$scope.Phone
     };

     $scope.SaveEmployee = function (emp) {
         
         $scope.Submitted = true;
         //if ($scope.IsFormValid) {
         employeeService.SaveEmployee($scope).then(function (d) {
             
             if (d.data > 0) {
                 //$scope.IsLoggedIn = true;
                 $scope.Message = "  Success";
                 window.location.href = '/Admin/Employee';
             } else {
                 $scope.Message = "  fail."
             }
         });
         employeeService.clearEmployeeFields($scope.employee).then(function (d) {

         });
         //}
     };
 }).factory("employeeService", function ($http) {
     var fac = {};
     fac.SaveEmployee = function (d) {
         
         alert(d.data);
         return fac;
         //return $http({
         //    url: '/Data/UserLogin',
         //    method: 'GET',
         //    data: JSON.stringify(d),
         //    headers: { 'content-type': 'application/json' }
         //});
     };
     fac.clearEmployeeFields = function (d) {
         alert('Clear');
         $scope.employee = null;
         return fac;
         //return $http({
         //    url: '/Data/UserLogin',
         //    method: 'GET',
         //    data: JSON.stringify(d),
         //    headers: { 'content-type': 'application/json' }
         //});
     };
     return fac;
 });