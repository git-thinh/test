{
    data: function () {
        return {
            visible: false
        };
    },
    mounted: function () {
        var _self = this;
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
        }
    }
}
