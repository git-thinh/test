﻿
using System.Diagnostics;
using System.Linq;

namespace Sider.Benchmark
{
  public class MultiplePipelinedResultTypeJob : Job
  {
    private string _numKey;
    private string _strKey;


    public override string Description
    {
      get { return "Pipeline with multiple return types."; }
    }

    public MultiplePipelinedResultTypeJob()
    {
      _numKey = "mprtj:NUM" + InstanceNumber.ToString();
      _strKey = "mprtj:HELLO" + InstanceNumber.ToString();
    }


    public override void BeforeBenchmark()
    {
      Debug.WriteLine(InstanceNumber.ToString() + ": BeforeBenchmark");
    }

    public override void Run(int iterations)
    {
      Debug.WriteLine(InstanceNumber.ToString() + ": RunOneIteration");

      var result = Client.Pipeline(c =>
      {
        for (var i = 0; i < iterations; i++)
          c.Set(_strKey + i.ToString(), "WORLD" + i.ToString());

        c.Set(_numKey, "0");
        for (var i = 1; i < iterations; i++)
          c.Incr(_numKey);

        for (var i = 0; i < iterations; i++)
          c.Get(_strKey + i.ToString());
      });

      var arr = result.ToArray();
      for (var i = 0; i < iterations; i++) // verify 100 SETs
        Assert(arr[i] is bool && (bool)arr[i]);

      Assert(arr[iterations] is bool && (bool)arr[iterations]); // verify num=0 SET
      for (var i = 1; i < iterations; i++) // verify INCRs
        Assert(arr[i + iterations] is long && (long)arr[i + iterations] == i);

      for (var i = 0; i < iterations; i++) // verify GETs
        Assert(arr[i + 2 * iterations] is string &&
          (string)arr[i + 2 * iterations] == "WORLD" + i.ToString());
    }

    public override void AfterBenchmark()
    {
      var keys = Client
        .Pipeline(c =>
        {
          c.Keys(_numKey + "*");
          c.Keys(_strKey + "*");
        })
        .Cast<string[]>()
        .SelectMany(s => s)
        .ToArray();

      Client.Del(keys);
    }
  }
}
