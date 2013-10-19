using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO.Ports;
using Toggl;
using Microsoft.VisualBasic;

namespace Arduino_Toggl
{
    class Program
    {
        #region HandlerRoutine

        // Constants which sends HandlerRoutine
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        // Declare SetConsoleCtrlHandler of Win32 API
        [DllImport("Kernel32")]
        static extern bool
            SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // Delegate which sends HanlerRoutine to SetConsoleCtrlHandler function
        delegate bool HandlerRoutine(CtrlTypes CtrlType);

        HandlerRoutine myHandlerOnExit;

        /// <summary>
        /// HandlerRoutine
        /// </summary>
        /// <param name="ctrlType"></param>
        /// <returns></returns>
        bool myHandler(CtrlTypes ctrlType)
        {
            // Close serial port connection
            if (statusChecker.mySerialPort.IsOpen)
            {
                statusChecker.mySerialPort.Close();
            }

            return false;
        }

        #endregion

        #region Private member variables

        StatusChecker statusChecker;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        Program()
        {
            // Set myHandler
            myHandlerOnExit = new HandlerRoutine(myHandler);
            SetConsoleCtrlHandler(myHandler, true);

            statusChecker = new StatusChecker();
        }

        #endregion

        #region Private methods

        void Run()
        {
            // OpenConnection
            try
            {
                Console.WriteLine("Establishing serial port connection...\n");
                statusChecker.mySerialPort.Open();
            }
            catch
            {
                Console.WriteLine("Cannnot establish serial port connection");
                Thread.Sleep(1000);
                return;
            }

            Console.WriteLine("Serial port connection established\n");

            AutoResetEvent autoEvent = new AutoResetEvent(false);

            // Create the delegate that invokes methods for the timer.
            TimerCallback timerDelegate =
                new TimerCallback(statusChecker.RefreshStatus);

            // Start Timer
            Console.WriteLine("{0} Start Timer.\n",
                DateTime.Now.ToString("h:mm:ss.fff"));

            Timer stateTimer = new Timer(timerDelegate, autoEvent, 0, 1000);

            // Unlimited waiting time
            autoEvent.WaitOne(-1, false);

            // Dispose Timer
            stateTimer.Dispose();
        }

        #endregion

        static void Main(string[] args)
        {
            (new Program()).Run();
        }
    }
}
