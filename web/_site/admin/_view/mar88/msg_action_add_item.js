{
    data: function () {
        return {
            visible: false,
            options: {
                items: []
            }
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

                _self.options = {
                    items: [
                        {
                            name: 'int_group', 
                            type: 'dropdown',
                            caption: 'Nhóm tin',
                            list: ___APP.objMar88.groups
                        },
                        {
                            name: 'str_subject',
                            type: 'text',
                            caption: 'Chủ đề tin'
                        },
                        {
                            name: 'str_content',
                            type: 'textarea',
                            caption: 'Nội dung tin'
                        },
                        {
                            name: 'int_cus_segment', 
                            type: 'dropdown',
                            caption: 'Nhóm khách hàng',
                            list: ___APP.objMar88.customer_segments
                        },
                        {
                            name: 'int_schedule',
                            type: 'dropdown',
                            caption: 'Lịch chạy',
                            value_default: '0 0/1 * * * ?',
                            list: ___APP.objMar88.schedules
                        },
                        {
                            name: 'int_state',
                            type: 'switch',
                            caption: 'Tình trạng',
                            list: ___APP.objMar88.message_states
                        }
                    ]
                };
                v_form.f_init(_self.options);

            }
        }


    },
    methods: {
        btn_submit_click: function(event) {
            //alert('btn_submit_click');
            var _self = this;
            var obj = {};
            var v_popup = _self.$refs['REF_KIT_POPUP'];
            //console.log('v_popup = ', v_popup);
            if (v_popup) {
                var v_form = v_popup.$refs['REF_KIT_FORM'];
                if (v_form) {
                    var data = v_form.f_get_data();
                    //console.log('data = ', data.items);
                    if (data && data.items) {
                        for (var i = 0; i < data.items.length; i++) {
                            var it = data.items[i];
                            if (it.list)
                                obj[it.name] = it.value.id;
                            else
                                obj[it.name] = it.value;
                        }
                    }
                }
            }

            if (Object.keys(obj).length > 0) {
                //___APP.objMar88.groups
                //___APP.objMar88.customer_segments
                //___APP.objMar88.schedules
                //___APP.objMar88.message_states
                // int_group, str_subject, str_content, int_cus_segment, int_schedule, int_state

                obj.str_group = '';
                obj.str_cus_segment = '';
                obj.str_schedule = '';
                obj.str_state = '';

                if (obj.int_group) {
                    var it = _.find(___APP.objMar88.groups, function (x) { return x.id == obj.int_group; });
                    if (it) obj.str_group = it.text;
                }

                if (obj.int_cus_segment) {
                    var it = _.find(___APP.objMar88.customer_segments, function (x) { return x.id == obj.int_cus_segment; });
                    if (it) obj.str_cus_segment = it.text;
                }

                if (obj.int_schedule) {
                    var it = _.find(___APP.objMar88.schedules, function (x) { return x.id == obj.int_schedule; });
                    if (it) obj.str_schedule = it.text;
                }

                if (obj.int_state) {
                    var it = _.find(___APP.objMar88.message_states, function (x) { return x.id == obj.int_state; });
                    if (it) obj.str_state = it.text;
                }
            }

            if (obj.str_subject == null || obj.str_subject.toString().length == 0) {
                ___error('Vui lòng nhập tiêu đề tin nhắn');
                return;
            }

            if (obj.str_content == null || obj.str_content.toString().length == 0) {
                ___error('Vui lòng nhập nội dung tin nhắn');
                return;
            }

            console.log(obj);

            ___alert('Tạo tin nhắn thành công');
        },
        btn_close_click: function(event) {
            //alert('btn_close_click');
        },
        on_created_addnew: function() {
            //console.log('000000000000000 ?????????????????????');
        },
        on_mounted_addnew: function() {
            //console.log('111111111111111 ?????????????????????');

            //////// Fetch all the forms we want to apply custom Bootstrap validation styles to
            //////var forms = document.getElementsByClassName('needs-validation');
            //////// Loop over them and prevent submission
            //////var validation = Array.prototype.filter.call(forms, function (form) {
            //////    form.addEventListener('submit', function (event) {
            //////        if (form.checkValidity() === false) {
            //////            event.preventDefault();
            //////            event.stopPropagation();
            //////        }
            //////        form.classList.add('was-validated');
            //////    }, false);
            //////});

        }
    }
}
