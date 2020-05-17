using Grpc.Demo.ProtoLib;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;

namespace Grpc.Demo.WebServer
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
      return Host
        .CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
          webBuilder.UseStartup<Startup>();

          webBuilder.ConfigureKestrel(kestrelOptions =>
          {
            kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
            {
              // new X509Certificate2("<ServerCertPath.pfx>", "<Password>");

              httpsOptions.ClientCertificateMode      = ClientCertificateMode.AllowCertificate;
              httpsOptions.ServerCertificate          = CertificateStore.GetCertificate("e57ee1fbef2cee2e87415104fdda177be3124de8");
              httpsOptions.CheckCertificateRevocation = false;
            });
          });
        });
    }
  }
}
