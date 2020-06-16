{
    data: function () {
        return {
            visible: false
        };
    },
    mounted: function () {
        var _self = this;
        console.log('vue.logint: mounted, role = ', _self.role___);
        
        //$("a[data-toggle='tooltip']").tooltip();
    },
    methods: {
        login: function() {
        }
    }
}
