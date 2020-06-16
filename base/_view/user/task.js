{
    data: function () {
        return {
            visible: false,
            task_current: {
                visible: false,
                index: -1,
                item: { to_ids: [] },
                to_users: []
            },
            tasks: [],
            users: []
        };
    },
    mounted: function () {
        var _self = this;
        _self.tasks = PROFILE.zalo.tasks;
        console.log('vue.logint: mounted, role = ', _self.role___);
    },
    methods: {
        format_date: function(getTime_) {
            if (getTime_ == null || getTime_ == 0 || getTime_ == '0') return '';

            var date = new Date();
            date.setTime(getTime_);

            var hours = date.getHours();
            var minutes = date.getMinutes();
            var ampm = hours >= 12 ? 'pm' : 'am';
            hours = hours % 12;
            hours = hours ? hours : 12; // the hour '0' should be '12'
            minutes = minutes < 10 ? '0' + minutes : minutes;
            var strTime = hours + ':' + minutes + ' ' + ampm;
            return (date.getMonth() + 1) + "/" + date.getDate() + "/" + date.getFullYear() + "  " + strTime;
        },
        show_users_receiver: function(task, index) {
            var _self = this;
            _self.task_current.index = index;
            _self.task_current.item = task;
            if (_self.task_current.visible) {
                _self.task_current.visible = false;
                _self.users = [];
            } else {
                _self.task_current.visible = true;
                _self.users = PROFILE.zalo.friends;
            }
        },
        check_color_users_receiver: function(user_id) {
            var _self = this;
            var to_ids = _self.task_current.item.to_ids;

            if (to_ids && to_ids.length > 0 && to_ids[0] == '*') return 'text-primary';

            for (var i = 0; i < to_ids.length; i++)
                if (to_ids[i] == user_id) return 'text-primary';

            return 'text-black-50';
        },
        update_users_receiver: function(user_id) {
            var _self = this;

            var to_ids = _self.task_current.item.to_ids;
            if (to_ids && to_ids.length > 0 && to_ids[0] == '*') {
                var a = _.map(PROFILE.zalo.friends, 'id');
                var index = _self.task_current.index;
                PROFILE.zalo.tasks[index].to_ids = _.filter(a, function (o) { return o != user_id; });
                _self.tasks = PROFILE.zalo.tasks;                
            }

            _self.users = [];
            setTimeout(function () {
                _self.users = PROFILE.zalo.friends;
            }, 500);
        }
    }
}
