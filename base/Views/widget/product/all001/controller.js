{
    data: function () {
        return {
            visible: false
        };
    },
    mounted: function () {
        var _self = this;
        //console.log('vue.logint: mounted, role = ', _self.role___);
        //$('#' + _self.idvc___ +' .styled').uniform({ radioClass: 'choice' });
    },
    methods: {
        _onActionEvent: function(event, codeAction) {
            console.log(codeAction);
        }
    }
}
