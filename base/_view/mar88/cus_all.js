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

        ___APP.objViewMain.str_title = 'Tin nhắn';
        ___APP.objViewMain.objItemSelected.str_title = 'Tin nhắn abc';

        Vue.nextTick(function () {
            var $table = $('#example');
            $table.css({ width: window.innerWidth + 'px' });
            $table.DataTable({
                destroy: true,
                //scrollY: "300px",
                scrollY: (window.innerHeight - 150) + 'px',
                scrollX: true,
                scrollCollapse: true,
                fixedColumns: {
                    leftColumns: 2,
                    rightColumns: 3
                },
                paging: false,
                ordering: false,
                info: false,
                dom: 'rtip',
                columnDefs: [
                    { targets: 1, className: 'dtm-cell-cus-name dtm-cell-fixed-col-left', title: 'Tên khách hàng' },
                    //{ targets: 6, className: 'dtm-cell-free', title: '????' },
                    //{ targets: 12, width: 200, className: 'dtm-cell-action', title: 'Tên khách hàng' }
                ]
            });
            setTimeout(function () {
                $('#example_wrapper').css({ width: window.innerWidth + 'px' });
                $('.DTFC_LeftBodyLiner table tbody tr td:last-of-type').css({ 'border-right': '1px solid #333' });
                $('.DTFC_RightBodyLiner table tbody tr td:first-child').css({ 'border-left': '1px solid #333' });
            }, 50);
        });
    },
    methods: {
        f_render_data: function() {
            var _self = this;
            var messages = ___APP.objMessage.items;
            var items = [];
            if (messages && Array.isArray(messages)) {
                var s = '', a = [];
                for (var i = 0; i < messages.length; i++) {
                    s = messages[i];
                    a = s.split('|');
                    var subs = _.filter(a, function (o, k) { return k < a.length - 1; });
                    items.push({
                        id: 0,
                        subjects: subs,
                        text: a[a.length - 1]
                    });
                }
            }
            return items;
        }
    }
}
