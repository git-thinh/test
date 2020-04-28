const app = require('express')();
const server = require('http').Server(app);
const io = require('socket.io')(server);



const PUPPETEER = require('puppeteer');
let BROWSER, PAGE;

async function ___download(url, callback) {
    BROWSER = await PUPPETEER.launch({ args: ['--no-sandbox', '--disable-setuid-sandbox'] });
    PAGE = await BROWSER.newPage();

    let s = url;
    await PAGE.setRequestInterception(true);
    PAGE.on('request', request => {
        var url_ = request.url(); 
        s = s + '^' + url_;
        var headers = request.headers();
        request.continue({ headers })
    });

    await PAGE.goto(url, { waitUntil: 'domcontentloaded' });
    await PAGE.setViewport({ width: 1024, height: 800 });
    //await PAGE.screenshot({ fullPage: true, path: '1.login.png' });

    await BROWSER.close();

    if (callback) callback(s);
};

server.listen(process.env.PORT); 

app.get('/njs/io', (req, res) => {
    res.sendFile(__dirname + '/io.html');
});

app.get('/njs/api/download', async function (req, res) {
    let url = req.query.url;
    await ___download(url, async (s) => {
        res.type('text/plain');
        res.send(s);
    });
});

app.get('/njs/api/link-1', function (req, res) {
    res.send('link-1: Hello from foo! [express sample]');
});

app.get('/njs/api/link-2', function (req, res) {
    res.send('link-2: Hello from bar! [express sample]');
});

io.on('connection', (socket) => {
    socket.emit('messages', { hello: 'world, Mr Thinh' });
});