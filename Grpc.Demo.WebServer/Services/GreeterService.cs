using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using GreeterService;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Grpc.Demo.WebServer.Services
{
  public class GreeterService : Greeter.GreeterBase
  {
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
      _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
      _logger.LogInformation($"SayHello to {request.Name}.");

      return Task.FromResult(new HelloReply { Message = $"Hello {request.Name}.", Status = ReplyStatusEnum.Success });
    }

    public override Task<HelloReply> SayHelloToNobody(Empty request, ServerCallContext context)
    {
      _logger.LogInformation("SayHello to Nobody.");

      return Task.FromResult(new HelloReply { Message = "Hello Nobody.", Status = ReplyStatusEnum.Success });
    }
  }
}
