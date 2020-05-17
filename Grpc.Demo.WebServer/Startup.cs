using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Grpc.Demo.WebServer
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthorization();

      services
        .AddAuthentication()
        .AddCertificate(configureCertificateOptions);

      services.AddGrpc();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseDeveloperExceptionPage();

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGrpcService<Services.GreeterService>();

        endpoints.MapGet("/", async context => await context.Response.WriteAsync("Hello gRPC server."));
      });
    }

    private static void configureCertificateOptions(CertificateAuthenticationOptions options)
    {
      // Not recommended in production. The example is using a self-signed test certificate.
      options.AllowedCertificateTypes = CertificateTypes.SelfSigned;
      options.RevocationMode          = X509RevocationMode.NoCheck;
      //options.Events                  = new CertificateAuthenticationEvents()
      //{
      //  OnCertificateValidated = ctx =>
      //  {
      //    // Write additional validation.
      //    ctx.Success();
      //    return Task.CompletedTask;
      //  }
      //};
    }
  }
}
