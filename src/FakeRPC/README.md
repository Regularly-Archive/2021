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
services.AddFakeRpc().UseMessagePack();
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


