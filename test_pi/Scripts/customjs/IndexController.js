'use strict';
(function () {
    app.controller('Index', function ($scope, $http) {
        $scope.show_button = false;
        $scope.UsersaveUrl = 'Home/SaveBookmark';
        $scope.LoginUser = 'LoginUser';
        $scope.model = {
            id: null,
            title: null,
            url: null,
            category_id: null,
            category_name: null
        };
        $scope.getCategoryUrl = 'Home/GetCategory';
        $scope.CategoryList = [];
        $scope.getCategory = function () {
            try {
                $http.post($scope.getCategoryUrl)
                    .then(function successCallback(response) {
                        $scope.CategoryList = angular.fromJson(response.data);
                        $scope.getBookmark();
                    });
            } catch (e) {
                alert(e);
            }
        };
        $scope.getCategory();

        $scope.getBookmarkUrl = 'Home/GetBookmark';
        $scope.BookmarkList = [];
        $scope.getBookmark = function () {
            try {
                $http.post($scope.getBookmarkUrl)
                    .then(function successCallback(response) {
                        $scope.BookmarkList = angular.fromJson(response.data);
                        if ($scope.CategoryList.length > 0) {
                            for (var i = 0; i < $scope.CategoryList.length; i++) {
                                for (var j = 0; j < $scope.BookmarkList.length; j++) {
                                    if ($scope.CategoryList[i].id == $scope.BookmarkList[j].category_id) {
                                        if ($scope.CategoryList[i].details == null) {
                                            $scope.CategoryList[i].details = [];
                                        }
                                        $scope.CategoryList[i].details.push($scope.BookmarkList[j]);
                                    }
                                }
                            }
                        }
                    });
            } catch (e) {
                alert(e);
            }
        };


        $scope.click = function () {
            $scope.model.category_id = null;
            $scope.show_button = true;
        }


        $scope.SaveBookmark = function () {
            try {

                if ($scope.model.title == null) {
                    alert("Insert Title..!");
                }
                else if ($scope.model.url == null) {
                    alert("Insert URL..!");
                }
                else if ($scope.model.category_id == null && $scope.model.category_name == null) {
                    alert("Select or Insert Category..!");
                }
                else {
                    $http({
                        method: 'POST',
                        url: $scope.UsersaveUrl,
                        data: { 'data': $scope.model },
                        dataType: 'JSON'
                    }).then(function successCallback(response) {
                        if (response.data.Error === true) {
                            alert(response.data.Message);
                        }
                        else {
                            $('#exampleModal').modal('hide');
                            $scope.getCategory();
                            $scope.clear();
                            alert(response.data.Message);
                        }
                    }), function errorCallBack(response) {
                        ShowResult(response.data.Message, 'failure');
                    }
                }
            } catch (e) {
                alert(e);
            }
        };
        $scope.modalData = { title: null, url: null, category_name: null };
        $scope.setData = function (title, url, category_name) {
            $scope.modalData = { title: title, url: url, category_name: category_name };
        }
        $scope.clear = function () {
            $scope.model = {
                id: null,
                title: null,
                url: null,
                category_id: null,
                category_name: null
            };
            $scope.modalData = { title: null, url: null, category_name: null };
            $scope.show_button = false;
        }


    });
}).call(angular);