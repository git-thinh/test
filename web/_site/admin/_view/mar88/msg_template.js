{
    data: function () {
        return {
            visible: false,
            selected_raw_data: null,
            arr_test: [
                "Bán hàng||ABC Shop đang có chương trình khuyến mãi mua 3 tặng 1 với toàn bộ sản phẩm tại cửa hàng đến ngày 01-05/11. Bạn ghé ngay cửa hàng vào tại địa chỉ…để nhận ưu đãi",
                "Sự kiện|Chương trình khách hàng thân thiết|Nhận ngay discount đặc biệt với chương trình “Bữa Trưa Tiết Kiệm” cùng Gửi tin nhắn “SAVE” đến số điện thoại của chúng tôi để được giảm giá 20% cho lần đặt hàng tiếp theo của bạn!",
                "Sự kiện||Mang đến mùa hè bất ngờ cho con bạn bằng cách tham gia hoạt động của trung tâm miễn phí trong suốt tháng 7. Gọi ngay cho chúng tôi để đăng ký tham gia",
                "Quảng cáo||Mọi tựa sách yêu thích của bạn đang ở ngay đây! Toàn bộ sản phẩm của nhà sách <>hiện đã có sẵn trên điện thoại di động. Giảm giá lớn chỉ duy nhất 3 ngày, hãy nhanh tay truy cập ngay...",
                "Quảng cáo||Chúng tôi giới thiệu đến các bạn phần mềm SMS Marketing -  SMS BrandName chuyên nghiệp và giá rẻ, giúp các bạn có thể nhanh chóng gửi tin nhắn cho khách hàng.",
                "Sự kiện|Lời mời đăng ký|Hãy là người đầu tiên biết về chương trình ưu đãi giảm giá tại ! Nhấp vào để đăng ký ngay: <địa chỉ website>",
                "Sự kiện||Vé hiện đã có sẵn cho chương trình từ thiện. Mua hai vé để nhận được món quà bất ngờ! Xem thêm thông tin chi tiết tại: <Địa chỉ website>",
                "Chăm sóc||Xin thông báo: Quý khách có hẹn vào ngày mai lúc với . Mọi thắc mắc xin vui lòng liên hệ số điện thoại",
                "Chăm sóc||Cuộc hẹn kiểm tra răng miệng của bạn với nha sĩ  được lên lịch vào ngày tại phòng khám <địa chỉ>, mọi chi tiết xin liên hệ số điện thoại",
                "Chăm sóc||Ngân hàng xin thông báo cho bạn về cuộc hẹn lúc  tại địa chỉ <địa chỉ chi tiết>. Nhân viên hỗ trợ của bạn là , số điện thoại liên hệ",
                "Chăm sóc||Nhân viên từ sẽ hướng dẫn bạn xem nhà vào. Vui lòng phản hồi vào số điện thoại này nếu bạn có nhu cầu thay đổi thời gian",
                "Quảng cáo||Cua hang thoi trang ABC tung bung khuyen mai 50% nhan dip 20/10 cho tat ca cac chi em phu nu. Hay nhanh tay va sam cho minh bo canh moi nao. Chi tiet xem tai : ABC",
                "Quảng cáo||Nhan dip ra mat BST moi, ABC xin giảm giá 30% toan bo san pham tu ngay 20-22/10. Dac biet moi khach den mua hang se nhan duoc mot phan qua bi mat cua shop. Dia chi : acbzxy. Chi tiet xem tai: ABC.vn",
                "Quảng cáo||Khu biet thu cao cap ABC – House hien dang mo ban  voi gia cuc uu dai. Hay nhanh tay so huu cho minh mot can ho voi lai suat vay von 0% trong 30 thang dau tien. Lien he: 0987654321.",
                "Quảng cáo||Xin chao anh Hung, em xin duoc gioi thieu cho anh khu chung cu cao cap cua ABC. Hien tai khu chung cu dang duoc mo ban voi gia cuc soc, uu dai vay von 50% va lai suat chi 0% trong 20 thang dau. Thong tin lien he: 0987654321",
                "Quảng cáo||Nhan dip 2/9 ABC se giam gia 50% phan mem CRM cho doanh nghiep goi PRO CALL CENTER. Ap dung tu ngay 2/9 den het ngay 5/9. Xem chi tiet tai : https://ABC.vn",
                "Quảng cáo||Nhan dip 8/3 nha hang ABC se giam gia 20% tong hoa don voi khach hang nu. Chi ap dung trong ngay 8/3. Dia chi : abcxyz.",
                "Quảng cáo||Tang ngay phieu giam gia 1 trieu dong cho cac cap doi dang ky tour du lich Viet Nam – Singapore trong ngay 14-2. Hay cung nhau co mot chuyen di day thu vi voi ABC. Dang ky ngay tai : ABC.vn",
                "Quảng cáo||ABC moi cho ra mat phan mem SMS marketing moi nhat, hay nhanh tay truy cap vao website https://ABC.vn de la nguoi dau tien su dung phan mem hien dai nay. Hotline : 0948003456",
                "Quảng cáo||Tu ngay 6/10 cong ty ABC chinh thuc ra mat mau dien thoai thong minh the he moi, mau ma dep nhat tu truoc den nay… Lien he: ABC moi cho ra mat phan mem SMS marketing moi nhat, hay nhanh tay truy cap vao website https://ABC.vn de la nguoi dau tien su dung phan mem hien dai nay. Hotline : 0948003456",
                "Quảng cáo||Tu ngay 6/10 cong ty ABC chinh thuc ra mat mau dien thoai thong minh the he moi, mau ma dep nhat tu truoc den nay… Lien he: …",
                "Chăm sóc||Cam on anh Hung da su dung phan mem cua cua ABC. Neu co bat ky kho khan gi trong viec su dung phan mem hay lien den hotline : 0948003456 de duoc tu van truc tiep. Xem chi tiet tai: https://ABC.vn",
                "Chăm sóc||ABC xin chuc mung sinh nhat anh Hung! ABC xin tang anh Hung phieu qua tang tri quá 200.000đ. Phieu chi co tac dung tu ngay",
                "Chăm sóc||Xin ban hay tham gia dong gop y kien cho phan mem SMS marketing cua ABC va nhan that nhieu phan qua gia tri. Tham gia dong gop y kien tai: https://ABC.vn/dong-gop-y-kien.html",
                "Chăm sóc||Hay tham gia binh chon cho thi sinh ban yeu thich nhat tai chuong trinh ABC. Hay nhanh tay binh chon tai abc.com.vn va nhan nhieu phan qua hap dan cua chuong trinh.",
                "Chăm sóc||Cam on chi Hoa da su dung dich vu SMS Marketing cua eSMS. Neu co bat ky kho khan gi trong viec su dung dich vu hay lien den hotline : 0948003456 de duoc tu van truc tiep. Xem chi tiet tai: https://abc.vn",
                "Chăm sóc||abc xin chuc mung sinh nhat anh Hung! abc xin tang anh Hung phieu qua tang tri quá 200.000đ. Phieu chi co tac dung tu ngay …..",
                "Chăm sóc||Cam on ban da su dung dich vu tai ABC. Hay dong gop y kien cua ban ve dich vu cua chung toi va nhan cac phan qua hap dan tai dia chi: abcx",
                "Chăm sóc||Tu ngay 25/1 sieu thi dien may ABC ap dung chuong trinh khuyen mai len den 50% danh cho hang ngan san pham. Chi tiet xem tai: http://abcd",
                "Quảng cáo||Tu ngay …. ABC chinh thuc ra mat bo suu tap LOVE PARADISE, ghe tham cac chi nhanh cua ABC tren toan quoc de tro thanh nguoi dau tien so huu bo trang suc tuyet dep nay. Xem chi tiet tai: http://abcd",
                "Quảng cáo||ABC tung bung khai truong. Dang ky ngay cac chuyen du lich trong nuoc chi tu 3 trieu dong. Xem them tai http://abcd. Tu choi soan TC gui 6266."
            ]
        };
    },
    mounted: function () {
        var _self = this;

        //console.log('vue.logint: mounted, role = ', _self.role___);
        //$('#' + _self.idvc___ + ' .styled').uniform({ radioClass: 'choice' });

        ___APP.objViewMain.str_title = 'Tin nhắn';
        //___APP.objViewMain.objItemSelected.str_title = 'Tin nhắn abc';
        ___APP.objViewMain.on_search_text = _self.on_search_text;
        ___APP.objViewMain.on_select_changed = _self.on_select_changed;
        ___APP.objViewMain.on_menu_action_selected = _self.on_menu_action_selected;
        ___APP.objViewMain.arr_menu_actions = [
            {
                code: 'VIEW_ITEM',
                title: 'Xem thông tin',
                active: false
            },
            {
                code: 'ADD_ITEM',
                title: 'Thêm mới',
                active: true
            },
            {
                code: 'EDIT_ITEM',
                title: 'Cập nhật tin nhắn và lịch chạy',
                active: false
            },
            {
                code: 'ACTIVE_ITEM',
                title: 'Cho phép hoặc cấm sử dụng',
                active: false
            },
            //{
            //    code: 'SCHEDULE_ITEM',
            //    title: 'Lập lịch chạy gửi nhóm KH',
            //    active: false
            //},
            {
                code: 'REPORT_SEND_ITEM',
                title: 'Kết quả nhắn tin',
                active: false
            }
        ];

        _self.f_load_table();
    },
    methods: {
        on_search_text: function (keyword) {
            var _self = this;
            _self.f_load_table(keyword);
        },
        on_select_changed: function (is_select, row_data) {
            var _self = this;
            console.log('????? on_select_changed = ', is_select, row_data);

            if (is_select) {
                _self.selected_raw_data = row_data;
                ___APP.objViewMain.objItemSelected.str_title = row_data[0][1] + '[' + row_data[0][0] + '] - ' + row_data[0][2];
                ___APP.objViewMain.arr_menu_actions = [
                    {
                        code: 'VIEW_ITEM',
                        title: 'Xem thông tin',
                        active: true
                    },
                    {
                        code: 'ADD_ITEM',
                        title: 'Thêm mới',
                        active: true
                    },
                    {
                        code: 'EDIT_ITEM',
                        title: 'Cập nhật tin nhắn và lịch chạy',
                        active: true
                    },
                    {
                        code: 'ACTIVE_ITEM',
                        title: 'Cho phép hoặc cấm sử dụng',
                        active: true
                    },
                    //{
                    //    code: 'SCHEDULE_ITEM',
                    //    title: 'Lập lịch chạy gửi nhóm KH',
                    //    active: true
                    //},
                    {
                        code: 'REPORT_SEND_ITEM',
                        title: 'Kết quả nhắn tin',
                        active: true
                    }
                ];
            } else {
                _self.selected_raw_data = null;
                ___APP.objViewMain.objItemSelected.str_title = '';
                ___APP.objViewMain.arr_menu_actions = [
                    {
                        code: 'VIEW_ITEM',
                        title: 'Xem thông tin',
                        active: false
                    },
                    {
                        code: 'ADD_ITEM',
                        title: 'Thêm mới',
                        active: true
                    },
                    {
                        code: 'EDIT_ITEM',
                        title: 'Cập nhật tin nhắn và lịch chạy',
                        active: false
                    },
                    {
                        code: 'ACTIVE_ITEM',
                        title: 'Cho phép hoặc cấm sử dụng',
                        active: false
                    },
                    //{
                    //    code: 'SCHEDULE_ITEM',
                    //    title: 'Lập lịch chạy gửi nhóm KH',
                    //    active: false
                    //},
                    {
                        code: 'REPORT_SEND_ITEM',
                        title: 'Kết quả nhắn tin',
                        active: false
                    }
                ];
            }
        },
        on_menu_action_selected: function (menu_code) {
            var _self = this;
            console.log('????? on_menu_action_selected = ', menu_code);
            var view_code;

            switch (menu_code) {
                case 'VIEW_ITEM':
                    break;
                case 'ADD_ITEM':
                    view_code = 'mar88___msg_action_add_item';
                    break;
                case 'EDIT_ITEM':
                    break;
                case 'ACTIVE_ITEM':
                    break;
                //case 'SCHEDULE_ITEM':
                //    break;
                case 'REPORT_SEND_ITEM':
                    break;
            }
             
            view___load(view_code, 'popup');
        },
        f_load_table: function (keyword) {
            var _self = this;
            console.log('????? keyword = ', keyword);

            _self.selected_raw_data = null;
            ___APP.objViewMain.objItemSelected.str_title = '';

            var vue_table = _self.$refs['VUE_KIT_TABLE'];
            if (vue_table) {
                var obj = {
                    id: 'Mã',
                    str_group: 'Nhóm tin',
                    str_subject: 'Chủ đề tin',
                    str_content: 'Nội dung tin',
                    str_cus_segment: 'Nhóm khách hàng',
                    str_schedule: 'Lịch chạy',
                    str_action: 'Thao tác',
                    str_counter_cus_received: 'KH đã nhận',
                    str_state: 'Tình trạng'
                };

                //vue_table.f_table_draw_empty(obj);
                vue_table.f_table_draw_loading(obj);

                var arr_items = [];
                var options = {
                    fixed_left_column: 3,
                    fixed_right_column: 5
                };

                //for (var i = 0; i < 1000; i++) {
                //    var cols = Object.keys(obj), ait = [];
                //    for (var j = 0; j < cols.length; j++) ait.push(i + '.' + j);
                //    arr_items.push(ait);
                //} 
                _self.arr_test.forEach((text, index_) => {
                    if (keyword == null || text.toLowerCase().indexOf(keyword) != -1) {
                        var a = text.split('|');
                        arr_items.push([index_ + 1, a[0], a[1], a[2], 'Tất cả', '0 0/1 * * * ?', 'BROADCAST_ZALO', index_ == 0 ? 9 : 0, index_ == 0 ? 'Đang chạy' : 'Tạm dừng']);
                    }
                });

                vue_table.f_table_draw(obj, arr_items, options);
            }

        }
    }
}
