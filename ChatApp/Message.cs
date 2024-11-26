using SQLite;
using System;

namespace ChatApp
{
    public class Message
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
