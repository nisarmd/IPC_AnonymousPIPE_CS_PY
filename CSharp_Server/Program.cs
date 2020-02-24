using System;


namespace IPC_Sender
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Server] : Starting C# Server");
            Server.Start();
        }
    }
}
