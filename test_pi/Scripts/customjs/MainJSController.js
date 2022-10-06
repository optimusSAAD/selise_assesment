'use strict';
(function () {
    app.controller('Index', function ($scope, $http) {
        $scope.UsersaveUrl = 'createUser';
        $scope.LoginUser = 'LoginUser';
        $scope.UserModel = {
            UserId: null,
            FName: null,
            LName: null,
            Password: null,
            Phone: null,
            Email: null,
            CityId: null,
            Image: null,
            CV: null,
            DOB: null,
            Gender: null,
            Country: null
        };
        $scope.getCountryUrl ='GetCountry'
        $scope.CountryList = [];
        $scope.getCountry = function () {
            try {
                $http.post($scope.getCountryUrl)
                    .then(function successCallback(response) {
                        $scope.CountryList = angular.fromJson(response.data);
                    });
            } catch (e) {
                alert(e);
            }
        };
        $scope.City = [];
        $scope.getCity = function (Id) {
            try {
                $http.post('City?Id=' + Id)
                    .then(function successCallback(response) {
                        $scope.City = angular.fromJson(response.data);
                    });
            } catch (e) {
                alert(e);
            }
        };
        $scope.getCountry();

        $scope.SaveUser = function () {
            try {

                $http({
                    method: 'POST',
                    url: $scope.UsersaveUrl,
                    data: { 'data': $scope.UserModel },
                    dataType: 'JSON'
                }).then(function successCallback(response) {
                    if (response.data.Error === true) {
                        alert(response.data.Message);
                    }
                    else {
                        $scope.ImageFileSave(response.data.Data.UserId);
                    }
                }), function errorCallBack(response) {
                    ShowResult(response.data.Message, 'failure');
                }
            } catch (e) {
                alert(e);
            }
        };

        $scope.ImageFileSave = function (Id) {
            try {
                var picData = new FormData();
                var fileInput = document.getElementById('uploadImage');
                var file = fileInput.files[0];

                $http({
                    method: 'POST',
                    url: 'ImageFile',
                    headers: { 'Content-Type': undefined },
                    transformRequest: function (data) {
                        picData.append("UserInfo", angular.toJson(data.data));
                        picData.append('file', data.file);
                        return picData;
                    },
                    data: { 'data': $scope.UserModel, 'file': $scope.picData},

                }).then(function successCallback(response) {
                    if (response.data.Error === true) {
                        alert(response.data.Message);
                        $scope.ImageFileSave();
                    }
                    else {
                        alert(response.data.Message);                        

                    }
                }), function errorCallBack(response) {
                    ShowResult(response.data.Message, 'failure');
                }
            } catch (e) {
                alert(e);
            }
        };

        $scope.picData = null;
        $("#uploadImage").change(function () {
            $scope.picData = this.files[0];
        });
        $scope.Login = function (email, pass) {
            try {
                
                $http({
                    method: 'POST',
                    url: $scope.LoginUser,
                    data: {
                        'User': $scope.UserModel
                    },
                    dataType: 'JSON'

                }).then(function successCallback(response) {
                    console.log(response);
                    if (response.data.Error === true) {
                        alert(response.data.Message);
                    }
                    else {
                        alert(response.data.Message);

                    }
                }), function errorCallBack(response) {
                }
            } catch (e) {
                alert(e);
            }


        }

    });
}).call(angular);