using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Server
{
    internal interface ICommand
    {
        void Execute();
    }
}
