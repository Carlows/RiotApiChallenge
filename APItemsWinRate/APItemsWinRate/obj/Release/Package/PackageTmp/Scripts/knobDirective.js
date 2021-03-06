﻿var app = angular.module("itemsApp");

app.directive('tooltip', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).tooltip('show');
        }
    };
});

app.directive('knob', ['$timeout', function ($timeout) {
    'use strict';

    return {
        restrict: 'EA',
        replace: true,
        template: '<input value="{{ knobData }}"/>',
        scope: {
            knobData: '=',
            knobOptions: '&'
        },
        link: function ($scope, $element) {
            var knobInit = $scope.knobOptions() || {};

            knobInit.release = function (newValue) {
                $timeout(function () {
                    $scope.knobData = newValue;
                    $scope.$apply();
                });
            };

            $scope.$watch('knobData', function (newValue, oldValue) {
                if (newValue != oldValue) {
                    $($element).val(newValue).change();
                }
            });

            $($element).val($scope.knobData).knob(knobInit);
        }
    };
}]);
