using System.Collections.Generic;
using System.Linq;

namespace ApiWithAuthentication.Servers.API.SignalR
{
    public class ConnectionService : IConnectionService
    {
        private readonly Dictionary<string, string> connectionIds = new Dictionary<string, string>();
        private readonly object _lock = new object();

        public void Add(string userId, string connectionId)
        {
            lock (_lock)
            {
                if (!connectionIds.ContainsKey(userId))
                {
                    connectionIds.Add(userId, connectionId);
                }
            }
        }

        public void Remove(string userId)
        {
            lock (_lock)
            {
                if (connectionIds.ContainsKey(userId))
                {
                    connectionIds.Remove(userId);
                }
            }
        }

        public IReadOnlyList<string> GetAllExcept(string userId)
        {
            return connectionIds
                .Where(ci => ci.Key != userId)
                .Select(ci => ci.Value)
                .ToList()
                .AsReadOnly();
        }
    }
}
