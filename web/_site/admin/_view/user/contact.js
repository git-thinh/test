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
        selected_friend: function(event, index) {
            var _self = this;
            $('#' + _self.idvc___ + ' .list-group-item').removeClass('active');
            $(event.target).closest('.list-group-item').addClass('active');
        }
    }
}
