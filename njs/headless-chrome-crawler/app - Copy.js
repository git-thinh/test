const _KUE = require('kue');
const _QUEUE = _KUE.createQueue({
    prefix: 'kue',
    redis: {
        port: 11111,
        host: '127.0.0.1',
        db: 3
    }
});

const _PHANTOM = require('phantom');
let _BROWSER;

(async function () {
    _BROWSER = await _PHANTOM.create();
    //await _BROWSER.exit();
}());

async function ___download(url, callback) {
    let s = '';

    try {
        const u = new URL(url);
        let host = u.host;
        if (host.startsWith('www.')) host = host.substr(4);

        //await PAGE.setRequestInterception(true);
        //PAGE.on('request', request => {
        //    var url_ = request.url();
        //    if (url_ == url || (url_.indexOf('.js') > 0 && url_.indexOf(host) > 0)) {
        //        console.log('OK: ' + url_);
        //        request.continue();
        //    } else {
        //        console.log('NO: ' + url_);
        //        request.abort();
        //    }
        //});


        const page = await _BROWSER.createPage();
        await page.on("onResourceRequested", function (requestInfo, request) {            
            var url_ = requestInfo.url;
            if (url_ == url || (url_.indexOf('.js') > 0 && url_.indexOf(host) > 0)) {
                console.log('-> ' + url_);
            } else {
                console.log('NO: ' + url_);
                request.abort();
            }
        });
        await page.on("onLoadFinished", function (p) {
            console.info('?????????????????? onLoadFinished === ', p);
        });

        const status = await page.open(url);
        console.log(status);

        s = await page.property('content');
        console.log(s);

    } catch (e) { ; }

    if (callback) callback(s);
}


_QUEUE.process('JOB_CRAWLER', function (job, done) {
    const url = job.data.url;
    console.log('JOB_CRAWLER = ', url);
    ___download(url, (s) => {
        console.log('JOB_CRAWLER ok = ', s);
    });
    done();
});

function isValidHttpUrl(url_) {
    let url;
    try {
        url = new URL(url_);
    } catch (_) {
        return false;
    }
    return url.protocol === "http:" || url.protocol === "https:";
}

require('http').createServer(async function (req, res) {
    const a = req.url.split('?');
    res.writeHead(200, { 'Content-Type': 'text/plain' });
    if (a.length > 1 && a[1].length > 0) {
        let url = a[1].trim();
        if (url.length > 4) url = url.substr(4).trim();
        if (isValidHttpUrl(url)) {
            _QUEUE.create('JOB_CRAWLER', { url: url }).save();
            res.end('URL: ' + url);
            return;
        }
    }
    res.end('');
}).listen(process.env.PORT || 9090);