using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Pipes;

namespace CSharp_Client
{
    public static class Client
    {
        private static AnonymousPipeClientStream _sender;
        private static AnonymousPipeClientStream _receiver;

        public static void Run(string senderHandle, string receiverHandle)
        {
            try
            {
                using (_sender = new AnonymousPipeClientStream(PipeDirection.Out, receiverHandle))
                {
                    using (_receiver = new AnonymousPipeClientStream(PipeDirection.In, senderHandle))
                    {

                        using (StreamReader sR = new StreamReader(_receiver))
                        {
                            using (StreamWriter sW = new StreamWriter(_sender))
                            {
                                string temp;
                                string incoming;
                                int count = 1;

                                sW.AutoFlush = true;
                                // Wait for 'sync message' from the server.
                                do
                                {
                                    Console.WriteLine("[CS CLIENT]:: Wait for sync...");
                                    temp = sR.ReadLine();
                                }
                                while (!temp.StartsWith("SYNC"));


                                while ((incoming = sR.ReadLine()) != null)
                                {
                                    Console.WriteLine($"[CS CLIENT]:: {incoming}");

                                    sW.WriteLine($"Hello from C# Client {count}");
                                    count++;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
