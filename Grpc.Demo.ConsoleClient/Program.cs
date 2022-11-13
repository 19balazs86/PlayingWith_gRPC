using Google.Protobuf.WellKnownTypes;
using GreeterService;
using Grpc.Core;
using Grpc.Demo.ProtoLib;
using Grpc.Net.Client;

namespace Grpc.Demo.ConsoleClient
{
    public static class Program
    {
        private static readonly Random _random = new Random();

        public static async Task Main(string[] args)
        {
            var client = createClient();

            HelloRequest request = new HelloRequest { Name = "John Doe" };

            // Call: SayHello
            HelloReply reply = await client.SayHelloAsync(request);

            reply.WriteToConsole();

            // Call: ThrowRpcException
            try
            {
                await client.ThrowRpcExceptionAsync(new Empty());
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status);
            }

            // Call: ServerStreaming
            await callServerStreaming(client, request);

            // Call: ClientStreaming
            await callClientStreaming(client);

            // Call: SayHelloCertAuth
            reply = await client.SayHelloCertAuthAsync(request);
            // RpcException: Status(StatusCode=PermissionDenied, Detail="Bad gRPC response. HTTP status code: 403")

            reply.WriteToConsole();
        }

        private static async Task callServerStreaming(Greeter.GreeterClient client, HelloRequest request)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromMilliseconds(_random.Next(2000, 3000)));

            using var serverStreamingCall = client.SayHelloServerStreaming(request, cancellationToken: cts.Token);

            try
            {
                await foreach (HelloReply reply in serverStreamingCall.ResponseStream.ReadAllAsync())
                    reply.WriteToConsole();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IOException: {ex.Message}"); // The request was aborted
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

            handler.ClientCertificates.Add(CertificateStore.GetCertificate("5cce32c0a54d65693f7b97312d33d9f0a6601d33"));

            return new HttpClient(handler);
        }
    }
}
