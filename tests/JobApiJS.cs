using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace tests
{ 
     public class JobApiJS : IJob
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