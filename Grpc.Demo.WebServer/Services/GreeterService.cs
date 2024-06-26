using Google.Protobuf.WellKnownTypes;
using GreeterService;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;

namespace Grpc.Demo.WebServer.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        _logger.LogInformation("SayHello to '{Name}'.", request.Name);

        return Task.FromResult(HelloReply.Create($"Hello {request.Name}."));

        // return Task.FromResult((HelloReply)null);
        // Client side: RpcException: 'Status(StatusCode=Cancelled, Detail="No message returned from method.")'
    }

    public override Task<Empty> ThrowRpcException(Empty request, ServerCallContext context)
    {
        var metadata = new Metadata { { "TestKey", "TestValue" } };

        var status = new Status(StatusCode.Internal, "Fake error happened.");

        throw new RpcException(status, metadata);
    }

    public override async Task SayHelloServerStreaming(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        int i = 0;

        CancellationToken cancelToken = context.CancellationToken;

        while (!cancelToken.IsCancellationRequested && i < 5)
        {
            string message = $"Hello {request.Name} #{++i}";

            _logger.LogInformation("Sending greeting '{Message}'.", message);

            await responseStream.WriteAsync(HelloReply.Create(message));
            // await responseStream.WriteAllAsync([HelloReply.Create(message)]); // You can send a list of messages

            await Task.Delay(500);
        }

        _logger.LogInformation("Request cancelled by the client ?= {YesOrNo}.", cancelToken.IsCancellationRequested);
    }

    public override async Task<HelloReply> SayHelloClientStreaming(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
    {
        var names = new List<string>();

        try
        {
            await foreach (HelloRequest request in requestStream.ReadAllAsync())
            {
                _logger.LogInformation("Incoming name: '{Name}'", request.Name);

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
        _logger.LogInformation("{Name} is authenticated.", request.Name);

        return Task.FromResult(HelloReply.Create($"Hello authenticated {request.Name}."));
    }

    public override Task<Empty> ReceiveNotification(ChangeNotification request, ServerCallContext context)
    {
        string oneOfText = request.InstrumentCase switch
        {
            ChangeNotification.InstrumentOneofCase.Stock    => $"OneOf: {request.Stock}",
            ChangeNotification.InstrumentOneofCase.Currency => $"OneOf: {request.Currency}",
            _                                               => "OneOf: None"
        };

        _logger.LogInformation("{OneOfText}", oneOfText);

        return Task.FromResult(new Empty());
    }
}
