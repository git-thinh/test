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

        var v_popup = _self.$refs['REF_KIT_POPUP'];
        //console.log('v_popup = ', v_popup);
        if (v_popup) {
            var v_form = v_popup.$refs['REF_KIT_FORM'];
            //console.log('v_form = ', v_form);
            if (v_form) {
                var obj = ___APP.objViewMain.objSelected;

                ////var obj = {
                ////    id: row_data[0][0],
                ////    str_group: row_data[0][1],
                ////    str_subject: row_data[0][2],
                ////    str_content: row_data[0][3],
                ////    str_cus_segment: row_data[0][4],
                ////    str_schedule: row_data[0][5],
                ////    str_counter_cus_received: row_data[0][6],
                ////    str_state: row_data[0][7]
                ////};

                var options = {
                    items: [
                        {
                            type: 'dropdown',
                            caption: 'Nhóm tin',
                            list: ___APP.objMar88.groups
                        },
                        {
                            type: 'text',
                            caption: 'Chủ đề tin',
                            value_default: obj.str_subject
                        },
                        {
                            type: 'textarea',
                            caption: 'Nội dung tin',
                            value_default: obj.str_content
                        },
                        {
                            type: 'dropdown',
                            caption: 'Nhóm khách hàng',
                            list: ___APP.objMar88.customer_segments
                        },
                        {
                            type: 'text',
                            caption: 'Lịch chạy',
                            value_default: '0 0/1 * * * ?',
                            list: ___APP.objMar88.schedules
                        },
                        {
                            type: 'switch',
                            caption: 'Tình trạng',
                            list: ___APP.objMar88.message_states
                        }
                    ]
                };
                v_form.f_init(options);
            }
        }
    },
    methods: {
        btn_submit_click: function(event) {
            alert('btn_submit_click');
        },
        btn_close_click: function(event) {
            alert('btn_close_click');
        },
        on_created_addnew: function() {
            //console.log('000000000000000 ?????????????????????');
        },
        on_mounted_addnew: function() {
            //console.log('111111111111111 ?????????????????????');

            // Fetch all the forms we want to apply custom Bootstrap validation styles to
            var forms = document.getElementsByClassName('needs-validation');
            // Loop over them and prevent submission
            var validation = Array.prototype.filter.call(forms, function (form) {
                form.addEventListener('submit', function (event) {
                    if (form.checkValidity() === false) {
                        event.preventDefault();
                        event.stopPropagation();
                    }
                    form.classList.add('was-validated');
                }, false);
            });

        }
    }
}
