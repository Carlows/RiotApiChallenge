var app = angular.module("itemsApp");

app.controller("itemsAppController", function ($scope, $timeout, itemsData, $filter) {
    
    $scope.items = itemsData.Items;

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

    $scope.itemSelected = false;
    $scope.dataPreChange = true;
    $scope.dataPostChange = false;
    $scope.dataVersion = "Before changes";

    $scope.currentItem = {};

    $scope.toggleData = function () {
        $scope.dataPreChange = !$scope.dataPreChange;
        $scope.dataPostChange = !$scope.dataPostChange;

        if($scope.dataPreChange == true)
        {
            $scope.dataVersion = "Before changes";
        } else {
            $scope.dataVersion = "After changes";
        }
    };

    $scope.selectItem = function (itemId) {
        $scope.itemSelected = false;

        var foundItem = $filter('filter')($scope.items, { ItemId: itemId }, true);
        var item = foundItem[0];
        //item.PrePatch = parseInt(item.PrePatch);
        $scope.currentItem = item;  

        $timeout(function () {
            $scope.itemSelected = true;
        }, 500);
    };

    $scope.chartOptions = {
        tooltipTemplate: "<%if (label){%><%=label%>: <%}%><%= value %>%"
    }
});