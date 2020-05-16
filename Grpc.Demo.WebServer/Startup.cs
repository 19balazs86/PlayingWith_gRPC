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
      services.AddGrpc();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseDeveloperExceptionPage();

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGrpcService<Services.GreeterService>();

        endpoints.MapGet("/", async context => await context.Response.WriteAsync("Hello gRPC server."));
      });
    }
  }
}
