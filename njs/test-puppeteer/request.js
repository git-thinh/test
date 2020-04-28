
const PUPPETEER = require('puppeteer');

const URL_LOGIN = 'http://192.168.10.58/login/index.php';

let BROWSER, PAGE;

(async () => {
    BROWSER = await PUPPETEER.launch({ args: ['--no-sandbox', '--disable-setuid-sandbox'] });
    PAGE = await BROWSER.newPage();

    //[1] go page login
    //await PAGE.goto(URL_LOGIN, { waitUntil: 'domcontentloaded' });
    //await PAGE.setViewport({ width: 1024, height: 800 });
    //await PAGE.screenshot({ fullPage: true, path: FILE_NAME_SCREENSHOT });
    //console.log('Screenshot done ...');

    await PAGE.setRequestInterception(true);
    //PAGE.on('request', request => {
    //    // Override headers
    //    //const headers = Object.assign({}, request.headers(), {
    //    //    foo: 'bar', // set "foo" header
    //    //    origin: undefined, // remove "origin" header
    //    //});
    //    //request.continue({ headers });
    //    var url = request.url();
    //    console.log(url);
    //    var headers = request.headers();
    //    request.continue({ headers })
    //});
    PAGE.on('request', request => {
        // Override headers
        //const headers = Object.assign({}, request.headers(), {
        //    foo: 'bar', // set "foo" header
        //    origin: undefined, // remove "origin" header
        //});
        //request.continue({ headers });
        var url = request.url();
        console.log(url);

        var headers = request.headers();
        request.continue({ headers })
    });
    await PAGE.goto(URL_LOGIN);

    //await BROWSER.close();
    //console.log('> BROWSER CLOSE ...');
})();


//#region [ READLINE ]

const READ_LINE = require("readline");
const RL = READ_LINE.createInterface({ input: process.stdin, output: process.stdout });
RL.on("line", function (line) {
    let text = line.trim();
    const has_pushLog = text.endsWith('?log');
    if (has_pushLog) text = text.substr(0, text.length - 4).trim();
    const a = text.split(' ');
    let cmd = a[0].toLowerCase();
    switch (cmd) {
        case 'exit':
            process.exit();
            break;
        case 'cls':
        case 'clear':
            console.clear();
            break;
        default:
            console.clear();
            break;
    }
});

//#endregion

