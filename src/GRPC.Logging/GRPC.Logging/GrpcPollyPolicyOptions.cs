using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GRPC.Logging
{
    public class GrpcPollyPolicyOptions
    {
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// 重试间隔
        /// </summary>
        public TimeSpan RetryInterval { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// 多少次后熔断
        /// </summary>
        public int CircuitBreakerCount { get; set; }

        /// <summary>
        /// 熔断时间
        /// </summary>
        public TimeSpan CircuitBreakerTime { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// 判定超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.Zero;
    }
}
