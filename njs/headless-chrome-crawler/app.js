var spawn = require('child_process').spawn;
var options = {};

var url = 'https://www.chotot.com/ha-noi/quan-bac-tu-liem/danh-sach-nguoi-tim-viec/70026333.htm';

var child = spawn('phantomjs.exe', ['./js-01.js', url], options);

var text = '';
child.stdout.on('data', function (data) {
    var s = data.toString('utf8');
    //console.log('DATA = ', s);
    text += s;
});

child.stderr.on('data', function (err) {
    var s = err.toString('utf8');
    console.log('ERROR = ', s);
});

child.on('close', function (code) {
    require('fs').writeFile('log.txt', text, 'utf8', function (err) {
        if (err) return console.log(err);
        console.log('DONE');
    });
});