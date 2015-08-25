var app = angular.module("itemsApp");

app.controller("backgroundCtrl", function ($scope) {
    var championVideoNames = ["kalista", "draven", "riven", "teemo", "yasuo", "jayce", "vi", "zed", "hecarim", "thresh"];

    $scope.championUrl = "/Content/Videos/" + championVideoNames[Math.floor(Math.random() * championVideoNames.length)] + ".mp4";

    $scope.selectRandomChampion = function(){
        $scope.championUrl = "/Content/Videos/" + championVideoNames[Math.floor(Math.random() * championVideoNames.length)] + ".mp4";
    }   
});