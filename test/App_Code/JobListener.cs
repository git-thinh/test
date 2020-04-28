using Quartz;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace test
{
    public class JobListener : IJobListener
    {
        public string Name { get { return "clsJobListener"; } }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            if (dataMap.ContainsKey("CURRENT_ID___"))
            {
                long CURRENT_ID___ = long.Parse(DateTime.Now.ToString("yyMMddHHmmss"));
                dataMap.Put("CURRENT_ID___", CURRENT_ID___);

                if (dataMap.ContainsKey("COUNTER___"))
                {
                    var state = (ConcurrentDictionary<long, bool>)dataMap["COUNTER___"];
                    state.TryAdd(CURRENT_ID___, false);
                }
            }
            return Task.FromResult(false);
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            if (dataMap.ContainsKey("CURRENT_ID___"))
            {
                long CURRENT_ID___ = dataMap.GetLong("CURRENT_ID___");
                if (dataMap.ContainsKey("COUNTER___"))
                {
                    var state = (ConcurrentDictionary<long, bool>)dataMap["COUNTER___"];
                    state[CURRENT_ID___] = true;
                }
            }
            return Task.FromResult(false);
        }
    }
}