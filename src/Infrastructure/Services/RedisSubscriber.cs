using Application.Interfaces;
using Domain.Attributes;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services
{
    [DI(typeof(IRedisSubscriber))]
    public class RedisSubscriber(IConnectionMultiplexer redis) : IRedisSubscriber
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer = redis;

        public virtual async Task Subscribe<T>(string channel, Action<T> handler)
        {            
            var subscriber = _connectionMultiplexer.GetSubscriber();

            await subscriber.SubscribeAsync(new RedisChannel(channel, RedisChannel.PatternMode.Literal), (redisChannel, message) =>
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    var deselizedMessage = JsonSerializer.Deserialize<T>(message!);

                    if (deselizedMessage != null) 
                    {
                        handler(deselizedMessage);
                    }
                }
            });            
        }
    }
}
