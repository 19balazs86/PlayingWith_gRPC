using Google.Protobuf.WellKnownTypes;
using GreeterService;
using Grpc.Core;
using Grpc.Demo.ProtoLib;
using Grpc.Net.Client;

namespace Grpc.Demo.ConsoleClient;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var client = createClient();

        var request = new HelloRequest { Name = "John Doe" };

        // Call: SayHello
        Console.WriteLine("--> Call: SayHello");

        HelloReply reply = await client.SayHelloAsync(request);

        reply.WriteToConsole();

        // Call: ReceiveNotification
        var changeNotification = new ChangeNotification { Id = 1, Stock = new Stock { Symbol = "StockSymbol" } };

        Console.WriteLine("--> Call: ReceiveNotification");
        await client.ReceiveNotificationAsync(changeNotification);

        // Call: ThrowRpcException
        Console.WriteLine("--> Call: ThrowRpcException");

        try
        {
            await client.ThrowRpcExceptionAsync(new Empty());
        }
        catch (RpcException ex)
        {
            Console.WriteLine("I expected an RpcException. Status: '{0}'", ex.Status);
        }

        // Call: ServerStreaming
        Console.WriteLine("--> Call: ServerStreaming");

        await callServerStreaming(client, request);

        // Call: ClientStreaming
        Console.WriteLine("--> Call: ClientStreaming");

        await callClientStreaming(client);

        // Call: SayHelloCertAuth
        Console.WriteLine("--> Call: SayHelloCertAuth");

        reply = await client.SayHelloCertAuthAsync(request);
        // RpcException: Status(StatusCode=PermissionDenied, Detail="Bad gRPC response. HTTP status code: 403")

        reply.WriteToConsole();
    }

    private static async Task callServerStreaming(Greeter.GreeterClient client, HelloRequest request)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(Random.Shared.Next(2_000, 4_000)));

        using var serverStreamingCall = client.SayHelloServerStreaming(request, cancellationToken: cts.Token);

        try
        {
            await foreach (HelloReply reply in serverStreamingCall.ResponseStream.ReadAllAsync())
            {
                reply.WriteToConsole();
            }
        }
        catch (RpcException ex)
        {
            Console.WriteLine("I expected an RpcException. Status: '{0}'", ex.Status); // The request was aborted
        }
    }

    private static async Task callClientStreaming(Greeter.GreeterClient client)
    {
        using var clientStreamingCall = client.SayHelloClientStreaming();

        for (int i = 0; i < 5; i++)
        {
            string name = $"John #{i}";

            Console.WriteLine($"Sending name: {name}");

            try
            {
                await clientStreamingCall.RequestStream.WriteAsync(new HelloRequest { Name = name });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"InvalidOperationException: {ex.Message}");
                // Can't write the message because the call is complete.
                return;
            }

            await Task.Delay(500);
        }

        await clientStreamingCall.RequestStream.CompleteAsync();

        HelloReply reply = await clientStreamingCall;

        Console.Write("ClientStreaming response: ");

        reply.WriteToConsole();
    }

    private static Greeter.GreeterClient createClient()
    {
        var channelOptions = new GrpcChannelOptions { HttpClient = createHttpClient() };

        GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001", channelOptions);

        return new Greeter.GreeterClient(channel);
    }

    private static HttpClient createHttpClient()
    {
        // new X509Certificate2("<ClientCertPath.pfx>", "<Password>");

        var handler = new HttpClientHandler();

        handler.ClientCertificates.Add(CertificateHelper.GetCertificate("da6ed664e5920ab2192a1993a04bf0d7a5d8f6e9"));

        return new HttpClient(handler);
    }
}
