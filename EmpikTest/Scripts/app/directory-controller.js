var app = angular.module('DirectoryApp', ['ngResource']);

app.controller('NavController', function ($scope, $http) {
    $scope.currentPath = "\\";
    $scope.amounts = [0, 0, 0];
    $scope.filelist = [];
    $scope.messages = [];
    $scope.navigate = function (file) {
        $scope.messages.length = 0;
        if (typeof file === 'undefined') {
            file = {
                path : "\\",
                isFile : false
            }
        }
        if (file.isFile == true) return;
        $http.get('api/Directory/GetList', { params: { path: file.path } }).success(function (data) {
                $scope.filelist = data;
                $scope.currentPath = file.path;
        }).error(function (data, status) {
            $scope.amounts = [0, 0, 0];
            $scope.messages.push(data);
        });
        $http.get('api/Directory/GetAmounts', { params: { path: file.path } }).success(function (data) {
            if (file.path == $scope.currentPath)
                $scope.amounts = data;
        }).error(function (data, status) {
            $scope.amounts = [0, 0, 0];
            $scope.messages.push(data);
    });
    }
    $scope.navigate();
})
