"use strict";
var system = require('system');

if (system.args.length < 2) {
    console.log('ERROR: URL is null or empty');
    phantom.exit();
}

//----------------------------------------------------------------------------

var _URL = '';
if (system.args.length > 1) _URL = system.args[1];
//console.log(_URL);
var a = _URL.split('/');
var host = a.length > 2 ? a[2] : '';
if (host.indexOf('www.') === 0) host = host.substr(4);
//console.log(host);
if (host.length === 0) {
    console.log('ERROR: HOST of URL is null or empty');
    phantom.exit();
}

//----------------------------------------------------------------------------

var page = require('webpage').create();
page.settings.userAgent = 'Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1';

//page.onResourceRequested = function (requestData, request) {
//    var url_ = requestData['url'];
//    //if (url_ === url || (url_.indexOf('.js') > 0 && url_.indexOf(host) > 0)) {
//    console.log('-> ' + url_);
//    //} else {
//    //    console.log('NO: ' + url_);
//    //    request.abort();
//    //}
//};

page.open(_URL, 'get', null, function (status) {
    if (status !== 'success') {
        console.log('ERROR: ' + status);
    } else {
        console.log(page.content);
        //console.log('done.................');
    }
    phantom.exit();
});
