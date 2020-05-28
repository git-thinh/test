
___COM["kit_datatables_net"] = {
    mixins: [___MIXIN],
    template:
        '<div class="kit-datatables-net">' +
        '   <table :id="table_id___" class="stripe row-border order-column"></table>' +
        '   <div :id="pager_id___" class="pl-1 pr-1"></div>' +
        '</div>',
    data: function () {
        return {
            table_id___: ___guid_id(),
            pager_id___: ___guid_id(),
        };
    },
    created: function () {
        var _self = this;
    },
    mounted: function () {
        var _self = this;
        //console.log('vue.logint: mounted, role = ', _self.role___);
        //$('#' + _self.idvc___ + ' .styled').uniform({ radioClass: 'choice' });

        ___APP.objViewMain.str_title = 'Tin nhắn';
        ___APP.objViewMain.objItemSelected.str_title = 'Tin nhắn abc';

        //Vue.nextTick(function (_sf) {
        _self.f_table_draw();
        //}, _self);
    },
    ////watch: {
    ////    'list': {
    ////        handler: function (after, before) {
    ////            var _self = this;
    ////        }, deep: true
    ////    }
    ////},
    methods: {
        f_render_data: function () {
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
                        id: (i + 1),
                        str_subject: subs.join(', '),
                        str_content: a[a.length - 1]
                    });
                }
            }
            return items;
        },
        f_table_draw_empty: function (obj) {
            var _self = this;
            var table_id___ = _self.table_id___;
            var pager_id___ = _self.pager_id___;

            if ($.fn.DataTable.isDataTable('#' + table_id___)) {
                $('#' + table_id___).dataTable().fnClearTable();
                $('#' + table_id___).dataTable().fnDestroy();
            }

            if (obj == null) {
                console.error('ERROR: f_table_draw_empty > config { name: "...", title: "..." } ');
                return;
            }

            var s_th = '', col_titles = Object.keys(obj);

            for (var i = 0; i < col_titles.length; i++) {
                var key = col_titles[i];
                if (obj[key])
                    s_th += '<th>' + obj[key] + '</th>';
                else
                    s_th += '<th></th>';
            }

            $('#' + table_id___).html('<thead><tr>' + s_th + '</tr></thead><tbody></tbody>');

            $('#' + table_id___).DataTable({
                destroy: true,
                paging: false,
                info: false,
                dom: 'rtip',
                data: [],
                language: {
                    emptyTable: 'Không có bản ghi'
                }
            });
        },
        f_table_draw: function (items) {
            var _self = this;
            var table_id___ = _self.table_id___;
            var pager_id___ = _self.pager_id___;

            var obj = {
                id: 'Mã tin',
                str_group: 'Nhóm tin',
                str_subject: 'Chủ đề tin',
                str_content: 'Nội dung tin',
                str_action: 'Thao tác'
            };

            if (items == null || items.length == 0) {
                _self.f_table_draw_empty(obj);
                return;
            }




            var fixed_left_column = 2, fixed_right_column = 0;







            var s_th = '', data_default = [], col_titles = Object.keys(obj);
            var a_columnDefs = [];

            for (var i = 0; i < col_titles.length; i++) {
                var col_name = col_titles[i];

                if (obj[col_name])
                    s_th += '<th>' + obj[col_name] + '</th>';
                else
                    s_th += '<th></th>';

                if (fixed_right_column > 0 && i == col_titles.length - fixed_right_column - 1) {
                    //console.log('????????????????????????? === ', i + 1);
                    s_th += '<th></th>';
                    data_default.push('');
                    a_columnDefs = [{ targets: (i + 1), className: 'dtm-cell-free', title: '' }];
                }

                data_default.push('');
            }
            $('#' + table_id___ + ' thead tr').html(s_th);

            var $table = $('#' + table_id___);
            $table.css({ width: window.innerWidth + 'px' });
            //$table.DataTable();

            $table.DataTable({
                destroy: true,
                //scrollY: "300px",
                scrollY: (window.innerHeight - 150) + 'px',
                scrollX: true,
                scrollCollapse: true,
                fixedColumns: {
                    leftColumns: fixed_left_column,
                    rightColumns: fixed_right_column
                },
                paging: false,
                ordering: false,
                info: false,
                dom: 'rtip',
                //data: [data_default],
                columnDefs: a_columnDefs,
                language: {
                    emptyTable: "3333333333333333333333333"
                }
            });

            setTimeout(function () {

                //////if ($.fn.DataTable.isDataTable('#example')) {
                //////    $('#example').dataTable().fnClearTable();
                //////    $('#example').dataTable().fnDestroy();
                //////}

                $('#' + table_id___ + '_wrapper').css({ width: window.innerWidth + 'px' });
                //$('.DTFC_LeftBodyLiner table tbody tr td:last-of-type').css({ 'border-right': '1px solid #333' });
                //$('.DTFC_RightBodyLiner table tbody tr td:first-child').css({ 'border-left': '1px solid #333' });

                //var it_ = { id: 1, str_subject: '', str_content: '' };
            }, 50);

        }
    }
};
Vue.component('kit_datatables_net', ___COM["kit_datatables_net"]);







