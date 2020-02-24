using System;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;

namespace IPC_Sender
{
    public class Server
    {
        
        private static AnonymousPipeServerStream _sender;
        private static AnonymousPipeServerStream _receiver;
        private static Process proc;

        private const string CLIENT_WORKING_DIR = @"C:\Users\Nisar\source\repos\IPC_AnonymousPIPE_CS_PY\CSharp_Client\bin\Publish";
        private const string CLIENT_FILE_NAME = @"CSharp_Client.exe";

        static Server()
        {
            _sender = new AnonymousPipeServerStream(PipeDirection.Out,
            HandleInheritability.Inheritable);

            _receiver = new AnonymousPipeServerStream(PipeDirection.In,
            HandleInheritability.Inheritable);
        }
        
        public static void Start()
        {
            proc = new Process();

            try
            {
                proc.StartInfo.WorkingDirectory = CLIENT_WORKING_DIR;
                proc.StartInfo.FileName = Path.Join(CLIENT_WORKING_DIR,CLIENT_FILE_NAME);
                proc.StartInfo.UseShellExecute = false;

                proc.StartInfo.Arguments =
                    $"{_sender.GetClientHandleAsString()} {_receiver.GetClientHandleAsString()}";

                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;

                proc.Start();

                _sender.DisposeLocalCopyOfClientHandle();
                _receiver.DisposeLocalCopyOfClientHandle();

                using (_sender)
                {
                    using (_receiver)
                    {

                        using (StreamWriter sW = new StreamWriter(_sender))
                        {
                            using (StreamReader sR = new StreamReader(_receiver))
                            {
                                sW.AutoFlush = true;
                                sW.WriteLine("SYNC");
                                _sender.WaitForPipeDrain();

                                for (int i = 0; i < 10; i++)
                                {
                                    sW.WriteLine($"Server message {i}");
                                    Console.WriteLine(proc.StandardOutput.ReadLine());

                                    var incoming = sR.ReadLine();
                                    Console.WriteLine(incoming);
                                }
                            }
                        }
                        
                    }
                }
                
                Console.WriteLine(proc.StandardError.ReadLine());
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                proc.WaitForExit();
                proc.Kill();
                proc.Close();
            }
        }
    }
}
