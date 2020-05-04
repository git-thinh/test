
___log_text('Thinh LOG');

var para = {
    connect_string: 'server=192.168.10.54;database=Test;UId=sa; Password=dev@123;Connection Timeout=180',
    script_command: 'select * from [Test].[TestSchema].[Employees]'
};



var rs = ___api_call('db_execute', para, {});

