{
    data: function () {
        return {
            visible: false
        };
    },
    mounted: function () {
        var _self = this;

        //console.log('vue.logint: mounted, role = ', _self.role___);
        //$('#' + _self.idvc___ + ' .styled').uniform({ radioClass: 'choice' });

        //___APP.objViewMain.str_title = 'Tin nhắn';
        //___APP.objViewMain.objItemSelected.str_title = 'Tin nhắn abc';

        //Vue.nextTick(function () {
        //    _self.f_table_draw();
        //});

        var vue_table = _self.$refs['VUE_KIT_TABLE'];
        //console.log(vue_table);
        if (vue_table) {
            var obj = {
                id: 'Mã tin',
                str_group: 'Nhóm tin',
                str_subject: 'Chủ đề tin',
                str_content: 'Nội dung tin',
                str_action: 'Thao tác'
            };

            //vue_table.f_table_draw_empty(obj);
            vue_table.f_table_draw_loading(obj);

            setTimeout(function () {
                var arr_items = [];
                var options = {
                    fixed_left_column: 1,
                    fixed_right_column: 1
                };

                for (var i = 0; i < 1000; i++) {
                    var cols = Object.keys(obj), ait = [];
                    for (var j = 0; j < cols.length; j++) ait.push(j + 1);
                    arr_items.push(ait);
                }

                vue_table.f_table_draw(obj, arr_items, options);

            }, 1000);
        }
    },
    methods: {
    }
}
