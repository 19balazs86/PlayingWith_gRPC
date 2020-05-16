using System;
using System.Threading;
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

      return Task.FromResult(createHelloReply($"Hello {request.Name}."));
    }

    public override Task<HelloReply> SayHelloToNobody(Empty request, ServerCallContext context)
    {
      _logger.LogInformation("SayHello to Nobody.");

      return Task.FromResult(createHelloReply("Hello Nobody."));
    }

    public override async Task SayHelloServerStreaming(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
      int i = 0;

      CancellationToken cancelToken = context.CancellationToken;

      while (!cancelToken.IsCancellationRequested && i < 5)
      {
        string message = $"Hello {request.Name}? {++i}";

        _logger.LogInformation($"Sending greeting '{message}'.");

        await responseStream.WriteAsync(createHelloReply(message));

        await Task.Delay(500);
      }

      _logger.LogInformation($"Request cancelled by the client ?= {cancelToken.IsCancellationRequested}.");
    }

    private static HelloReply createHelloReply(string message)
      => new HelloReply { Message = message, Status = ReplyStatusEnum.Success, Timestamp = Timestamp.FromDateTime(DateTime.UtcNow) };
  }
}
