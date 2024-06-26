using Grpc.Demo.ProtoLib;
using Microsoft.AspNetCore.Server.Kestrel.Https;

namespace Grpc.Demo.WebServer;

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
                      httpsOptions.ServerCertificate          = CertificateHelper.GetCertificate("0df08ea616f04c3f8eac4f95a69725e0c1d3d4bf");
                      httpsOptions.CheckCertificateRevocation = false;
                  });
              });
          });
    }
}
