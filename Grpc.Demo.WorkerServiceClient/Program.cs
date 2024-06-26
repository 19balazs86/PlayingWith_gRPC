using GreeterService;

namespace Grpc.Demo.WorkerServiceClient;

public static class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        var services = builder.Services;

        // Add services to the container
        {
            services.AddHostedService<Worker>();

            services.AddGrpcClient<Greeter.GreeterClient>(options => options.Address = new Uri("https://localhost:5001"));
        }

        builder.Build().Run();
    }
}
