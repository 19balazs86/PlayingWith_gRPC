using Grpc.Demo.ProtoLib;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;

namespace Grpc.Demo.WebServer;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(configureKestrel);

        var services = builder.Services;

        // Add services to the container
        {
            services
              .AddAuthentication()
              .AddCertificate(configureCertificate);

            services.AddAuthorization();

            services.AddGrpc();

            services.AddGrpcReflection(); // Enables it to test in Postman. See in README
        }

        WebApplication app = builder.Build();

        // Configure the request pipeline
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGrpcService<Services.GreeterService>();

            app.MapGrpcReflectionService();

            app.MapGet("/", () => "Hello gRPC server.");
        }

        app.Run();
    }

    private static void configureKestrel(KestrelServerOptions kestrelOptions)
    {
        // Configure this for Certificate authentication
        kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
        {
            // new X509Certificate2("<ServerCertPath.pfx>", "<Password>");

            httpsOptions.ClientCertificateMode      = ClientCertificateMode.AllowCertificate;
            httpsOptions.ServerCertificate          = CertificateHelper.GetCertificate("0df08ea616f04c3f8eac4f95a69725e0c1d3d4bf");
            httpsOptions.CheckCertificateRevocation = false;
        });
    }

    private static void configureCertificate(CertificateAuthenticationOptions options)
    {
        // We need to use ConfigureKestrel with SelfSigned certificates, because the following 'ChainCertificate' is not working
        // options.AdditionalChainCertificates.Add(CertificateHelper.GetCertificate("0df08ea616f04c3f8eac4f95a69725e0c1d3d4bf"));

        // Not recommended in production. The example is using a self-signed test certificate.
        options.AllowedCertificateTypes   = CertificateTypes.SelfSigned;
        options.RevocationMode            = X509RevocationMode.NoCheck;
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
