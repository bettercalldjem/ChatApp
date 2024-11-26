using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace ChatApp
{
    public class DatabaseService
    {
        private SQLiteConnection _connection;

        public DatabaseService(string dbPath)
        {
            _connection = new SQLiteConnection(dbPath);
            _connection.CreateTable<Message>();
        }

        public void SaveMessage(Message message)
        {
            _connection.Insert(message);
        }

        public List<Message> GetMessages()
        {
            return _connection.Table<Message>().OrderBy(m => m.Timestamp).ToList();
        }
    }
}
