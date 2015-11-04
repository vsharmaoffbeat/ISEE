angular
    .module('app', ['ngMessages'])
    .controller('MainCtrl', MainCtrl);

function MainCtrl() { }

var app = angular.module('nums', ['ngResource']);

app.controller('employeeCtl', function ($scope, $http) {
    $scope.digits = {};
});

angular.module('app', [])

var app = angular.module('employeeCtl', []);
app.controller('MainCtrl', function ($scope) {
    $scope.employee = 'this is wrong';
});
 
app.directive('regexValidate', function () {
    return {
        restrict: 'A',// restrict to an attribute type.
        require: 'ngModel',// element must have ng-model attribute.
        link: function (scope, elem, attr, ctrl) {
            // here
            //scope = parent scope
            // elem = element
            // attr = attributes on the element
            // ctrl = controller for ngModel.
            var flags = attr.regexValidateFlags || '';
            //get the regex flags from the regex-validate-flags="" attribute (optional) 
            var regex = new RegExp(attr.regexValidate, flags);// create the regex obj.
            ctrl.$parsers.unshift(function (value) { // add a parser that will process each time the value is
                // parsed into the model when the user updates it.
                var valid = regex.test(value); // test and set the validity after update. 
                ctrl.$setValidity('regexValidate', valid);
                return valid ? value : '#ff0000';// if it's valid, return the value to the model,
                // otherwise return undefined. 
            });
 
            ctrl.$formatters.unshift(function (value) {
                // add a formatter that will process each time the value
                // is updated on the DOM element.
                ctrl.$setValidity('regexValidate', regex.test(value));
                return value; // return the value or nothing will be written to the DOM.
            });}
    };
}); 

function employeeCtl($scope, $window) {


}