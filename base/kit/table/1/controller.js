{
    data: function () {
        return {
            visible: false,
            titleCheck: '23456789',
            list: [
                { id: 1, text: 'a' },
                { id: 2, text: 'b' },
            ]
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
        },
        clickSomething: function(id) {
            alert(id)
        }
    }
}
