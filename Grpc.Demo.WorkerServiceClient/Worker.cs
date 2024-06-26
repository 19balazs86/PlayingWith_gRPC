using GreeterService;

namespace Grpc.Demo.WorkerServiceClient;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    private readonly Greeter.GreeterClient _greeterClient;

    public Worker(ILogger<Worker> logger, Greeter.GreeterClient greeterClient)
    {
        _logger = logger;

        _greeterClient = greeterClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var request = new HelloRequest { Name = "Worker Service" };

        while (!stoppingToken.IsCancellationRequested)
        {
            HelloReply reply = await _greeterClient.SayHelloAsync(request);

            _logger.LogInformation(reply.ToString());

            await Task.Delay(1000, stoppingToken);
        }
    }
}
