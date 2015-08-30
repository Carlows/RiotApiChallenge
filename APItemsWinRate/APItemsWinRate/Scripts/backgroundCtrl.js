var app = angular.module("itemsApp");

app.controller("backgroundCtrl", function ($scope, $sce) {
    var championVideoNames = ["kalista_dmzolq", "draven_petw6j", "yasuo_nvklba", "jayce_l4hf21", "vi_udduuv", "zed_kehyin", "hecarim_ibyazg", "thresh_wnffkj"];
    $scope.championUrl = championVideoNames[Math.floor(Math.random() * championVideoNames.length)] + ".mp4";

    $scope.selectRandomChampion = function(){
        $scope.championUrl = championVideoNames[Math.floor(Math.random() * championVideoNames.length)] + ".mp4";
    }

    $scope.getUrlVideo = function (videoId) {
        return $sce.trustAsResourceUrl('http://res.cloudinary.com/carlosemecloud/video/upload/' + videoId);
    };
});