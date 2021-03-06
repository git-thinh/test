﻿{
    data: function () {
        return {
            visible: false
        };
    },
    mounted: function () {
        var _self = this;
        //console.log('vue.logint: mounted, role = ', _self.role___);
        //$('#' + _self.idvc___ + ' .styled').uniform({ radioClass: 'choice' });
    },
    methods: {
        menu_action_click: function(menu_code) {
            console.log('menu_action_click = ', menu_code);
            ___APP.objViewMain.on_menu_action_selected(menu_code);
        },
        item_click: function(event, scope___view, str_title) {
            var el;
            if (event && event.target) el = event.target;

            $(el.parentElement).children().removeClass('active');

            ___APP.objViewMain.str_title = str_title;
            ___APP.objViewMain.objItemSelected.str_title = '';

            switch (scope___view) {
                case 'mar88___cus_group':
                    break;
                case 'mar88___cus_label':
                    break;
                case 'mar88___cus_all':
                    break;
                case 'mar88___msg_template':
                    break;
                case 'mar88___msg_timer':
                    break;
                case 'mar88___report_broadcast_msg':
                    break;
            }

            view___load(scope___view);

            $(el).addClass('active');
        }
    }
}
