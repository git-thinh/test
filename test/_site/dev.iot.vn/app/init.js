var ___PATH_DOMAIN = '/_site/' + location.host + '/';
var ___APP, ___VIEW = {}, ___COM = {}, ___HTML = {},
    ___DL_CURRENT_EVENT = null, ___DL_CURRENT_ID = null,
    ___V_LOGOUT, ___V_MAIN;

var ___DATA = {
    view___sidebar_left: null,
    view___sidebar_right: null,

    view___header_top: null,
    view___header_breadcrumb: null,
    view___header_tab: null,
    view___header_filter: null,
    view___main_left: null,
    view___main_body: null,
    view___footer: null,

    view___dialog_1: null,
    view___dialog_2: null,
    view___dialog_3: null,

    objApp: {
        is_mobile: window.innerWidth < 481,
        int_width: window.innerWidth
    },
    objUser: {},
    objKanban: {
        items: [
            {
                name: 'backlog',
                title: 'Backlog',
                order: 0
            },
            {
                name: 'waiting',
                title: 'Waiting',
                order: 0
            },
            {
                name: 'doing',
                title: 'Doing',
                order: 0
            },
            {
                name: 'in_review',
                title: 'In Review',
                order: 0
            },
            {
                name: 'done',
                title: 'Done',
                order: 0
            }
        ]
    },
    objProject: {
        items:[]
    },
};

var ___MIXIN = {
    props: [
        'is-dialog',
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
        view_id: function () { return this.$vnode.componentOptions.tag; }
    },
    created: function () { },
    mounted: function () {
        var _self = this;
        _self.___init_class();
        console.log('MIXIN: mounted -> ' + _self.view_id);
        if (_self.view_id.indexOf('___logout') != -1) ___V_LOGOUT = _self;
    },
    methods: {
        ___init_class: function () {
            var _self = this;
            var el = _self.$el;
            classie.add(el, '___com').add(el, _self.view_id);

            //console.log('MIXIN: ___init_class ' + _self.view_id + ', role = ', el.parentElement.getAttribute('role'));
            //console.log('MIXIN: ___init_class ' + _self.view_id + ', is-dialog = ', _self.isDialog);

            var pa = el.parentElement;
            if (pa && pa.hasAttribute('role')) {
                var role = pa.getAttribute('role');
                _self.role___ = role;
            } else if (_self.isDialog == true) {
                _self.role___ = 'dialog';
                classie.add(el, ___DL_CURRENT_ID);

                var rec = ___DL_CURRENT_EVENT.target.getBoundingClientRect();
                //console.log('MIXIN: ___init_class ' + _self.view_id + ', rec = ', rec);
                _self.dialog___.top = rec.bottom + 'px';
                _self.dialog___.left = rec.x + 'px';
            }
        }
    },
    watch: {
        $data: {
            handler: function (val, oldVal) {
                var _self = this;
                //console.log('MIXIN_WATCH: ' + _self.view_id);
                console.log('MIXIN_WATCH: ___init_class ' + _self.view_id + ', is-dialog = ', _self.isDialog);
                if (_self.isDialog != true)
                    Vue.nextTick(function () { _self.___init_class(); });
            },
            deep: true
        }
    },
};

var view___init = (callback) => {
    fetch(___PATH_DOMAIN + '_view/config.json').then(r => r.json()).then(async cf_ => {
        ___VIEW = cf_;
        var fets = [];
        for (var vn in cf_) {
            if (cf_[vn] && cf_[vn].views) {
                cf_[vn].views.forEach((vi) => {
                    fets.push(fetch((___PATH_DOMAIN + '_view/' + vn + '/' + vi.name + '.htm').toLowerCase()));
                    fets.push(fetch((___PATH_DOMAIN + '_view/' + vn + '/' + vi.name + '.js').toLowerCase()));
                    fets.push(fetch((___PATH_DOMAIN + '_view/' + vn + '/' + vi.name + '.css').toLowerCase()));
                });
            }
        }

        var results = await Promise.all(fets).then(async fs => {
            var a = [];
            for (var i = 0; i < fs.length; i++) {
                var r = fs[i];
                var type = r.url.substr(r.url.length - 2, 2);
                var p = r.url.split('/');
                var scope = p[p.length - 2];
                var api = p[p.length - 1].split('.')[0];
                var key = scope + '___' + api;
                var index = -1;

                //console.log(r.url, r.ok);

                if (type == 'js') {
                    if (r.ok) text = await r.text();
                    else text = '{ data: function () { return {}; }, mounted: function () {}, methods: {} }';

                    text = text.trim().substr(1);
                    text = '___COM["' + key + '"] = { mixins: [___MIXIN], template: ___HTML["' + key + '"], \r\n ' + text + ' \r\n ' +
                        'Vue.component("' + key + '", ___COM["' + key + '"]); \r\n ';
                    var url_js = URL.createObjectURL(new Blob([text], { type: 'text/javascript' }));

                    index = ___VIEW[scope].views.findIndex(function (o) { return o.name == api; });
                    //console.log(scope, api, index);
                    if (index != -1) ___VIEW[scope].views[index].url_js = url_js;
                    ___VIEW[scope].views[index].ok = false;
                    ___VIEW[scope].views[index].key = key;

                    a.push({ key: key, scope: scope, api: api, type: 'js', url_js: url_js });
                } else if (r.ok) {
                    text = await r.text();
                    switch (type) {
                        case 'tm': // htm
                            ___HTML[key] = text;
                            break;
                        case 'ss': // css
                            var url_css = ___PATH_DOMAIN + '_view/' + scope + '/' + api + '.css';
                            //var url_css = URL.createObjectURL(new Blob([text], { type: 'text/css' }));
                            //index = ___VIEW[scope].views.findIndex(function (o) { return o.name == api; });
                            ////console.log(scope, api, index);
                            //if (index != -1) ___VIEW[scope].views[index].url_css = url_css;

                            var el = document.createElement('link');
                            el.setAttribute('rel', 'stylesheet');
                            el.setAttribute('href', url_css);
                            el.setAttribute('id', key + '___css');
                            document.getElementsByTagName('head')[0].appendChild(el);
                            break;
                    }
                }
            }
            //console.log('a === ', a);
            return a;
        }).then(async a => {
            var js_add = await Promise.all(a.map(it => {
                if (___HTML[it.key] == null) ___HTML[it.key] = '<div></div>';
                return new Promise((resolve, reject) => {
                    var el = document.createElement('script');
                    el.setAttribute('src', it.url_js);
                    el.setAttribute('id', it.key + '___js');
                    el.onload = function () {
                        resolve(it);
                    };
                    document.getElementsByTagName('head')[0].appendChild(el);
                });
            }));
            return js_add;
        });

        //console.log(JSON.stringify(results));
        results.forEach(r => {
            var index = ___VIEW[r.scope].views.findIndex(function (o) { return o.name == r.api; });
            if (index != -1) {
                var it = ___VIEW[r.scope].views[index];
                it.ok = true;
                console.log('VIEW___INIT: ' + it.key + ' = true');

                //if (it.ok && it.area_init != null && it.area_init.length > 0) {
                //    ___DATA['view___' + it.area_init] = r.key;
                //    console.log('VIEW___INIT: ' + it.area_init + ' ----> ' + r.key);
                //}
            }
        });

        callback({ ok: true, configs: results });
    }).catch((err) => callback({ ok: false, message: err.message }));
};

var view___get = (scope_, name_) => {
    if (Array.isArray(___VIEW[scope_].views)) {
        var views = _.filter(___VIEW[scope_].views, function (o) { return o.name != null && o.name == name_; });
        if (views.length > 0) return views[0];
    }
    return null;
};

view___init((m) => {
    console.log('VIEW___INIT ----> ' + m.ok);
    if (m.ok == false) return;

    ___APP = new Vue({
        el: '#app',
        data: function () { return ___DATA; },
        mounted: function () {
            var _self = this;
            Vue.nextTick(function () {
                _self.reload();
            });
        },
        methods: {
            reload: function () {
                var _self = this;

                if (localStorage['USER_TOKEN'] == null) {
                    ___APP.$data.view___main_body = 'user___login';
                } else {
                    ___APP.$data.objUser = JSON.parse(localStorage['USER']);

                    fetch(___PATH_DOMAIN + '_view/default.json').then(r => r.json()).then(async cf_ => {
                        for (var ky_ in cf_) {
                            var scope_view = cf_[ky_];
                            if (scope_view && scope_view.length > 0) {
                                var a = scope_view.split('___');
                                if (a.length == 2) {
                                    var obj_view = view___get(a[0], a[1]);
                                    if (obj_view) {
                                        console.log('VIEW___RELOAD: ' + ky_ + ' === ' + scope_view);
                                        ___APP.$data['view___' + ky_] = obj_view.key;
                                    }
                                }
                            }
                        }
                    });

                }
            }
        }
    });
});

window.addEventListener("hashchange", function (h) {
    var old_hash = new URL(h.oldURL).hash;
    var new_hash = location.hash;
    if (old_hash && old_hash.indexOf('#!') == 0) old_hash = old_hash.substr(2);
    if (new_hash && new_hash.indexOf('#!') == 0) new_hash = new_hash.substr(2);
    console.log('HASH_CHANGE: ' + old_hash + ' -> ', new_hash);
    view___load(new_hash, old_hash);
}, false);
window.addEventListener('DOMContentLoaded', (e) => {
    document.onclick = function (event) {
        var event_id = event.target.getAttribute('id');
        //console.log('DOC_CLICK: CURRENT_ID = ' + ___DL_CURRENT_ID + ', event_id = ' + event_id);
        if (___DL_CURRENT_ID && ___DL_CURRENT_ID != event_id) {
            var dl = event.target.closest('.' + ___DL_CURRENT_ID);
            //console.log('DOC_CLICK: dl = ', dl == null ? '' : ' dialog_current -> close it');
            if (dl == null) {
                // Click out of DIALOG -> close it
                ___APP.$data.view___dialog_1 = null;
                ___DL_CURRENT_ID = null;
                ___DL_CURRENT_EVENT.target.removeAttribute('id');
                ___DL_CURRENT_EVENT = null;
            }
        }
    };
});


var view___load = (view, view_called) => {
    if (view == null || view.length == 0) return;

    if (___HTML[view] == null)
        return console.error('VIEW___LOAD: ERROR -> ' + view + ': Template HTML is not exist');

    var cf;
    var views = Object.keys(___VIEW);
    for (var i = 0; i < views.length; i++) {
        if (Array.isArray(___VIEW[views[i]].views)) {
            for (var j = 0; j < ___VIEW[views[i]].views.length; j++) {
                if (___VIEW[views[i]].views[j].key == view) {
                    cf = ___VIEW[views[i]].views[j];
                    break;
                }
            }
        }
        if (cf != null) break;
    }

    console.log('VIEW___LOAD: ' + view);

    if (cf && cf.ok) {
        ___APP.view___main_body = view;
    } else console.error('VIEW___LOAD: ERROR -> ' + view + ': ok is false Or Config is null', JSON.stringify(cf));
};

var view___dialog = (event, view) => {
    var event_id = event.target.getAttribute('id');
    console.log('DIALOG: ' + view + ', CURRENT_ID = ' + ___DL_CURRENT_ID + ', event_id = ' + event_id);

    if (___DL_CURRENT_ID && event_id == null) {
        // Close dialog current is not itself, after open other dialog
        console.log('DIALOG: ' + view + ' -> [1] ');
        ___APP.$data.view___dialog_1 = null;
        ___DL_CURRENT_ID = null;
        ___DL_CURRENT_EVENT.target.removeAttribute('id');
        ___DL_CURRENT_EVENT = null;
    } else if (___DL_CURRENT_ID && ___DL_CURRENT_ID == event_id) {
        // Close dialog current is itself
        console.log('DIALOG: ' + view + ' -> [2] ');
        ___APP.$data.view___dialog_1 = null;
        ___DL_CURRENT_ID = null;
        ___DL_CURRENT_EVENT.target.removeAttribute('id');
        ___DL_CURRENT_EVENT = null;
        return;
    }

    setTimeout(function (ev) {
        ___DL_CURRENT_EVENT = ev;
        ___DL_CURRENT_ID = 'dl-' + (new Date()).getTime();
        ev.target.setAttribute('id', ___DL_CURRENT_ID);
        // Open dialog
        ___APP.$data.view___dialog_1 = view;
    }, 1, event);
};

var ___login = (user) => {
    console.log('___login: user = ', user);
    localStorage['USER_TOKEN'] = user.str_token;
    localStorage['USER'] = JSON.stringify(user);
    ___APP.$data.objUser = user;
    ___APP.reload();
};

var ___logout = () => {
    if (___V_LOGOUT && ___V_LOGOUT.logout)
        ___V_LOGOUT.logout((ok) => {
            localStorage.removeItem('USER_TOKEN');
            localStorage.removeItem('USER');

            Object.keys(___DATA).forEach(key => {
                if (key.indexOf('view___') == 0) {
                    ___APP.$data[key] = null;
                }
            });

            view___load('user___login');

        });
};