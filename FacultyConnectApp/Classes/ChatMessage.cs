using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacultyConnectApp.Models
{
    public class ChatMessage
    {
        public string MessageId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public string Timestamp { get; set; }

        public ChatMessage()
        {
            // Default constructor for Firebase
        }

        public ChatMessage(string content, string senderName)
        {
            MessageId = $"{DateTime.Now.Ticks}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            Content = content;
            SenderName = senderName;
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public override string ToString()
        {
            return $"{SenderName}: {Content}";
        }
    }
}
