using FakeRpc.Core;
using MessagePack;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakeRpc.Web.Services
{
    [FakeRpc]
    public class CalculatorService : ICalculatorService
    {
        public Task<CalculatorReply> Calculate(CalculatorRequest request)
        {
            var exp = string.Empty;
            var result = 0M;
            switch (request.Op)
            {
                case "+":
                    exp = $"{request.Num1} + {request.Num2} =";
                    result = request.Num1 + request.Num2;
                    break;
                case "-":
                    exp = $"{request.Num1} - {request.Num2} =";
                    result = request.Num1 - request.Num2;
                    break;
                case "*":
                    exp = $"{request.Num1} * {request.Num2} =";
                    result = request.Num1 * request.Num2;
                    break;
                case "/":
                    exp = $"{request.Num1} / {request.Num2} = ";
                    result = request.Num1 / request.Num2;
                    break;
            }

            return Task.FromResult(new CalculatorReply() { Expression = exp, Result = result });
        }

        public Task<CalculatorReply> Random()
        {
            var operators = new string[] { "+", "-", "*", "/" };
            var random = new Random();
            var num1 = random.Next(0, 100);
            var num2 = random.Next(0, 100);
            var op = operators[random.Next(operators.Length)];
            return Calculate(new CalculatorRequest() { Num1 = num1, Num2 = num2, Op = op });
        }
    }

    [Serializable]
    [ProtoContract]
    [MessagePackObject]
    public class CalculatorReply
    {
        [Key(0)]
        [ProtoMember(1)]
        public string Expression { get; set; }
        [Key(1)]
        [ProtoMember(2)]
        public decimal Result { get; set; }
    }

    [Serializable]
    [ProtoContract]
    [MessagePackObject]
    public class CalculatorRequest
    {
        [Key(0)]
        [ProtoMember(1)]
        public string Op { get; set; }
        [Key(1)]
        [ProtoMember(2)]
        public decimal Num1 { get; set; }
        [Key(2)]
        [ProtoMember(3)]
        public decimal Num2 { get; set; }
    }

    public interface ICalculatorService
    {
        Task<CalculatorReply> Calculate(CalculatorRequest request);
        Task<CalculatorReply> Random();
    }
}
