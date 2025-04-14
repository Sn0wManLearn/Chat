using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Server
{
    internal class PrototypePattern
    {
        public string ConstantText = " С наилучшими пожеланиями";
        public Message Message { get; set; }

        public PrototypePattern(Message message)
        {
            Message = message;
        }

        public object Clone()
        {
            Message.MsgText += ConstantText;
            return Message as object;

        }
    }
}
