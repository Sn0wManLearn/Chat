using Microsoft.VisualStudio.TestPlatform.TestHost;
using static System.Net.Mime.MediaTypeNames;
using Chat_Client;

namespace ChatTests
{
    [TestClass]
    public class ClientTests
    {
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void RunWithoutArgument()
        {
            Chat_Client.Program.Main(new string[] { });
        }
    }
}
