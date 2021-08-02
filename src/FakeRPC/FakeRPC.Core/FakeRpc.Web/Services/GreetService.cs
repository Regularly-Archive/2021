using FakeRpc.Core;
using MessagePack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeRpc.Web.Services
{
    /// <summary>
    /// GreetService
    /// </summary>
    [FakeRpc]
    public class GreetService : IGreetService
    {
        private readonly ILogger<GreetService> _logger;
        public GreetService(ILogger<GreetService> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// SayHello
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<HelloReply> SayHello(HelloRequest request)
        {
            _logger.LogInformation($"Invoke SayHello() with \"{JsonConvert.SerializeObject(request)}\"...");
            return Task.FromResult(new HelloReply { Message = $"Hello {request.Name}" });
        }

        /// <summary>
        /// SayWho
        /// </summary>
        /// <returns></returns>
        public Task<HelloReply> SayWho()
        {
            _logger.LogInformation("Invoke SayWho() with \"{ }\"...");
            return Task.FromResult(new HelloReply { Message = $"I'm 长安书小妆" });
        }
    }

    /// <summary>
    /// IGreetService
    /// </summary>
    public interface IGreetService
    {
        /// <summary>
        /// SayHello
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<HelloReply> SayHello(HelloRequest request);

        /// <summary>
        /// SayWho
        /// </summary>
        /// <returns></returns>
        Task<HelloReply> SayWho();
    }

    /// <summary>
    /// HelloReply
    /// </summary>
    [Serializable]
    [ProtoContract]
    [MessagePackObject]
    public class HelloReply
    {
        /// <summary>
        /// Message
        /// </summary>
        [Key(0)]
        [ProtoMember(1)]
        public string Message { get; set; }
    }

    /// <summary>
    /// HelloRequest
    /// </summary>
    [Serializable]
    [ProtoContract]
    [MessagePackObject]
    public class HelloRequest
    {
        /// <summary>
        /// Name
        /// </summary>
        [Key(0)]
        [ProtoMember(1)]
        public string Name { get; set; }
    }
}
