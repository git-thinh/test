var Hiweb = (function () {
    function VueEngine(options) {
        options = options || {};

        var _APP, _HTML = {}, _COM = {}, _COM_ST = {},
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

        var _MIXIN = {
            props: [
                'ref-name',
                'is-dialog',
                'popup-index',
                'obj-user'
            ],
            data: function () {
                return {
                    role___: null,
                    dialog___: {
                        top: '0px',
                        left: '0px'
                    }
                }
            },
            computed: {
                viewKey: function () {
                    var tag = this.$vnode.tag;
                    if (tag) {
                        var a = tag.split('-');
                        tag = a[a.length - 1];
                    }
                    return tag;
                }
            },
            created: function () {
                var _self = this;
            },
            mounted: function () {
                var _self = this;
                _self.___init_com();
                //console.log('MIXIN: mounted -> ' + _self.viewKey);
                //if (_self.viewKey && _self.viewKey.indexOf('___logout') != -1) ___V_LOGOUT = _self;
            },
            methods: {
                ___init_com: function () {
                    var _self = this;
                    var el = _self.$el;

                    if (_self.viewKey)
                        classie.add(el, '___com').add(el, _self.viewKey);

                    var id = 'idvc___' + _self._uid;
                    el.setAttribute('id', id);
                    _self.idvc___ = id;

                    //console.log('MIXIN: ___init_com ' + _self.viewKey + ', role = ', el.parentElement.getAttribute('role'));
                    //console.log('MIXIN: ___init_com ' + _self.viewKey + ', is-dialog = ', _self.isDialog);
                    console.log('MIXIN: ___init_com = ' + _self.viewKey + ', id = ', id, _self.$vnode);

                    if (_self.popupIndex != null) {
                        classie.add(el, '___com').add(el, '___com_popup');
                        el.setAttribute('tabindex', _self.popupIndex);
                    }

                    if (_self.refName != null && _self.$parent) {
                        //console.log(':ref-name = ', _self.refName);
                        _self.$parent.$refs[_self.refName] = _self;
                    }

                    ////var pa = el.parentElement;
                    ////if (pa && pa.hasAttribute('role')) {
                    ////    var role = pa.getAttribute('role');
                    ////    _self.role___ = role;
                    ////} else if (_self.isDialog == true) {
                    ////    _self.role___ = 'dialog';
                    ////    classie.add(el, ___DL_CURRENT_ID);

                    ////    var rec = ___DL_CURRENT_EVENT.target.getBoundingClientRect();
                    ////    console.log('MIXIN: ___init_com ' + ___DL_CURRENT_ID + ', ' + _self.viewKey + ', rec = ', rec);
                    ////    //_self.dialog___.opacity = 0;
                    ////    //_self.dialog___.top = rec.bottom + 'px';

                    ////    var dl = document.querySelector('.' + ___DL_CURRENT_ID);
                    ////    if (dl) {
                    ////        dl.style.opacity = 0;
                    ////        dl.style.top = rec.bottom + 'px';
                    ////    }

                    ////    setTimeout(function () {
                    ////        var dl = document.querySelector('.' + ___DL_CURRENT_ID);
                    ////        if (dl) {
                    ////            var r1 = dl.getBoundingClientRect();
                    ////            //console.log(rec.x + r1.width, window.innerWidth);
                    ////            if (rec.x + r1.width > window.innerWidth) {
                    ////                dl.style.left = 'auto';
                    ////                dl.style.right = '0px';
                    ////            } else {
                    ////                dl.style.right = 'auto';
                    ////                dl.style.left = rec.x + 'px';
                    ////            }
                    ////            dl.style.opacity = 1;
                    ////        }
                    ////    }, 100);
                    ////}
                }
            },
            watch: {
                //$data: {
                //    handler: function (val, oldVal) {
                //        var _self = this;
                //        //console.log('MIXIN_WATCH: ' + _self.viewKey);
                //        console.log('MIXIN_WATCH: ___init_com ' + _self.viewKey + ', is-dialog = ', _self.isDialog);
                //        //if (_self.isDialog != true)
                //        //    Vue.nextTick(function () { _self.___init_com(); });
                //    },
                //    deep: true
                //}
            },
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
                            //var vueLoading = window[_KITNAME]._getComponent(viewLoading);
                            //console.log(vueLoading);
                            _APP.view.loading = _COM[viewLoading];
                        });
                    }
                }
            }
        }

        function _showLogin(visiable) {
            if (visiable == false)
                _APP.view.main_body = null;
            else {
                if (_VIEW_CF.login) {
                    var viewLogin = _VIEW_CF.login.split('|')[0];
                    var cfLogin = _.find(_VIEWS, function (o) { return o.key == viewLogin; });
                    //console.log(cfLogin);
                    if (cfLogin) {
                        _viewInit(cfLogin, () => {
                            //var vueLogin = window[_KITNAME]._getComponent(viewLogin);
                            //console.log(vueLogin);
                            _APP.view.main_body = _COM[viewLogin];
                            _APP.view.loading = null;
                        });
                    }
                }
            }
        }

        function _showDashboard() {
            _mainLoading(false);
            _showLogin(false);

            var areas = _.filter(Object.keys(_VIEW_CF), function (o) {
                return (o.startsWith('sidebar')
                    || o.startsWith('header')
                    || o.startsWith('main')
                    || o.startsWith('footer')) && _VIEW_CF[o] != null && _VIEW_CF[o].length > 0;
            });
            //console.log(areas);
            areas.forEach((area) => {
                var viewName = _VIEW_CF[area];
                if (viewName && viewName.length > 0) {
                    viewName = viewName.split('|')[0];
                    var cf = _.find(_VIEWS, function (o) { return o.key == viewName; });
                    //console.log(area, viewName, cf);
                    if (cf) {
                        _viewInit(cf, () => {
                            //var v = window[_KITNAME]._getComponent(viewName);
                            _APP.view[area] = _COM[viewName];
                        });
                    }
                }
            });
        }

        ///////////////////////////////////////////////////////////

        function _appInit(cb) {
            _VIEW_CF = _getUrl('app/config.json?v=' + new Date().getTime(), 'json');
            _VIEWS = _getUrl('app/list.json?v=' + new Date().getTime(), 'json');
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
                    var fileSetting = _.find(cf.files, function (o) { return o == 'setting.json' });

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
                    } else _HTML[key] = '<div></div>';

                    if (fileSetting) {
                        var st = _getUrl('app/' + cf.scope + '/' + cf.name + '/setting.json', 'json');
                        st = st || { enable: true };
                        _COM_ST[key] = st;
                    } else _COM_ST[key] = { enable: true };

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
                        'var ' + dynName + ' = { mixins: [' + _KITNAME + '._getMixin()], \r\n template: ' + strTemplate + ', \r\n ' + textJs + ' \r\n\r\n ' +
                        _KITNAME + '._setComponent("' + key + '", ' + dynName + '); \r\n\r\n ' +
                        'Vue.component("' + key + '", ' + dynName + '); \r\n ';
                    var urlJs = URL.createObjectURL(new Blob([textJs], { type: 'text/javascript' }));
                    //console.log('VIEW_INIT: ' + key + ' = ' + urlJs);
                    head.load([{ vueComJs: urlJs }], cb);
                }
            }
        }

        return {
            _getMixin: function () { return _MIXIN; },
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
                        item['item_' + i] = libs[i] + '?v=' + new Date().getTime();
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
            },
            goScreen: function (screenName) {

            },
        };
    }

    var instance;
    return {
        getInstance: function (options) {
            if (instance === undefined) instance = new VueEngine(options);
            return instance;
        }
    };
})();
var _KITNAME = '_HW';
var _HW = Hiweb.getInstance();
_HW.init([
    'assets/libs/jquery-3.5.1.slim.min.js',
    'assets/libs/popper.min.js',
    'assets/libs/lodash.min.js',
    'assets/libs/classie.js',
    'assets/libs/vue.min.js',
    'assets/libs/bootstrap.min.js',
    'assets/libs/bootstrap.min.css',
    'assets/fonts/fontawesome5/all.css',
    'assets/fonts/feather/feather.css',
    'assets/libs/reset.css'
]);