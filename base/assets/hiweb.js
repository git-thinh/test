var Hiweb = (function () {
    function HiwebObject(options) {
        options = options || {};

        var _APP, _HTML = {}, _COM = {},
            _VIEWS = [], _VIEW_CF = {},
            _DATA = {
                view: {
                    loading: null,
                    login: null,
                    confirm_pass: null,
                    screen_lock: null,

                    sidebar_left: null,
                    sidebar_right: null,

                    header_top: null,
                    header_breadcrumb: null,
                    header_tab: null,
                    header_filter: null,
                    main_left: null,
                    main_body: null,
                    footer: null,

                    dialog_1: null,
                    dialog_2: null,
                    dialog_3: null,

                    popup_1: null,
                    popup_2: null,
                    popup_3: null,

                    alert_1: null,
                    alert_2: null,
                    alert_3: null
                }
            };

        function _guidID() {
            return 'id-xxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        };

        function _guidName() {
            return 'name_xxxxx_xxxx_4xxx_yxxx_xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        };

        function _getUrl(url, typeResponse) {
            typeResponse = typeResponse || 'text';

            var xhr = new XMLHttpRequest();
            xhr.open('GET', url, false);
            xhr.send(null);

            var result = '';
            if (typeResponse != 'text') result = null;

            if (xhr.status === 200) {
                if (typeResponse == 'text') result = xhr.responseText;
                else result = JSON.parse(xhr.responseText);
            }

            return result;
        }

        ///////////////////////////////////////////////////////////

        function _mainLoading(visiable) {
            if (visiable == false)
                _APP.view.loading = null;
            else {
                if (_VIEW_CF.loading) {
                    var viewLoading = _VIEW_CF.loading.split('|')[0];
                    var cfLoading = _.find(_VIEWS, function (o) { return o.key == viewLoading; });
                    //console.log(cfLoading);
                    if (cfLoading) {
                        _viewInit(cfLoading, () => {
                            var vueLoading = window[_KITNAME]._getComponent(viewLoading);
                            //console.log(vueLoading);
                            _APP.view.loading = vueLoading;
                        });
                    }
                }
            }
        }

        function _showLogin() {
            if (_VIEW_CF.login) {
                var viewLogin = _VIEW_CF.login.split('|')[0];
                var cfLogin = _.find(_VIEWS, function (o) { return o.key == viewLogin; });
                //console.log(cfLogin);
                if (cfLogin) {
                    _viewInit(cfLogin, () => {
                        var vueLogin = window[_KITNAME]._getComponent(viewLogin);
                        //console.log(vueLogin);
                        _APP.view.main_body = vueLogin;
                        _APP.view.loading = null;
                    });
                }
            }
        }

        function _showDashboard() {
            for (var area in _VIEW_CF) {
                if (area.startsWith('sidebar')
                    || area.startsWith('header')
                    || area.startsWith('main')
                    || area.startsWith('footer')) {
                    var viewName = _VIEW_CF[area];
                    if (viewName && viewName.length > 0) {
                        viewName = viewName.split('|')[0];
                        var cf = _.find(_VIEWS, function (o) { return o.key == viewName; });
                        console.log(area, viewName, cf);
                        if (cf) {
                            _viewInit(cf, () => {
                                var v = window[_KITNAME]._getComponent(viewName);
                                //_APP.view.main_body = v;
                                console.log('???????????? ', v)
                            });
                        }
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////

        function _appInit(cb) {
            _VIEW_CF = _getUrl('app/config.json', 'json');
            _VIEWS = _getUrl('app/list.json', 'json');
            if (_VIEWS == null) _VIEWS = [];
            if (_VIEW_CF == null) _VIEW_CF = {};
            if (_VIEW_CF && _VIEWS) {
                _vueInit(cb);
            } else {
                console.error('Cannot found the file: app/config.json or app/list.json');
            }
        }

        function _vueInit(cb) {
            _APP = new Vue({
                el: '#app',
                data: function () { return _DATA; },
                mounted: function () {
                    Vue.nextTick(function () {
                        if (cb) cb();
                    });
                }
            });
        }

        function _viewInit(cf, cb) {
            if (cf && cf.key) {
                var key = cf.key;

                if (_HTML.hasOwnProperty(key)) {
                    if (cb) cb();
                    return;
                }

                if (cf.files && Array.isArray(cf.files)) {
                    var fileHtml = _.find(cf.files, function (o) { return o == 'temp.htm' });
                    var fileJs = _.find(cf.files, function (o) { return o == 'controller.js' });
                    var fileCss = _.find(cf.files, function (o) { return o == 'style.css' });

                    if (fileCss) {
                        var urlCss = 'app/' + cf.scope + '/' + cf.name + '/style.css';
                        var el = document.createElement('link');
                        el.setAttribute('rel', 'stylesheet');
                        el.setAttribute('href', urlCss);
                        //el.setAttribute('id', cf.key + '___css');
                        document.getElementsByTagName('head')[0].appendChild(el);
                    }

                    if (fileHtml) {
                        var htm = _getUrl('app/' + cf.scope + '/' + cf.name + '/temp.htm');
                        _HTML[key] = htm;
                    } else {
                        _HTML[key] = '<div></div>';
                    }

                    var textJs = '';
                    if (fileJs) textJs = _getUrl('app/' + cf.scope + '/' + cf.name + '/controller.js');


                    if (textJs.length == 0)
                        textJs = '{ data: function () { return {}; }, mounted: function () {}, methods: {} }';
                    textJs = textJs.trim().substr(1);

                    var strTemplate = _KITNAME + '._getViewHtml("' + key + '")';
                    //if (___HTML[key + '.' + DEVICE_NAME] != null) strTemplate = '___HTML["' + key + '.' + DEVICE_NAME + '"]';

                    var dynName = _guidName() + '_' + (new Date()).getTime();
                    textJs =
                        //'var ' + dynName + ' = { mixins: [___MIXIN], template: ' + strTemplate + ', \r\n ' + text + ' \r\n ' +
                        'var ' + dynName + ' = { \r\n template: ' + strTemplate + ', \r\n ' + textJs + ' \r\n\r\n ' +
                        _KITNAME + '._setComponent("' + key + '", ' + dynName + '); \r\n\r\n ' +
                        'Vue.component("' + key + '", ' + dynName + '); \r\n ';
                    var urlJs = URL.createObjectURL(new Blob([textJs], { type: 'text/javascript' }));
                    console.log('VIEW_INIT: ' + key + ' = ' + urlJs);
                    head.load([{ vueComJs: urlJs }], cb);
                }
            }
        }

        return {
            _setComponent: function (keyView, objVueCom) { _COM[keyView] = objVueCom; },
            _getComponent: function (keyView) { return _COM[keyView]; },
            _getViewHtml: function (keyView) { return _HTML[keyView]; },
            getApp: function () { return _APP; },
            init: function (libs) {
                libs = libs || [];
                if (libs.length > 0) {
                    var items = [];
                    for (var i = 0; i < libs.length; i++) {
                        var item = {};
                        item['item_' + i] = 'assets/libs/' + libs[i] + '?v=' + new Date().getTime();
                        items.push(item);
                    }
                    head.load(items, () => {
                        _appInit(() => {
                            //_mainLoading();
                            //_showLogin();
                            _showDashboard();
                        });
                    });
                }
            },
            mainLoading: function (visiable) {
                _mainLoading(visiable);
            },
            showLogin: _showLogin,
            login: function () {
                _showDashboard();
            }
        };
    }

    var instance;
    return {
        getInstance: function (options) {
            if (instance === undefined) instance = new HiwebObject(options);
            return instance;
        }
    };
})();
var _KITNAME = '_HW';
var _HW = Hiweb.getInstance();
_HW.init([
    'jquery-3.5.1.slim.min.js',
    'popper.min.js',
    'lodash.min.js',
    'classie.js',
    'vue.min.js',
    'bootstrap.min.js',
    'bootstrap.min.css',
    'reset.css'
]);