using System.Threading.Tasks;

namespace SK.Events
{
    public interface IEventHandler<T> where T : IEvent
    {
        Task HandleAsync(T args);
    }
}
