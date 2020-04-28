const app = require('express')();
const server = require('http').Server(app);
const io = require('socket.io')(server);

server.listen(process.env.PORT); 

app.get('/njs/io', (req, res) => {
    res.sendFile(__dirname + '/io.html');
});

app.get('/njs/api/link-1', function (req, res) {
    res.send('link-1: Hello from foo! [express sample]');
});

app.get('/njs/api/link-2', function (req, res) {
    res.send('link-2: Hello from bar! [express sample]');
});

io.on('connection', (socket) => {
    socket.emit('news', { hello: 'world, Mr Thinh' });
    socket.on('my other event', (data) => {
        console.log(data);
    });
});