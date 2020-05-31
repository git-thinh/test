// sessionStorage['USER_ID'] = 'zalo.5130398983683244855'; //Hook login
var USER_ID = sessionStorage['USER_ID'];
var PROFILE = { zalo: null };

var DEVICE_NAME = '';
if (window.innerWidth < 400) DEVICE_NAME = 'mobi'; else if (window.innerWidth < 1025) DEVICE_NAME = 'tablet'

var ___PATH_DOMAIN = '/_site/[' + location.host + ']/';
var ___XHR = new XMLHttpRequest();
___XHR.open('GET', '/_site/site.json', false);
___XHR.send(null);
if (___XHR.status === 200) {
    var domains = JSON.parse(___XHR.responseText);
    if (domains[location.host] != null)
        ___PATH_DOMAIN = '/_site/' + domains[location.host] + '/';
} else {
    alert('Please setting site.json');
}

if (location.href.indexOf('zalo_id') > 0) {
    var urlParams = new URLSearchParams(window.location.search);
    var zalo_id = urlParams.get('zalo_id');
    var msg = urlParams.get('msg');
    if (zalo_id) {
        USER_ID = 'zalo.' + zalo_id;
        sessionStorage['USER_ID'] = USER_ID;
        //alert(msg);
        location.href = location.href.split('?')[0];
    }
}

if (USER_ID && USER_ID.startsWith('zalo.')) {
    var id = USER_ID.substr(5);
    var fets = [
        fetch('zalo/file?file_name=' + id + '.profile.txt').then(r => r.text()),
        fetch('zalo/file?file_name=' + id + '.token.txt').then(r => r.text()),
        fetch('zalo/file?file_name=' + id + '.friend.txt').then(r => r.text()),
        fetch('zalo/file?file_name=' + id + '.invitable.txt').then(r => r.text()),
        fetch('zalo/file?file_name=' + id + '.task.txt').then(r => r.text())
    ];
    Promise.all(fets).then(async arr => {
        var a = arr.map(text => { return JSON.parse(text); });
        PROFILE.zalo = JSON.parse(a[0].data);
        PROFILE.zalo.token = a[1].data;
        PROFILE.zalo.friends = JSON.parse(a[2].data);
        PROFILE.zalo.invitables = JSON.parse(a[3].data);
        PROFILE.zalo.tasks = JSON.parse(a[4].data);

        console.log('???????????? PROFILE.zalo = ', PROFILE.zalo);
    })
}

/////////////////////////////////////////////////////////////////////

var ___guid_id = function () {
    return 'id-xxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
};

/////////////////////////////////////////////////////////////////////

var ___APP, ___VIEW = {}, ___VIEW_CF = {}, ___COM = {}, ___HTML = {},
    ___DL_CURRENT_EVENT = null, ___DL_CURRENT_ID = null,
    ___V_LOGOUT, ___V_MAIN;

var ___DATA = {
    view___loading: null,
    view___login: null,
    view___confirm_pass: null,
    view___screen_lock: null,

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
    objContact: {
        items: [
            {
                ok: true,
                id: 1,
                calls_to: [{ str_phone: "0975600710", time: new Date().getTime() }],
                str_token: "6171234525",
                str_username: "thinhnv",
                str_shortname: "Mr Thinh",
                str_phone: "0948003456",
                str_phones: "0948003456,0626111347",
                str_fullname: "Nguyễn Văn Thịnh",
                str_email: "thinhifis@gmail.com",
                str_avatar: "https://s120.avatar.talk.zdn.vn/7/5/c/b/1/120/ca96210ff1addff45f03e144ec4aa052.jpg",
                socials: [
                    {
                        type: "zalo",
                        name: "Nguyễn Thịnh",
                        id: 2998071883607128,
                        friends: [
                            {
                                id: 7287083737778696997,
                                str_phone: "",
                                str_username: "Nguyen Huu Sinh",
                                str_avatar: "https://s120.avatar.talk.zdn.vn/f/1/1/a/12/120/11ddc4a09d9273dc8a1bfd8d2e0adeac.jpg"
                            },
                            {
                                id: 7900271606406461606,
                                str_phone: "",
                                str_username: "Việt Cường",
                                str_avatar: "https://s120.avatar.talk.zdn.vn/5/a/7/0/5/120/ee11866fe3603fd00896d6459d47ffbf.jpg"
                            },
                            {
                                id: 5130398983683244855,
                                str_phone: "0926111347",
                                str_username: "Thinh",
                                str_avatar: "https://s120.avatar.talk.zdn.vn/7/5/c/b/1/120/ca96210ff1addff45f03e144ec4aa052.jpg"
                            }
                        ]
                    },
                    {
                        type: "facebook",
                        name: "Thịnh Nguyễn",
                        id: 2998071883607128,
                        friends: [
                            {
                                id: 7287083737778696997,
                                str_phone: "",
                                str_username: "Nguyen Huu Sinh",
                                str_avatar: "https://s120.avatar.talk.zdn.vn/f/1/1/a/12/120/11ddc4a09d9273dc8a1bfd8d2e0adeac.jpg"
                            },
                            {
                                id: 7900271606406461606,
                                str_phone: "",
                                str_username: "Việt Cường",
                                str_avatar: "https://s120.avatar.talk.zdn.vn/5/a/7/0/5/120/ee11866fe3603fd00896d6459d47ffbf.jpg"
                            },
                            {
                                id: 5130398983683244855,
                                str_phone: "0926111347",
                                str_username: "Thinh",
                                str_avatar: "https://s120.avatar.talk.zdn.vn/7/5/c/b/1/120/ca96210ff1addff45f03e144ec4aa052.jpg"
                            }
                        ]
                    }
                ]
            },
            {
                id: 2,
                str_phone: "0975600710",
                str_fullname: "Nguyễn Cẩm Tú",
                str_username: "Cẩm Tú",
                str_avatar: "https://scontent.fhph1-1.fna.fbcdn.net/v/t1.0-1/p160x160/18058007_114394709123150_2227901527307964772_n.jpg?_nc_cat=104&_nc_sid=dbb9e7&_nc_ohc=sCErkL5L_C0AX-LATtj&_nc_ht=scontent.fhph1-1.fna&_nc_tp=6&oh=cf751a8fc43294cf65b42d2f85a0e44b&oe=5EEE64A1"
            },
            {
                id: 3,
                str_phone: "",
                str_fullname: "Nguyễn Mạnh Hà",
                str_username: "Mạnh Hà",
                str_avatar: "https://s120-ava-talk.zadn.vn/9/5/d/0/23/120/949d6a7659fc2f2e27a0af0acc8e009e.jpg"
            },
            {
                id: 4,
                str_phone: "",
                str_fullname: "Chị Quỳnh",
                str_username: "Chị Quỳnh",
                str_avatar: "https://s120-ava-talk.zadn.vn/e/a/e/4/4/120/19abc4d6e7162510afbf8a2e36e1456f.jpg"
            },
            {
                id: 5,
                str_phone: "",
                str_fullname: "Nguyễn Quý Phi",
                str_username: "Quý Phi",
                str_avatar: "https://s120-ava-talk.zadn.vn/e/4/e/8/20/120/0e1d0af9a5c954bcfa12797d94961d3b.jpg"
            },
            {
                id: 7287083737778696997,
                str_phone: "",
                str_username: "Nguyen Huu Sinh",
                str_avatar: "https://s120.avatar.talk.zdn.vn/f/1/1/a/12/120/11ddc4a09d9273dc8a1bfd8d2e0adeac.jpg"
            },
            {
                id: 7900271606406461606,
                str_phone: "",
                str_username: "Việt Cường",
                str_avatar: "https://s120.avatar.talk.zdn.vn/5/a/7/0/5/120/ee11866fe3603fd00896d6459d47ffbf.jpg"
            },
            {
                str_fullname: "Domain Admin",
                str_email: "admin@phuquyland.net"
            },
            {
                str_fullname: "Nguyễn Tuấn Anh",
                str_email: "anhnt1@phuquyland.net"
            },
            {
                str_fullname: "INFO AROMA",
                str_email: "aroma@phuquyland.net"
            },
            {
                str_fullname: "Live Chat",
                str_email: "livechat@phuquyland.net"
            },
            {
                str_fullname: "Nguyễn Thanh Duân",
                str_email: "duannt@phuquyland.net"
            },
            {
                str_fullname: "Tuyển Dụng",
                str_email: "tuyendung@phuquyland.net"
            },
            {
                str_fullname: "landora facebook",
                str_email: "facebook@phuquyland.net"
            },
            {
                str_fullname: "Liên Giáp Thị Thúy",
                str_email: "liengtt@phuquyland.net"
            },
            {
                str_fullname: "Trần Văn Hiệp",
                str_email: "hieptv@phuquyland.net"
            },
            {
                str_fullname: "tan hoang ngoc",
                str_email: "tanhn1@phuquyland.net"
            },
            {
                str_fullname: "Phương Hoàng Lan",
                str_email: "phuonghl@phuquyland.net"
            },
            {
                str_fullname: "Tân Hoàng Ngọc",
                str_email: "tanhn@phuquyland.net"
            },
            {
                str_fullname: "Phạm Thu Hà",
                str_email: "haptt@phuquyland.net"
            },
            {
                str_fullname: "Nguyễn Mạnh Hà",
                str_email: "manhha@phuquyland.net"
            },
            {
                str_fullname: "Nguyễn Thanh Hùng",
                str_email: "hungnt@phuquyland.net"
            },
            {
                str_fullname: "Ngô Thị Thu Hương",
                str_email: "huongntt@phuquyland.net"
            },
            {
                str_fullname: "Nguyễn Thị Phương Hảo",
                str_email: "haontp@phuquyland.net"
            },
            {
                str_fullname: "Đường Thị Kim Khánh",
                str_email: "khanhdtk@phuquyland.net"
            },
            {
                str_fullname: "Anh Kiều Hoàng",
                str_email: "anhkh@phuquyland.net"
            },
            {
                str_fullname: "Phú Quý Land",
                str_email: "info@phuquyland.net"
            },
            {
                str_fullname: "Trương Tùng Lâm",
                str_email: "lamtt@phuquyland.net"
            },
            {
                str_fullname: "Long Lê Hữu",
                str_email: "longlh@phuquyland.net"
            },
            {
                str_fullname: "Thảo Lê Thị",
                str_email: "thaolt@phuquyland.net"
            },
            {
                str_fullname: "Hiên Lê Thị",
                str_email: "hienlt@phuquyland.net"
            },
            {
                str_fullname: "Oanh Lê Thị Kim",
                str_email: "oanhltk@phuquyland.net"
            },
            {
                str_fullname: "Anh Lê Tuấn",
                str_email: "anhlt@phuquyland.net"
            },
            {
                str_fullname: "Cường Lê Viết",
                str_email: "cuonglv@phuquyland.net"
            },
            {
                str_fullname: "Doanh Lê Văn",
                str_email: "doanhlv@phuquyland.net"
            },
            {
                str_fullname: "Huỳnh Lê Văn",
                str_email: "huynhlv@phuquyland.net"
            },
            {
                str_fullname: "Thịnh Mai Đức",
                str_email: "thinhmd@phuquyland.net"
            },
            {
                str_fullname: "Nguyễn Thị Mát",
                str_email: "matnt@phuquyland.net"
            },
            {
                str_fullname: "Nguyễn Văn Mạnh",
                str_email: "manhnv@phuquyland.net"
            },
            {
                str_fullname: "Minh Nguyễn Công",
                str_email: "minhnc@phuquyland.net"
            },
            {
                str_fullname: "Phúc Nguyễn Hữu",
                str_email: "phucnh@phuquyland.net"
            },
            {
                str_fullname: "Hưng Nguyễn Duy",
                str_email: "hungnd@phuquyland.net"
            },
            {
                str_fullname: "Việt Nguyễn Hoàng Tuấn",
                str_email: "vietnht@phuquyland.net"
            },
            {
                str_fullname: "Lan Nguyễn Mai",
                str_email: "lannm@phuquyland.net"
            },
            {
                str_fullname: "Huệ Nguyễn Thanh",
                str_email: "huent@phuquyland.net"
            },
            {
                str_fullname: "Long Nguyễn Thành",
                str_email: "longnt@phuquyland.net"
            },
            {
                str_fullname: "Duyên Nguyễn Thị",
                str_email: "duyennt1@phuquyland.net"
            },
            {
                str_fullname: "Loan Nguyễn Thị",
                str_email: "loannt@phuquyland.net"
            },
            {
                str_fullname: "Duyên Nguyễn Thị",
                str_email: "duyennt@phuquyland.net"
            },
            {
                str_fullname: "Thơ Nguyễn Thị",
                str_email: "thont@phuquyland.net"
            },
            {
                str_fullname: "Thơ Nguyễn Thị",
                str_email: "thont1@phuquyland.net"
            },
            {
                str_fullname: "Thủy Nguyễn Thị",
                str_email: "thuynt@phuquyland.net"
            },
            {
                str_fullname: "Dũng Nguyễn Tiến",
                str_email: "dungnt1@phuquyland.net"
            },
            {
                str_fullname: "Anh Nguyễn Tuấn",
                str_email: "anhnt@phuquyland.net"
            },
            {
                str_fullname: "Quân Nguyễn Tùng",
                str_email: "quannt@phuquyland.net"
            },
            {
                str_fullname: "Dũng Nguyễn Văn",
                str_email: "dungnv@phuquyland.net"
            },
            {
                str_fullname: "Sơn Nguyễn Văn",
                str_email: "sonnv@phuquyland.net"
            },
            {
                str_fullname: "Vượng Nguyễn Văn",
                str_email: "vuongnv@phuquyland.net"
            },
            {
                str_fullname: "Tây Nguyễn Văn",
                str_email: "taynv@phuquyland.net"
            },
            {
                str_fullname: "Thăng Nguyễn Xuân",
                str_email: "thangnx@phuquyland.net"
            },
            {
                str_fullname: "Đại Nguyễn Xuân",
                str_email: "dainx@phuquyland.net"
            },
            {
                str_fullname: "Nhung Ngô Thị Cẩm",
                str_email: "nhungntc@phuquyland.net"
            },
            {
                str_fullname: "Thảo Ngô Thị Tâm",
                str_email: "thaontt@phuquyland.net"
            },
            {
                str_fullname: "Lộc Ngô Văn",
                str_email: "locnv@phuquyland.net"
            },
            {
                str_fullname: "Võ Hồng Nhung",
                str_email: "nhungvh@phuquyland.net"
            },
            {
                str_fullname: "khách hàng online",
                str_email: "khachhang@phuquyland.net"
            },
            {
                str_fullname: "Nghĩa Phan Trọng",
                str_email: "nghiapt@phuquyland.net"
            },
            {
                str_fullname: "Nguyễn Quý Phi",
                str_email: "phinq@phuquyland.net"
            },
            {
                str_fullname: "Report PhuQuyLand",
                str_email: "report@phuquyland.net"
            },
            {
                str_fullname: "Land Phú Quý",
                str_email: "phuquyland.service@phuquyland.net"
            },
            {
                str_fullname: "Gia Phạm Hoàng",
                str_email: "giaph@phuquyland.net"
            },
            {
                str_fullname: "An Phạm Hải",
                str_email: "anph@phuquyland.net"
            },
            {
                str_fullname: "Linh Phạm Mai",
                str_email: "linhpm@phuquyland.net"
            },
            {
                str_fullname: "Phương Phạm Quỳnh",
                str_email: "phuongpq@phuquyland.net"
            },
            {
                str_fullname: "My Phạm Thảo",
                str_email: "mypt@phuquyland.net"
            },
            {
                str_fullname: "Thuỳ Phạm Thị",
                str_email: "thuypt1@phuquyland.net"
            },
            {
                str_fullname: "googleads quang cao",
                str_email: "googleads@phuquyland.net"
            },
            {
                str_fullname: "facebook Quảng cáo",
                str_email: "ads@phuquyland.net"
            },
            {
                str_fullname: "Nguyễn Thị Trúc Quỳnh",
                str_email: "trucquynh@phuquyland.net"
            },
            {
                str_fullname: "Quản Lý Sản Phẩm",
                str_email: "qlsp@phuquyland.net"
            },
            {
                str_fullname: "LOGIN TEAMVIEWER",
                str_email: "teamviewer@phuquyland.net"
            },
            {
                str_fullname: "Vũ Khắc Hoàng Thu",
                str_email: "thuvkh@phuquyland.net"
            },
            {
                str_fullname: "Thành Thái Văn",
                str_email: "thanhtv@phuquyland.net"
            },
            {
                str_fullname: "Thành Thân Quang",
                str_email: "thanhtq@phuquyland.net"
            },
            {
                str_fullname: "Đỗ Đức Thịnh",
                str_email: "thinhdd@phuquyland.net"
            },
            {
                str_fullname: "TIVI TIVI",
                str_email: "tivi@phuquyland.net"
            },
            {
                str_fullname: "Vũ Thị Minh Trang",
                str_email: "trangvtm@phuquyland.net"
            },
            {
                str_fullname: "Ân Trương Quốc",
                str_email: "antq@phuquyland.net"
            },
            {
                str_fullname: "An Trần Hoàng",
                str_email: "anth@phuquyland.net"
            },
            {
                str_fullname: "Thiện Trần Hoàng",
                str_email: "thienth@phuquyland.net"
            },
            {
                str_fullname: "Trang Trần Minh",
                str_email: "trangtm@phuquyland.net"
            },
            {
                str_fullname: "Khải Trần Xuân",
                str_email: "khaitx@phuquyland.net"
            },
            {
                str_fullname: "Ánh Trịnh Thị",
                str_email: "anhtt@phuquyland.net"
            },
            {
                str_fullname: "Vũ Văn Tuân",
                str_email: "tuanvv@phuquyland.net"
            },
            {
                str_fullname: "Tô Anh Tuấn",
                str_email: "tuanta@phuquyland.net"
            },
            {
                str_fullname: "Thùy Tống Thị Minh",
                str_email: "thuyttm@phuquyland.net"
            },
            {
                str_fullname: "Đào Đình Việt",
                str_email: "vietdd1@phuquyland.net"
            },
            {
                str_fullname: "Tư Đoàn Văn",
                str_email: "tudv@phuquyland.net"
            },
            {
                str_fullname: "Hoàng Văn Đông",
                str_email: "donghv@phuquyland.net"
            },
            {
                str_fullname: "Kiều Quang  Tiến Đạt",
                str_email: "datkqt@phuquyland.net"
            },
            {
                str_fullname: "Tình Đậu Thị",
                str_email: "tinhdt@phuquyland.net"
            },
            {
                str_fullname: "Long Đỗ Thế",
                str_email: "longdt@phuquyland.net"
            },
            {
                str_fullname: "Linh Đỗ Thị Thùy",
                str_email: "linhdtt@phuquyland.net"
            },
            {
                str_fullname: "Dũng Đỗ Tiến",
                str_email: "dungdt@phuquyland.net"
            },
            {
                str_fullname: "Việt Đỗ Đức",
                str_email: "vietdd@phuquyland.net"
            }
        ]
    },
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
        items: []
    },
    objMessage: {
        items: []
    },
    objViewMain: {
        str_name: '',
        str_title: '',
        on_search_text: function (keyword) { },
        objItemSelected: {
            id: 0,
            str_name: '',
            str_title: '',
        }
    }
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

            var id = 'idvc___' + _self._uid;
            el.setAttribute('id', id);
            _self.idvc___ = id;

            //console.log('MIXIN: ___init_class ' + _self.view_id + ', role = ', el.parentElement.getAttribute('role'));
            //console.log('MIXIN: ___init_class ' + _self.view_id + ', is-dialog = ', _self.isDialog);
            //console.log('MIXIN: ___init_class ' + _self.view_id + ', _uid = ', _self._uid);

            var pa = el.parentElement;
            if (pa && pa.hasAttribute('role')) {
                var role = pa.getAttribute('role');
                _self.role___ = role;
            } else if (_self.isDialog == true) {
                _self.role___ = 'dialog';
                classie.add(el, ___DL_CURRENT_ID);

                var rec = ___DL_CURRENT_EVENT.target.getBoundingClientRect();
                console.log('MIXIN: ___init_class ' + ___DL_CURRENT_ID + ', ' + _self.view_id + ', rec = ', rec);
                //_self.dialog___.opacity = 0;
                //_self.dialog___.top = rec.bottom + 'px';

                var dl = document.querySelector('.' + ___DL_CURRENT_ID);
                if (dl) {
                    dl.style.opacity = 0;
                    dl.style.top = rec.bottom + 'px';
                }

                setTimeout(function () {
                    var dl = document.querySelector('.' + ___DL_CURRENT_ID);
                    if (dl) {
                        var r1 = dl.getBoundingClientRect();
                        //console.log(rec.x + r1.width, window.innerWidth);
                        if (rec.x + r1.width > window.innerWidth) {
                            dl.style.left = 'auto';
                            dl.style.right = '0px';
                        } else {
                            dl.style.right = 'auto';
                            dl.style.left = rec.x + 'px';
                        }
                        dl.style.opacity = 1;
                    }
                }, 100);
            }
        },
        contact___get_name: function (o) {
            if (o) {
                if (o.str_shortname && o.str_shortname.length > 0) return o.str_shortname;
                if (o.str_fullname && o.str_fullname.length > 0) return o.str_fullname;
                if (o.str_username && o.str_username.length > 0) return o.str_username;
                if (o.str_email && o.str_email.length > 0) return o.str_email;
            }
            return '';
        },
        contact___has_avatar: function (o) {
            if (o && o.str_avatar && o.str_avatar.startsWith('http')) return true;
            return false;
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
                    var url_ = (___PATH_DOMAIN + '_view/' + vn + '/' + vi.name).toLowerCase();
                    fets.push(fetch(url_ + '.htm'));
                    fets.push(fetch(url_ + '.mobi.htm'));
                    fets.push(fetch(url_ + '.tablet.htm'));
                    fets.push(fetch(url_ + '.js'));
                    fets.push(fetch(url_ + '.css'));
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
                var text = '';

                //console.log(r.url, r.ok);

                if (type == 'js') {
                    if (r.ok) text = await r.text();
                    if (text.length == 0)
                        text = '{ data: function () { return {}; }, mounted: function () {}, methods: {} }';

                    text = text.trim().substr(1);

                    var str_template = '___HTML["' + key + '"]';
                    if (___HTML[key + '.' + DEVICE_NAME] != null) str_template = '___HTML["' + key + '.' + DEVICE_NAME + '"]';

                    text = '___COM["' + key + '"] = { mixins: [___MIXIN], template: ' + str_template + ', \r\n ' + text + ' \r\n ' +
                        'Vue.component("' + key + '", ___COM["' + key + '"]); \r\n ';
                    var url_js = URL.createObjectURL(new Blob([text], { type: 'text/javascript' }));

                    index = ___VIEW[scope].views.findIndex(function (o) { return o.name == api; });
                    if (index != -1) ___VIEW[scope].views[index].url_js = url_js;
                    ___VIEW[scope].views[index].ok = false;
                    ___VIEW[scope].views[index].key = key;

                    //console.log(scope, api, index);
                    //console.log(key, str_template);
                    //console.log(key, url_js);

                    a.push({ key: key, scope: scope, api: api, type: 'js', url_js: url_js });
                } else if (r.ok) {
                    text = await r.text();
                    switch (type) {
                        case 'tm': // htm
                            //console.log('HTM -> ', key);
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

/////////////////////////////////////////////////////////////////////

function touchHandler(event) {
    if (event.touches.length > 1) {
        //the event is multi-touch. you can then prevent the behavior
        event.preventDefault()
    }
}
window.addEventListener("touchstart", touchHandler, false);

var touchStartHandler = function (event) {
    if (___DL_CURRENT_ID && ___DL_CURRENT_EVENT) {
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
    }
};
window.addEventListener('DOMContentLoaded', (e) => {

    if ('ontouchstart' in document.documentElement) {
        document.addEventListener("touchstart", function (event) {
            if (___DL_CURRENT_ID && ___DL_CURRENT_EVENT) {
                touchStartHandler(event);
                event.preventDefault();
            }
        }, { passive: false });
    } else {
        document.onclick = function (event) { touchStartHandler(event); };
    }
});

window.addEventListener("hashchange", function (h) {
    var old_hash = new URL(h.oldURL).hash;
    var new_hash = location.hash;
    if (old_hash && old_hash.indexOf('#!') == 0) old_hash = old_hash.substr(2);
    if (new_hash && new_hash.indexOf('#!') == 0) new_hash = new_hash.substr(2);
    console.log('HASH_CHANGE: ' + old_hash + ' -> ', new_hash);
    view___load(new_hash, old_hash);
}, false);

/////////////////////////////////////////////////////////////////////

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

    sessionStorage['USER_ID'] = 'zalo.5130398983683244855'; //Hook login
    USER_ID = sessionStorage['USER_ID'];

    localStorage['USER_TOKEN'] = user.str_token;
    localStorage['USER'] = JSON.stringify(user);

    ___APP.$data.objUser = user;

    location.reload();
};

var ___logout = () => {
    sessionStorage.removeItem('USER_ID');
    USER_ID = null;

    localStorage.removeItem('USER_TOKEN');
    localStorage.removeItem('USER');

    Object.keys(___DATA).forEach(key => {
        if (key.indexOf('view___') == 0) {
            ___APP.$data[key] = null;
        }
    });

    location.reload();
};

/////////////////////////////////////////////////////////////////////

var setup_loading = function (visible) {
    var el = document.getElementById('setup_loading');
    if (el) el.style.display = (visible != false) ? 'block' : 'none';
};

var app___load_view = function () {
    if (USER_ID) {
        var a = Object.keys(___VIEW_CF);
        a.forEach((ky_) => {
            if (ky_.startsWith('___') == false) {
                var scope_view = ___VIEW_CF[ky_];
                if (scope_view && scope_view.length > 0) {
                    scope_view = scope_view.split('|')[0];
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
    } else {
        ___APP.$data.view___main_body = ___VIEW_CF['___login'];
    }
    setup_loading(false);
};

var app___init = function () {
    setup_loading();

    view___init((m) => {
        console.log('VIEW___INIT ----> ' + m.ok);
        if (m.ok == false) {
            alert('Call function view___init() fail...');
            return;
        }

        ___APP = new Vue({
            el: '#app',
            data: function () { return ___DATA; },
            mounted: function () {
                fetch(___PATH_DOMAIN + '_view/default.json').then(r => r.json()).then(cf_ => {
                    var a = Object.keys(cf_);
                    a.forEach((key) => {
                        var val = cf_[key].split('|')[0];
                        cf_[key] = val;
                    });
                    //console.log(cf_);
                    ___VIEW_CF = cf_;
                    app___load_view();
                });
            }
        });
    });
};

if (DEVICE_NAME == 'mobi') {
    head.load([
        { components_css: 'app/components.css?v=' + new Date().getTime() },
        { components_js: 'app/components.js?v=' + new Date().getTime() },
        { reset_css: 'app/reset.css?v=' + new Date().getTime() },
    ], app___init);
} else {
    head.load([
        { components_css: 'app/components.css?v=' + new Date().getTime() },
        { components_js: 'app/components.js?v=' + new Date().getTime() },

        { datatables_css: 'app/jquery.dataTables.min.css' },
        { datatables_js: 'app/jquery.dataTables.min.js' },
        { datatables_js_fix_col: 'app/dataTables.fixedColumns.min.js' },
        { datatables_js_select_col: 'app/dataTables.select.min.js' },

        { reset_css: 'app/reset.css?v=' + new Date().getTime() },
    ], app___init);
}