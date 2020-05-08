(function () {
    'use strict';
    try {
        var ___log_text = function (text) { log___.write('[API_NAME]', '_', text); };
        var ___log_key = function (key, text) { log___.write('[API_NAME]', key, text); };

        var ___api_call = function (api_name, paramenter, request, result_type) {
            var v = ___api.js_call('[API_NAME]', api_name, JSON.stringify(paramenter), JSON.stringify(request));
            if (result_type == 'text')
                return v;
            else
                return JSON.parse(v);
        };

        var ___request = [TEXT_REQUEST];
        var ___parameters = [TEXT_PARAMETER];

        //___log_text('This is from JavaScript...');

        [EXECUTE_TEXT_JS]

    } catch (e) {
        return { ok: false, name: '[API_NAME]', error: e.message };
    }
})();