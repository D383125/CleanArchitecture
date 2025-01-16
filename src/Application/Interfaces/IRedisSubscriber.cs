namespace Application.Interfaces
{
    public interface IRedisSubscriber
    {
        Task Subscribe<T>(string channel, Action<T> handler);
    }
}
