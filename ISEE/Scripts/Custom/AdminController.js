﻿var module = angular.module('TreeDetails', [])

//appAdmin.controller("ss", function ($scope, EmployeeService) {




//});


module.controller('SearchCtrl', function ($scope, ContactService) {
    //  $scope.contacts = ContactService.list();
    var SysIdLevel1 = {};
    var SysIdLevel1max = {};
    var OverallSecondarys = [];

    //ContactService.getMaxValue().then(function (d) {
    //    var array = $.makeArray(d.data);
    //    Math.max.apply(Math, array.map(function (maxvalue) {
    //        return SysIdLevel1max = maxvalue.RequestSysIdLevel1 + 1;
    //    }))
    //}, function (error) {
    //    alert('Error !');
    //});
    ContactService.getList().then(function (d) {
        debugger;
        $scope.contacts = $.makeArray(d.data);
        SysIdLevel1max = d.data[d.data.length - 1].RequestSysIdLevel1 + 1;
    }, function (error) {
        alert('Error !');
    });
    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }
    $scope.BindSecondary = function (contact) {
        $scope.SelectedSysIdLevel1 = contact.RequestSysIdLevel1;
        SysIdLevel1 = contact.RequestSysIdLevel1;

        var isVAlExisting = false;

        $.each(OverallSecondarys, function (index, value) {
            debugger;
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
    $scope.saveSecondary = function () {
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
    // code  for Secondary cases
    $scope.OverallSave = function () {
        ContactService.OverallSave($scope.contacts, OverallSecondarys).then(function (d) {
            debugger;
            $scope.msg = "Save SuccessFully";
            //  $window.alert("Save SuccessFully");
        })
    }
})



module.service('ContactService', function ($http) {
    //to create unique contact id
    var contacts = {};
    contacts.getList = function () {
        debugger;
        return $http({
            url: '/Admin/getAll',
            method: 'GET',
        });
    }
    contacts.getMaxValue = function () {
        debugger;
        return $http({
            url: '/Admin/getMaxValue',
            method: 'GET',
        });
    }
    contacts.getSecondarylist = function (SysIdLevel1) {
        // var SysIdLevel1Val = { SysIdLevel1: SysIdLevel1 };
        debugger;
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
        debugger;
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
    return contacts;
});
