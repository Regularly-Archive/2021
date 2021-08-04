# FakeRPC

A fake RPC framework but it is real.

# Quick Start

* Define Your Service

```csharp
[FakeRpc]
public class GreetService : IGreetService
{

    public Task<HelloReply> SayHello(HelloRequest request)
    {
        return Task.FromResult(new HelloReply { Message = $"Hello {request.Name}" });
    }
}

public interface IGreetService
{
    Task<HelloReply> SayHello(HelloRequest request);
}
```

Generally, a request and a response like gRPC

* Configure You Startup

```csharp
services.AddControllersWithViews();
// Use Json by default
services.AddFakeRpc();
// Use Protobuf 
services.AddFakeRpc().UseProtobuf();
// Use MessagePack
services.AddFakeRpc().UseMessagePack();
```

* Create Your Client

```csharp
var services = new ServiceCollection();
services.AddFakeRpcClient<IGreetService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000");
});

var serviceProvider = services.BuildServiceProvider();
var clientFactory = serviceProvider.GetService<FakeRpcClientFactory>();
var clientProxy = clientFactory.Create<IGreetService>(new Uri("http://localhost:5000"));
var reply = await clientProxy.SayHello(new HelloRequest() { Name = "张三" });
```

# Benchmark
``` ini
BenchmarkDotNet=v0.13.0, OS=Windows 10.0.17763.2061 (1809/October2018Update/Redstone5)
Intel Core i5-10400 CPU 2.90GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK=5.0.301
  [Host]        : .NET Core 3.1.16 (CoreCLR 4.700.21.26205, CoreFX 4.700.21.26205), X64 RyuJIT
  .NET 5.0      : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT
  .NET Core 3.1 : .NET Core 3.1.16 (CoreCLR 4.700.21.26205, CoreFX 4.700.21.26205), X64 RyuJIT


```
|         Method |           Job |       Runtime |      Mean |     Error |    StdDev |   Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------- |-------------- |-------------- |----------:|----------:|----------:|--------:|------:|------:|----------:|
| RunMessagePack |      .NET 5.0 |      .NET 5.0 |  7.971 ms | 0.1569 ms | 0.3134 ms | 15.6250 |     - |     - |    167 KB |
|    RunProtobuf |      .NET 5.0 |      .NET 5.0 | 12.680 ms | 0.5286 ms | 1.5337 ms |       - |     - |     - |    217 KB |
|        RunJson |      .NET 5.0 |      .NET 5.0 |  8.065 ms | 0.1230 ms | 0.1803 ms | 23.4375 |     - |     - |    176 KB |
| RunMessagePack | .NET Core 3.1 | .NET Core 3.1 |  8.886 ms | 0.2249 ms | 0.6343 ms | 15.6250 |     - |     - |    153 KB |
|    RunProtobuf | .NET Core 3.1 | .NET Core 3.1 | 11.034 ms | 0.2182 ms | 0.4408 ms |       - |     - |     - |    157 KB |
|        RunJson | .NET Core 3.1 | .NET Core 3.1 | 10.582 ms | 0.3087 ms | 0.8905 ms | 15.6250 |     - |     - |    162 KB |



