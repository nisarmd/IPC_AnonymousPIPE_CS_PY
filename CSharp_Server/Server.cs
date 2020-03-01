using System;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IPC_Sender
{
    public enum Runtime
    {
        Python, CSharp
    }
    public static class Server
    {
        
        private static readonly AnonymousPipeServerStream _sender;
        private static readonly AnonymousPipeServerStream _receiver;
        //static Object obj = new Object();

        public static readonly StreamWriter sW;
        public static readonly StreamReader sR;

        private const string CSHARP_CLIENT_WORKING_DIR = @"C:\Users\Nisar\source\repos\IPC_AnonymousPIPE_CS_PY\CSharp_Client\bin\Publish";
        private const string CSHARP_CLIENT_FILE_NAME = @"CSharp_Client.exe";

        private const string PYTHON_INTERPRETER_WORKING_DIR = @"C:\Users\Nisar\AppData\Local\Programs\Python\Python36-32\";
        private const string PYTHON_INTERPRETER = @"python";
        private const string PYTHON_SCRIPT = @"C:\Users\Nisar\source\repos\IPC_AnonymousPIPE_CS_PY\Python_Client\pyClient.py";

        static Server()
        {
            _sender = new AnonymousPipeServerStream(PipeDirection.Out,
            HandleInheritability.Inheritable);

            _receiver = new AnonymousPipeServerStream(PipeDirection.In,
            HandleInheritability.Inheritable);

            sW = new StreamWriter(_sender);
            sW.AutoFlush = true;
            sR = new StreamReader(_receiver);
        }
        
        
        public static void Start(Runtime runtime)
        {
            Process proc = null;
            try
            {
                proc = GetProcess(runtime);

                //_sender.DisposeLocalCopyOfClientHandle();
                //_receiver.DisposeLocalCopyOfClientHandle();
                Console.WriteLine();
                Console.WriteLine($">>Starting IPC between C# Server and {runtime.ToString()} client");
                sW.WriteLine("SYNC");

                for (int i = 0; i < 10; i++)
                {
                    //
                    sW.WriteLine($"Server message {i}");
                    Console.WriteLine($"C# Server sent msg {i} to {runtime.ToString()}");

                    var incoming = sR.ReadLine();
                    Console.WriteLine(incoming);
                        
                }
                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                proc.Kill();
                proc.Close();
            }
        }
        private static Process GetProcess(Runtime runtime)
        {
            Process proc = null;
            try
            {
                proc = new Process();
                proc.StartInfo.WorkingDirectory = runtime.Equals(Runtime.CSharp) ?
                        CSHARP_CLIENT_WORKING_DIR : PYTHON_INTERPRETER_WORKING_DIR;
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
            }
            catch(Exception e)
            {
                proc.Kill();
                proc.Close();
                throw e;
            }
            return proc;
        }
        private static void OutputResponse(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Output@ "+DateTime.Now+"   "+e.Data);
        }
        private static void ErrorResponse(object sender, DataReceivedEventArgs args)
        {
            Console.WriteLine("Errors@ "+DateTime.Now + "   "+args.Data);
        }
    }
}
