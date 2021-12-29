using Confluent.Kafka;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Kafka.Learning.Consumer
{
    static class ConsumerExtensions
    {
        public static void Subscribe<TKey, TValue>(
            this IConsumer<TKey, TValue> consumer,
            string topic,
            CancellationToken cancellationToken,
            Action<TValue> callback)
        {
            consumer.Subscribe(topic);

            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult != null)
                    {
                        var headers = consumeResult.Message.Headers;
                        if (headers != null && headers.TryGetLastBytes("Authorization", out byte[] values))
                        {
                            var jwtToken = Encoding.UTF8.GetString(values).Replace("Bearer", "").Trim();
                            var userInfo = new JwtTokenResloverService().ValidateToken(jwtToken);
                            UserContext.SetUserInfo(userInfo);
                        }

                        if (callback != null)
                            callback(consumeResult.Message.Value);
                    }
                }
                catch (ConsumeException e)
                {
                    // ...
                }
            }
        }
    }
}
