using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using Toggl;
using Microsoft.VisualBasic;

namespace Arduino_Toggl
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            StatusChecker statusChecker = new StatusChecker();

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
    }
}
