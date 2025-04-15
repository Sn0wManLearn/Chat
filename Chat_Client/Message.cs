using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Chat_Client
{
    internal class Message
    {
        public string FromName { get; set; }
        public string ToName { get; set; }
        public string MsgText { get; set; }
        public DateTime MsgTime { get; set; }

        public Message() { }
        public Message(string fromName, string toName, string msgText)
        {
            FromName = fromName;
            ToName = toName;
            MsgText = msgText;
            MsgTime = DateTime.Now;
        }

        public Message(string fromName, string toName, string msgText, DateTime time)
        {
            FromName = fromName;
            ToName = toName;
            MsgText = msgText;
            MsgTime = time;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public static Message? FromJson(string jsonText)
        {
            return JsonSerializer.Deserialize<Message>(jsonText);
        }
        public override string ToString() => $"{MsgTime.ToShortTimeString()} | {FromName} : {MsgText}";

    }
}
