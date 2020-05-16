using System;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using GreeterService;
using Grpc.Net.Client;

namespace Grpc.Demo.ConsoleClient
{
  public static class Program
  {
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
    }

    private static Greeter.GreeterClient createClient()
    {
      GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");

      return new Greeter.GreeterClient(channel);
    }
  }
}
