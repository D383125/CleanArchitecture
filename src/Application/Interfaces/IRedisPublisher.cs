using Domain.Events;

namespace Application.Interfaces
{
    public interface IRedisPublisher
    {
        Task PublishAsync<T>(Channel channel, T message);
    }
}
