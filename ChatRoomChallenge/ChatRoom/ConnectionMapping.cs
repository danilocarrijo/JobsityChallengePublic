using System.Collections.Generic;
using System.Linq;

namespace ChatRoomChallenge.ChatRoom
{
    public class ConnectionMapping
    {
        private readonly Dictionary<string, string> _connections =
               new Dictionary<string, string>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public string GetConnection(string key)
        {
            return _connections[key];
        }

        public List<string> GetConnectionUserNames()
        {
            return _connections.Select(x => x.Key).ToList();
        }

        public void AddOrUpdateConnection(string key, string connectionId)
        {
            lock (_connections)
            {
                if (!ContainsConnections(key))
                {
                    _connections.Add(key, connectionId);
                }
                else
                {
                    _connections[key] = connectionId;
                }
            }
        }

        public bool ContainsConnections(string key)
        {
            return _connections.ContainsKey(key);
        }

        public void Remove(string key)
        {
            lock (_connections)
            {
                if (!ContainsConnections(key))
                {
                    return;
                }

                lock (_connections)
                {
                    _connections.Remove(key);
                }
            }
        }
    }
}
