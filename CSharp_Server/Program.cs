using System;
using System.Threading;

namespace IPC_Sender
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Server] : Starting C# Server");

            Server.Start(Runtime.Python);
            Server.Start(Runtime.CSharp);

            Console.WriteLine("[Server] : Exiting C# Server");
        }
    }
}
