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
                            name: 'str_group',
                            type: 'dropdown',
                            caption: 'Nhóm tin',
                            list: [
                                { id: 1, text: 'Sự kiện', selected: true },
                                { id: 2, text: 'Bán hàng' },
                                { id: 3, text: 'Chăm sóc' },
                                { id: 4, text: 'Quảng cáo' }
                            ]
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
                            name: 'str_cus_segment',
                            type: 'dropdown',
                            caption: 'Nhóm khách hàng',
                            list: [
                                { id: 0, text: 'Tất cả', selected: true },
                                { id: 1, text: 'Khách hàng đã đăng ký vay tại F88 trên POL trong 30 ngày gần nhất' },
                                { id: 2, text: 'Khách hàng đã đăng ký vay tại F88 trên POL trong 60 ngày gần nhất' },
                                { id: 3, text: 'Khách hàng đã đăng ký vay nhưng không thành công tại PGD F88 trong trong 30 ngày gần nhất' },
                                { id: 4, text: 'Khách hàng đã đăng ký vay nhưng không thành công tại PGD F88 trong trong 60 ngày gần nhất' },
                                { id: 5, text: 'Khách hàng đã vay được tại F88 trong 30 ngày gần nhất' },
                                { id: 6, text: 'Khách hàng đã vay được tại F88 trong 60 ngày gần nhất' },
                                { id: 7, text: 'Khách hàng đã vay nhưng đang inactive dưới 60 ngày' },
                                { id: 8, text: 'Khách hàng đã vay nhưng đang inactive dưới 60-119 ngày' },
                                { id: 9, text: 'Khách hàng đã vay nhưng đang inactive dưới 120-239 ngày' },
                                { id: 10, text: 'Khách hàng đã vay nhưng đang inactive dưới từ 240 ngày trở lên' },
                                { id: 11, text: 'Khách hàng có điểm tín dụng TỐT' },
                                { id: 12, text: 'Khách hàng có điểm tín dụng KHÁ' },
                                { id: 13, text: 'Khách hàng có điểm tín dụng TRUNG' },
                                { id: 14, text: 'Khách hàng có tần suất vay tại F88 (tính theo số lượng HĐ đã mở)' },
                                { id: 15, text: 'Khách hàng có loại tài sản đã từng cầm cố tại F88' },
                                { id: 16, text: 'Khách hàng nhận ít nhất 1 trong 3 tin nhắn gần nhất' },
                                { id: 17, text: 'Khách hàng đọc ít nhất 1 trong 3 tin nhắn gần nhất' },
                                { id: 18, text: 'Khách hàng phản hồi lại ít nhất 1 trong 3 tin nhắn gần nhất' }
                            ]
                        },
                        {
                            name: 'str_schedule',
                            type: 'text',
                            caption: 'Lịch chạy',
                            value_default: '0 0/1 * * * ?'
                        },
                        {
                            name: 'str_state',
                            type: 'switch',
                            caption: 'Tình trạng',
                            list: [
                                { id: 1, text: 'Đang chạy' },
                                { id: 0, text: 'Tạm dừng', selected: true }
                            ]
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
            var v_popup = _self.$refs['REF_KIT_POPUP'];
            //console.log('v_popup = ', v_popup);
            if (v_popup) {
                var v_form = v_popup.$refs['REF_KIT_FORM'];
                if (v_form) {
                    var data = v_form.f_get_data();
                    console.log('data = ', data.items);
                }
            }
        },
        btn_close_click: function(event) {
            alert('btn_close_click');
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
