using Quartz;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace tests
{
    public class JobCrawler : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var para = context.getParaInput();

            //var api = para.getValue<string>("api___");
            //if (string.IsNullOrWhiteSpace(api)) { 

            //}

            context.log("key_name_1", "This is content of JOB. It executing...");
            await Task.FromResult(false);
        }
    }
}