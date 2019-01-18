using System;
namespace BotExample.Model
{
    public class ChatMessage
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public bool IsIncoming { get; set; }
    }
}
