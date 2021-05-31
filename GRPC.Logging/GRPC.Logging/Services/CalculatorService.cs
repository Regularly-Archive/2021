using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;

namespace GRPC.Logging
{
    public class CalculatorService : Calculator.CalculatorBase
    {
        public override Task<CalculatorReply> Calc(CalculatorRequest request, ServerCallContext context)
        {
            var exp = string.Empty;
            var result = 0;
            switch (request.Op)
            {
                case "+":
                    exp = $"{request.Num1} + {request.Num2}";
                    result = request.Num1 + request.Num2;
                    break;
                case "-":
                    exp = $"{request.Num1} - {request.Num2}";
                    result = request.Num1 - request.Num2;
                    break;
                case "*":
                    exp = $"{request.Num1} * {request.Num2}";
                    result = request.Num1 * request.Num2;
                    break;
                case "/":
                    exp = $"{request.Num1} / {request.Num2}";
                    result = request.Num1 / request.Num2;
                    break;
            }

            return Task.FromResult(new CalculatorReply()
            {
                Exp = exp,
                Result = result

            });
        }
    }
}
