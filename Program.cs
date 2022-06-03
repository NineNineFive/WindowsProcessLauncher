using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// TO COMPILE:
// dotnet build -o windowsProcessLauncher --configuration Release

namespace CMD_script_tests
{
    internal class Program
    {
        static bool exitSystem = false;
        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private static Process process = new Process();
        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");

            process.Kill(true);
            //allow main to run off
            exitSystem = true;
            process.WaitForExit();

            //shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion

        static void Main(string[] args)
        {

            // Some boilerplate to react to close window event, CTRL-C, kill, etc
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            Console.WriteLine("Starting " + args[0]);
            // Configure the process using the StartInfo properties

            process.StartInfo.FileName = args[0];
            
            //optional arguments
            if (args.Length > 1)
            {
                String arguments="";
                foreach (var argument in args)
                {
                    arguments+=" "+argument;
                }
                Console.WriteLine("Arguments: " + arguments);
                process.StartInfo.Arguments =arguments;
            }
            process.EnableRaisingEvents = true;
            
            

            process.Start();
            process.WaitForExit();

        }
    }
}
