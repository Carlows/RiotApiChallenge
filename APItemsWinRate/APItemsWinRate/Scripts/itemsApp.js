var app = angular.module("itemsApp", ['ngAnimate', 'chart.js']);

app.config(function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
});

app.constant("serverURL", "http://localhost:50433/StatsByItem");
app.constant("lolStaticDataUrl", "https://global.api.pvp.net/api/lol/static-data/lan/v1.2/");
