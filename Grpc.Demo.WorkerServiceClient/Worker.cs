using System.Threading;
using System.Threading.Tasks;
using GreeterService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Grpc.Demo.WorkerServiceClient
{
  public class Worker : BackgroundService
  {
    private readonly ILogger<Worker> _logger;
    private readonly Greeter.GreeterClient _greeterClient;

    private static readonly HelloRequest _request = new HelloRequest { Name = "Worker Service" };

    public Worker(ILogger<Worker> logger, Greeter.GreeterClient greeterClient)
    {
      _logger        = logger;
      _greeterClient = greeterClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        HelloReply reply = await _greeterClient.SayHelloAsync(_request);

        _logger.LogInformation(reply.ToString());

        await Task.Delay(1000, stoppingToken);
      }
    }
  }
}
