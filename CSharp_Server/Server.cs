using System;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;

namespace IPC_Sender
{
    public enum Runtime
    {
        Python,CSharp
    }
    public class Server
    {
        
        private static AnonymousPipeServerStream _sender;
        private static AnonymousPipeServerStream _receiver;
        private static Process proc;

        private const string CSHARP_CLIENT_WORKING_DIR = @"C:\Users\Nisar\source\repos\IPC_AnonymousPIPE_CS_PY\CSharp_Client\bin\Publish";
        private const string CSHARP_CLIENT_FILE_NAME = @"CSharp_Client.exe";

        private const string PYTHON_INTERPRETER_WORKING_DIR = @"C:\Users\Nisar\AppData\Local\Programs\Python\Python36-32\";
        private const string PYTHON_INTERPRETER = @"python";
        private const string PYTHON_SCRIPT = @"C:\Users\Nisar\source\repos\IPC_AnonymousPIPE_CS_PY\Python_Client\Python_Client.py";

        static Server()
        {
            _sender = new AnonymousPipeServerStream(PipeDirection.Out,
            HandleInheritability.Inheritable);

            _receiver = new AnonymousPipeServerStream(PipeDirection.In,
            HandleInheritability.Inheritable);
        }
        
        
        public static void Start(Runtime runtime)
        {
            proc = new Process();

            try
            {
                proc.StartInfo.WorkingDirectory = runtime.Equals(Runtime.CSharp)?
                    CSHARP_CLIENT_WORKING_DIR: PYTHON_INTERPRETER_WORKING_DIR;
                proc.StartInfo.FileName = runtime.Equals(Runtime.CSharp) ? 
                    Path.Join(CSHARP_CLIENT_WORKING_DIR, CSHARP_CLIENT_FILE_NAME) :
                    Path.Join(PYTHON_INTERPRETER_WORKING_DIR, PYTHON_INTERPRETER);
                proc.StartInfo.UseShellExecute = false;

                proc.StartInfo.Arguments = runtime.Equals(Runtime.CSharp) ?
                    $"{_sender.GetClientHandleAsString()} {_receiver.GetClientHandleAsString()}" :
                    $"{PYTHON_SCRIPT} {_sender.GetClientHandleAsString()} {_receiver.GetClientHandleAsString()}";

                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.OutputDataReceived += new DataReceivedEventHandler(OutputResponse);
                proc.ErrorDataReceived += new DataReceivedEventHandler(ErrorResponse);
                proc.Start();
                
                _sender.DisposeLocalCopyOfClientHandle();
                _receiver.DisposeLocalCopyOfClientHandle();

                //while (true){
                //    Console.WriteLine(proc.StandardOutput.ReadLine());
                //}
                

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
                                int count = 1;

                                while (count < 20)
                                {
                                    Console.WriteLine(proc.StandardOutput.ReadLine());
                                } 
                                for (int i = 0; i < 10; i++)
                                {
                                    //
                                    sW.WriteLine($"Server message {i}");
                                    
                                    var incoming = sR.ReadLine();
                                    Console.WriteLine(incoming);
                                }
                            }
                        }
                        
                    }
                }
                
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

        static void OutputResponse(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Output@ "+DateTime.Now+"   "+e.Data);
        }
        static void ErrorResponse(object sender, DataReceivedEventArgs args)
        {
            Console.WriteLine("Errors@ "+DateTime.Now + "   "+args.Data);
        }
    }
}
