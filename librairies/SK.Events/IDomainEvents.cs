using System.Threading.Tasks;

namespace SK.Events
{
    public interface IDomainEvents
    {
        Task RaiseAsync<T>(T args) where T : IEvent;
    }
}
