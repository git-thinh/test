{
    data: function () {
        return {
            visible: false,
            zalo_test_ids: []
        };
    },
    mounted: function () {
        var _self = this;
        console.log('vue.logint: mounted, role = ', _self.role___);

        //$('#' + _self.idvc___ +' .styled').uniform({ radioClass: 'choice' });

        //fetch('zalo/ids').then(r => r.json()).then(rs_ => {
        //    console.log('???????? = ', rs_);
        //    if (rs_.ok) {
        //        _self.zalo_test_ids = rs_.data;
        //    }
        //});
    },
    methods: {
        login_zalo_id: function(zalo_id) {
            sessionStorage['USER_ID'] = 'zalo.' + zalo_id;
            location.href = '/';
        },
        login_zalo: function() {
            location.href = 'zalo/login';
        },
        login: function() {
            //___login({
            //    ok: true,
            //    id: 1,
            //    str_token: "6171234525",
            //    str_username: "thinhnv",
            //    str_shortname: "Mr Thinh",
            //    str_phones: "0948003456,0626111347",
            //    str_fullname: "Nguyễn Văn Thịnh",
            //    str_email: "thinhifis@gmail.com"
            //});
        }
    }
}
