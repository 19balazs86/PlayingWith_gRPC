using System;
using GreeterService;

namespace Grpc.Demo.ProtoLib
{
  public static class Extensions
  {
    public static void WriteToConsole(this HelloReply reply)
    {
      Console.WriteLine($"{reply.Message} at {reply.Timestamp.ToDateTime().ToLocalTime()}");
    }
  }
}
