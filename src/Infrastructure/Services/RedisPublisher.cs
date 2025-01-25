using Application.Interfaces;
using Domain.Attributes;
using Domain.Events;
using StackExchange.Redis;
using System.Text.Json;

namespace Infrastructure.Services
{
    [DI(typeof(IRedisPublisher))]
    public class RedisPublisher(IConnectionMultiplexer redis) : IRedisPublisher
    {
        private readonly IConnectionMultiplexer _redis = redis;

        public virtual async Task PublishAsync<T>(Channel channel, T message)
        {
            var database = _redis.GetSubscriber();
            var serializedMessage = JsonSerializer.Serialize(message);
            await database.PublishAsync(new(Enum.GetName(channel)!, RedisChannel.PatternMode.Literal), serializedMessage); 
        }
    }
}
