angular.module("LoginDetails", [])

.controller("LoginCtrl", function ($scope, LoginService) {
    $scope.IsLoggedIn = false;
    $scope.Message = '';
    $scope.IsFormValid = false;
    $scope.Submitted = false;

    //$scope.name = '';
    //$scope.showMsgs = false;
    //$scope.preview = function (Login) {
    //    if ($scope[Login].$valid) {
    //        alert("Previewed");
    //    } else {
    //        $scope.showMsgs = true;
    //    }

    //};

    $scope.LoginFirst = function () {
        debugger
        if ($scope.LoginData.UserName == "" || $scope.LoginData.UserName == undefined || $scope.LoginData.Password == undefined || $scope.LoginData.Password == "") {
            $scope.showValidation = true;

            return false;
        }
        else {
            $scope.showValidation = false;
            $scope.Submitted = true;
            if ($scope.IsFormValid) {
                LoginService.GetUser($scope.LoginData).then(function (d) {
                    if (d.data > 0) {
                        $scope.IsLoggedIn = true;
                        $scope.Message = "Logged in Success" + d.data.UserName;
                        window.location.href = '/Admin/Admin';
                    } else {
                        alert("Please check Your user name and password");
                        $scope.Message = "Logged in fail."
                    }
                });
            }
        }
    }





    $scope.LoginData = {
        UserName: '',
        Password: ''
    };
    $scope.Imagee = {
        url: '~/images/img/cl.png'
    };

    $scope.$watch("login.$valid", function (newVal) {
        $scope.IsFormValid = newVal;
    });

    $scope.CountryCode = null;
    $scope.CountryCodeList = null;
    LoginService.GetCountries().then(function (d) {
        $scope.selectedCountryCode = d.data.SelectedCountry
        $scope.selectedImageUrl = d.data.SelectedCountryImageUrl;
        $scope.CountryCodeList = d.data.CountryList;
    });

    $scope.selectedCountryCode;
    $scope.selectedImageUrl;
    $scope.dropboxitemselected = function (countrycode,imageUrl) {

        $scope.selectedCountryCode = countrycode;
        $scope.selectedImageUrl = imageUrl;
        LoginService.SetSelectedCountry(countrycode).then(function (d) {
            window.location.reload();
        });
    }

    $scope.Login = function () {

    };

}).factory("LoginService", function ($http) {
    var fac = {};
    fac.GetUser = function (d) {
        if (d.UserName !== null && d.Password !== null) {
            return $http({
                url: '/Data/UserLogin',
                method: 'POST',
                data: JSON.stringify(d),
                headers: { 'content-type': 'application/json' }
            });
        } else { alert("fail"); }
    };

    fac.GetCountries = function (d) {

        return $http({
            method: 'Get',
            url: '/Login/GetCountries'
        })
    };
    fac.SetSelectedCountry = function (countryCode) {
        //$('#ru').click();
        //return "";
        return $http({
            method: 'POST',
            url: '/Login/SetSelectedCountry?lang=' + countryCode
        })

    }
    return fac;
});

