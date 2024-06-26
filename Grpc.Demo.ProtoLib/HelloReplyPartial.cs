using Google.Protobuf.WellKnownTypes;

namespace GreeterService;

public sealed partial class HelloReply
{
    public static HelloReply Create(string message, ReplyStatusEnum status = ReplyStatusEnum.Success)
    {
        return new HelloReply { Message = message, Status = status, Timestamp = Timestamp.FromDateTime(DateTime.UtcNow) };
    }


    public void WriteToConsole()
    {
        Console.WriteLine($"{Message} at {Timestamp.ToDateTime().ToLocalTime()}");
    }
}
