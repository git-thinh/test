{
    props: [
        'title'
    ],
    data: function () {
        return {
            visible: false,
            label: ''
        };
    },
    mounted: function () {
        var _self = this;
        //if (_self.title && _self.title.length > 0) {
        //    _self.label = _self.title;
        //}
    },
    methods: {
        _onActionEvent: function(event, codeAction) {
            console.log(codeAction);
        }
    }
}
