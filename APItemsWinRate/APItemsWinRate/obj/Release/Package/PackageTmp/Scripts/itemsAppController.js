var app = angular.module("itemsApp");

app.controller("itemsAppController", function ($scope, $timeout, itemsData, $filter, $sce) {
    
    $scope.items;

    prepareItemsForChart();

    function prepareItemsForChart() {
        $scope.items = itemsData.Items.map(function (item) {
            // We create an array of arrays, because that's what the chart is expecting
            // I think chart.js expects an object actually. whatever.
            var modified = item;
            modified.ItemDataByRankPrePatchData = new Array(item.ItemDataByRankPrePatchData);
            modified.ItemDataByRankPostPatchData = new Array(item.ItemDataByRankPostPatchData);
            modified.ItemDataByRegionPrePatchData = new Array(item.ItemDataByRegionPrePatchData);
            modified.ItemDataByRegionPostPatchData = new Array(item.ItemDataByRegionPostPatchData);

            return modified;
        });
    };

    $scope.options = {
        readOnly: true,
        min: 0,
        max: 100,
        inputColor: "#222222",
        width: 140,
        thickness: .35,
        format: function(value){
            return value + "%";
        }
    };

    $scope.data = [[10, 20, 30, 40]];
    $scope.labels = ["A", "B", "C", "D"];
    
    $scope.itemSelected = false;
    $scope.dataPreChange = true;
    $scope.dataPostChange = false;
    $scope.dataVersion = "Check out item after rework";
    $scope.currentItemChampionBgUrlPrePatch;
    $scope.currentItemChampionBgUrlPostPatch;
    $scope.currentItemChampionName;
    $scope.championSelected = false;

    $scope.selectedChampion = {};
    $scope.currentItem = {};

    $scope.selectChampion = function (champ) {
        $scope.selectedChampion = champ;
        $scope.championSelected = true;
    }

    $scope.toggleData = function () {
        $scope.dataPreChange = !$scope.dataPreChange;
        $scope.dataPostChange = !$scope.dataPostChange;

        if($scope.dataPreChange == true)
        {
            $scope.dataVersion = "Check out item after rework";
        } else {
            $scope.dataVersion = "Check out item before rework";
        }

        $scope.championSelected = false;
        $scope.selectedChampion = {};
    };

    $scope.selectItem = function (itemId) {
        $scope.itemSelected = false;
        $scope.championSelected = false;
        $scope.selectedChampion = {};

        var foundItem = $filter('filter')($scope.items, { ItemId: itemId }, true);
        var item = foundItem[0];
        //item.PrePatch = parseInt(item.PrePatch);
        $scope.currentItem = item;
        var champName = item.MostUsedChampionsPrePatchLabels[0] == "Fiddlesticks" ? "FiddleSticks" : item.MostUsedChampionsPrePatchLabels[0];
        var champName = champName == "LeBlanc" ? "Leblanc" : champName;
        $scope.currentItemChampionBgUrlPrePatch = "http://ddragon.leagueoflegends.com/cdn/img/champion/loading/" + champName + "_0.jpg";


        var champNamePost = item.MostUsedChampionsPostPatchLabels[0] == "Fiddlesticks" ? "FiddleSticks" : item.MostUsedChampionsPostPatchLabels[0];
        var champNamePost = champNamePost == "LeBlanc" ? "Leblanc" : champNamePost;
        $scope.currentItemChampionBgUrlPostPatch = "http://ddragon.leagueoflegends.com/cdn/img/champion/loading/" + champNamePost + "_0.jpg";

        $timeout(function () {
            $scope.itemSelected = true;
        }, 500);
    };

    $scope.chartOptions = {
        tooltipTemplate: "<%if (label){%><%=label%>: <%}%><%= value %>%"
    };

    $scope.barChartOptions = {
        "colours": [{ // default
            "fillColor": "rgba(255,70,74,0.6)",
            "strokeColor": "rgba(215,70,74,0.9)",
            "pointColor": "rgba(210,70,74,1)",
            "pointStrokeColor": "#fff",
            "pointHighlightFill": "#fff",
            "pointHighlightStroke": "rgba(151,187,205,0.8)"
        }],
        "otherColours": [{ // default
            "fillColor": "rgba(241,89,40,0.6)",
            "strokeColor": "rgba(210,89,40,0.9)",
            "pointColor": "rgba(210,70,74,1)",
            "pointStrokeColor": "#fff",
            "pointHighlightFill": "#fff",
            "pointHighlightStroke": "rgba(151,187,205,0.8)"
        }],
        "otherOptions": {
            scaleOverride: true,
            scaleLabel: "<%=value%>%",
            scaleSteps: 10,
            scaleStepWidth: 10,
            scaleStartValue: 0,
            barValueSpacing: 10,
            scaleShowGridLines: true,
            scaleFontSize: 10,
            tooltipTemplate: "<%if (label){%><%=label%>: <%}%><%= value %>%"
        },
        "options": {
            // Boolean - If we want to override with a hard coded scale
            scaleOverride: true,
            scaleLabel: "<%=value%>%",
            // ** Required if scaleOverride is true **
            // Number - The number of steps in a hard coded scale
            scaleSteps: 10,
            // Number - The value jump in the hard coded scale
            scaleStepWidth: 10,
            // Number - The scale starting value
            scaleStartValue: 0,
            barValueSpacing: 12,
            scaleShowGridLines: true,
            scaleFontSize: 10,
            tooltipTemplate: "<%if (label){%><%=label%>: <%}%><%= value %>%"
        }
    };
});