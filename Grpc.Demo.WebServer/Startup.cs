using Microsoft.AspNetCore.Authentication.Certificate;
using System.Security.Cryptography.X509Certificates;

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

            services.AddGrpcReflection(); // Enables it to test in Postman. See in README
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

                endpoints.MapGrpcReflectionService();

                endpoints.MapGet("/", async context => await context.Response.WriteAsync("Hello gRPC server."));
            });
        }

        private static void configureCertificateOptions(CertificateAuthenticationOptions options)
        {
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
}
