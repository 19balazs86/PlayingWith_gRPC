using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using GreeterService;
using Grpc.Core;
using Grpc.Net.Client;

namespace Grpc.Demo.ConsoleClient
{
  public static class Program
  {
    private static Random _random = new Random();

    public static async Task Main(string[] args)
    {
      var client = createClient();

      HelloRequest request = new HelloRequest { Name = "John Doe" };

      // Call: SayHello
      HelloReply reply = await client.SayHelloAsync(request);

      Console.WriteLine($"{reply.Message} at {reply.Timestamp.ToDateTime().ToLocalTime()}");

      // Call: SayHelloToNobody
      reply = await client.SayHelloToNobodyAsync(new Empty());

      Console.WriteLine(reply.Message);

      await callServerStreaming(client, request);
    }

    private static async Task callServerStreaming(Greeter.GreeterClient client, HelloRequest request)
    {
      var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromMilliseconds(_random.Next(2000, 3000)));

      using var serverStreamingCall = client.SayHelloServerStreaming(request, cancellationToken: cts.Token);

      try
      {
        await foreach (HelloReply reply in serverStreamingCall.ResponseStream.ReadAllAsync())
          Console.WriteLine($"{reply.Message} at {reply.Timestamp.ToDateTime().ToLocalTime()}");
      }
      catch (IOException ex)
      {
        Console.WriteLine($"IOException: {ex.Message}"); // The request was aborted
      }
    }

    private static Greeter.GreeterClient createClient()
    {
      GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");

      return new Greeter.GreeterClient(channel);
    }
  }
}
