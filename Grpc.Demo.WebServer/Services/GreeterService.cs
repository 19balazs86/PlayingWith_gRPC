using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using GreeterService;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
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

      return Task.FromResult(HelloReply.Create($"Hello {request.Name}."));

      // return Task.FromResult((HelloReply)null);
      // Client side: RpcException: 'Status(StatusCode=Cancelled, Detail="No message returned from method.")'
    }

    public override Task<Empty> ThrowRpcException(Empty request, ServerCallContext context)
    {
      var metadata = new Metadata { { "TestKey", "TestValue" } };

      var status = new Status(StatusCode.Cancelled, "Fake error happened.");

      throw new RpcException(status, metadata);
    }

    public override async Task SayHelloServerStreaming(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
      int i = 0;

      CancellationToken cancelToken = context.CancellationToken;

      while (!cancelToken.IsCancellationRequested && i < 5)
      {
        string message = $"Hello {request.Name}? {++i}";

        _logger.LogInformation($"Sending greeting '{message}'.");

        await responseStream.WriteAsync(HelloReply.Create(message));

        await Task.Delay(500);
      }

      _logger.LogInformation($"Request cancelled by the client ?= {cancelToken.IsCancellationRequested}.");
    }

    public override async Task<HelloReply> SayHelloClientStreaming(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
    {
      var names = new List<string>();

      try
      {
        await foreach (HelloRequest request in requestStream.ReadAllAsync())
        {
          _logger.LogInformation($"Incoming name: {request.Name}");

          names.Add(request.Name);
        }

        _logger.LogInformation("End of client streaming.");

        return HelloReply.Create($"Hi all: {string.Join(" and ", names)}.");
      }
      catch (IOException ex)
      {
        _logger.LogError(ex.Message); // The request stream was aborted.

        return new HelloReply();
      }
    }

    [Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]
    public override Task<HelloReply> SayHelloCertAuth(HelloRequest request, ServerCallContext context)
    {
      _logger.LogInformation($"{request.Name} is authenticated.");

      return Task.FromResult(HelloReply.Create($"Hello authenticated {request.Name}."));
    }
  }
}
