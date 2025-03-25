using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chat_Server
{
    internal class Message
    {
        public string? UserName { get; set; }
        public string? MsgText { get; set; }
        public DateTime MsgTime { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Message? FromJson(string jsonText)
        {
            return JsonSerializer.Deserialize<Message>(jsonText);
        }

        public override string ToString() => $"{MsgTime.ToShortTimeString()} | {UserName} : {MsgText}";


    }
}
