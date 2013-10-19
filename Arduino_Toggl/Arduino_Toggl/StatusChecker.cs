using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

using Arduino_Toggl.Properties;
using Toggl;

namespace Arduino_Toggl
{
    class StatusChecker
    {
        #region Private member variables

        private DateTime _refreshTime;
        private bool _isNoTimeEntry;

        #endregion

        #region Public member variables

        public SerialPort mySerialPort { get; private set; }

        /// <summary>
        /// Time entry desscription
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Time entry EntryTime
        /// </summary>
        public DateTime EntryTime { get; private set; }


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public StatusChecker()
        {
            Description = string.Empty;
            EntryTime = DateTime.Now;
            
            // Set Serial Port
            mySerialPort = new SerialPort(Settings.Default.SerialPort);
            mySerialPort.BaudRate = 9600;

            GetTimeEntry();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Refreshing status
        /// This method is called by the timer delegate.
        /// </summary>
        /// <param name="stateInfo"></param>
        public void RefreshStatus(Object stateInfo)
        {
            // Notifies a waiting thread that an event has occurred
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

#if DEBUG
            Console.WriteLine("{0} Checking status {0}.",
                DateTime.Now.ToString("h:mm:ss.fff"));
#endif
            // Refresh per minuite
            TimeSpan span = new TimeSpan(0, 0, 1, 0, 0);

            // Set time difference
            TimeSpan duration = DateTime.Now - EntryTime;

            // Call API
            if (DateTime.Now - _refreshTime > span)
            {
                GetTimeEntry();
            }

            // Send data
            mySerialPort.Write(Description.PadLeft(Settings.Default.MaxCharacterPerLine) + duration.ToString(@"hh\:mm\:ss").PadLeft(Settings.Default.MaxCharacterPerLine));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Get Time Entry from Toggl.com
        /// </summary>
        private void GetTimeEntry()
        {
            try
            {
                // Get Time Entries in the last 8 hours
                var rte = new Toggl.QueryObjects.TimeEntryParams();
                rte.StartDate = DateTime.Now.AddHours(-8);
                rte.EndDate = DateTime.Now;

                Toggl.Services.TimeEntryService timeEntrySrv = new Toggl.Services.TimeEntryService(Settings.Default.ApiKey);
                var entries = timeEntrySrv.List(rte);

                // Check current Time Entry
                if (entries.Count() > 0)
                {
                    TimeEntry currentEntry = entries.Last();
                    if (currentEntry.Duration < 0)
                    {
                        Description = ConvertString(currentEntry.Description);
                        EntryTime = Convert.ToDateTime(currentEntry.Start);
                        _isNoTimeEntry = false;
                    }
                    else if (!_isNoTimeEntry)
                    {
                        Description = "No Time Entry";
                        EntryTime = DateTime.Now;
                        _isNoTimeEntry = true;
                    }
                }
                else if (!_isNoTimeEntry)
                {
                    Description = "No Time Entry";
                    EntryTime = DateTime.Now;
                }

                _refreshTime = DateTime.Now;
            }
            catch
            {
                Console.WriteLine("Failed to get time entry");
            }
        }

        /// <summary>
        /// Conver string for LCD
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ConvertString(string str)
        {
            string returnStr = str;

            // Convert double byte character to single character
            returnStr = Strings.StrConv(returnStr, VbStrConv.Narrow, 0x0411);

            // Substring for LCD
            if (returnStr.Length > Settings.Default.MaxCharacterPerLine)
            {
                returnStr = returnStr.Substring(0, Settings.Default.MaxCharacterPerLine);
            }

            return returnStr;
        }

        #endregion
    }
}
