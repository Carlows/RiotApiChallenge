var app = angular.module("itemsApp");

app.factory("itemsFactory", function ($http, $q, serverURL, lolStaticDataUrl) {
    return {
        getItemData: function (itemId){
            var deferred = $q.defer();

            var url = serverURL + "/GetItemData?ItemID=" + itemId;

            $http.get(url).then(function(data){
                deferred.resolve(data);
            }, function(error){
                deferred.reject(error);
            });

            return deferred.promise;
        }
    }
});