var Hiweb = (function () {
    function HiwebObject(options) {
        options = options || {};
        var _APP, _DATA = {
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

        function _appInit() {
            var cf = _getUrl('app/config.json', 'json');
            var cms = _getUrl('app/list.json', 'json');
            if (cf && cms) {
                _vueInit(() => {
                    if (cf.loading) {
                        var cfLoading = _.find(cms, function (o) { return o.key == cf.loading; });
                        if (cfLoading) {
                            //_APP.view.loading 

                            var files = cfLoading.files;
                            var fets = [];
                            for (var i = 0; i < files.length; i++)
                                fets.push(fetch(cfLoading.scope + '/' + cfLoading.name + '/' + files[i]));
                        }
                    }
                });
            } else {
                console.error('Cannot found the file: app/config.json or app/list.json');
            }
        }

        return {
            getApp: function () { return _APP; },
            init: function (libs) {
                libs = libs || [];
                if (libs.length > 0) {
                    var items = [];
                    for (var i = 0; i < libs.length; i++) {
                        var item = {};
                        item['item_' + i] = 'assets/libs/' + libs[i] + '?v='+ new Date().getTime();
                        items.push(item);
                    }
                    head.load(items, _appInit);
                }
            },
            mainLoading: function (visiable) {
                var el = document.getElementById('main_loading');
                if (el) {
                    if (visiable == false)
                        el.style.display = 'none';
                    else
                        el.style.display = 'inline-block';
                }
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


const hiw = Hiweb.getInstance();
hiw.init([
    'jquery-3.5.1.slim.min.js',
    'popper.min.js',
    'lodash.min.js',
    'classie.js',
    'vue.min.js',
    'bootstrap.min.js',
    'bootstrap.min.css',
    'reset.css'
]);