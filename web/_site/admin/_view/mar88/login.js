{
    data: function () {
        return {
            visible: false
        };
    },
    mounted: function () {
        var _self = this;
        console.log('vue.logint: mounted, role = ', _self.role___);
        //$('#' + _self.idvc___ + ' .styled').uniform({ radioClass: 'choice' });
    },
    methods: {
        login: function() {
            if (DEVICE_NAME != '') {
                alert('System only run on PC')
                return;
            }

            ___login({
                ok: true,
                id: 1,
                str_token: "6171234525",
                str_username: "thinhnv",
                str_shortname: "Mr Thinh",
                str_phones: "0948003456,0626111347",
                str_fullname: "Nguyễn Văn Thịnh",
                str_email: "thinhifis@gmail.com"
            });
        }
    }
}
