using System;
using System.IO;
using System.IO.Pipes;

namespace CSharp_Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("C# client started");
            Client.Run(args[0], args[1]);
        }
    }
}
