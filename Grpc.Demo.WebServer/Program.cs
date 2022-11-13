using Grpc.Demo.ProtoLib;
using Microsoft.AspNetCore.Server.Kestrel.Https;

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
                          httpsOptions.ServerCertificate          = CertificateStore.GetCertificate("2e2175c8ec45708d4c3ff37e815bf884c8f7c930");
                          httpsOptions.CheckCertificateRevocation = false;
                      });
                  });
              });
        }
    }
}
