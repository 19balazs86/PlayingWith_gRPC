# Playing with .NET Core gRPC
Try out the .NET Core client and server gRPC Remote Procedure Call (RPC) framework. This can be a good fit for high-performance or streaming communication between microservices.

#### In this example

`Grpc.Demo.ProtoLib`

- Contains the proto file in a common library for the client and server projects
- CertificateStore.cs: It is used to read the certificate from the Current User Store

`Grpc.Demo.WebServer`

- gRPC server implementation (unary call, throw RpcException, server and client streaming, define enum and timestamp types)
- Implement *Certificate Authentication*. You can use the *MakeCerts.ps1* to create development client a server certificates

`Grpc.Demo.ConsoleClient`

- gRPC client implementation for console application

`Grpc.Demo.WorkerServiceClient`

- gRPC client implementation for *worker service* using *gRPC client factory*

#### Resources

- [Introduction to gRPC on .NET Core](https://docs.microsoft.com/en-us/aspnet/core/grpc) *(Microsoft Docs)*
- [gRPC for .NET Examples](https://github.com/grpc/grpc-dotnet/tree/master/examples#grpc-for-net-examples) *(GitHub)*
- [.NET Core ❤ gRPC](https://grpc.io/blog/grpc-on-dotnetcore/) *(grpc.io)*
- [Protobuf scalar data types](https://docs.microsoft.com/en-us/dotnet/architecture/grpc-for-wcf-developers/protobuf-data-types) *(Microsoft Docs)*
---
- [Using gRPC in ASP.NET Core](https://app.pluralsight.com/library/courses/aspnet-core-grpc/table-of-contents) *(Pluralsight - Shawn Wildermuth, 2h 32min)*
- [Talking between services with gRPC](https://www.youtube.com/watch?v=W-bULzA0ki8) *(NET Core Summer Event - Marc Gravell, 50min)*
- [gRPC for ASP.NET Core](https://www.youtube.com/watch?v=JpM95-Wplzo) *(NDC Conferences - James Newton-King, 1h)*

---

- [Message validation with FluentValidation](https://anthonygiretti.com/2020/05/18/grpc-asp-net-core-3-1-model-validation) *(Anthony Giretti)*