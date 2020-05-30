{
    data: function () {
        return {
            visible: true
        };
    },
    mounted: function () {
        var _self = this;
        console.log('vue_header_top === ', _self.objUser);
    },
    methods: {
        item_click: function(event, scope___view, str_title) {
            var _self = this;
            $('#' + _self.idvc___ + ' .nav-link').removeClass('active');
            $(event.target).closest('.nav-link').addClass('active');

            //var el;
            //if (event && event.target) el = event.target;

            //$(el.parentElement).children().removeClass('active');

            ////___APP.objViewMain.str_title = str_title;
            ////___APP.objViewMain.objItemSelected.str_title = '';
             
            view___load(scope___view);

            //$(el).addClass('active');
        }
    }
}