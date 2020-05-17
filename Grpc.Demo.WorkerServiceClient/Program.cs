using System;
using GreeterService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Grpc.Demo.WorkerServiceClient
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
      return Host
        .CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
          services.AddHostedService<Worker>();

          services.AddGrpcClient<Greeter.GreeterClient>(options => options.Address = new Uri("https://localhost:5001"));
        });
    }
  }
}
