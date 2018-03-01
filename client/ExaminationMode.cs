using commons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cslabs_win.client
{
    public class ExaminationMode
    {

        private static readonly List<string> KnownBrowsers = new List<string>()
        {
            "firefox",
            "chrome",
            "chromium",
            "iexplore",
            "launcher",
            "microsoftedge"
        };

        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { get; set; }

        [JsonProperty(PropertyName = "allow_ui")]
        public bool AllowUI { get; set; }
        [JsonProperty(PropertyName = "allow_browsers")]
        public bool AllowBrowsers { get; set; }

        [JsonProperty(PropertyName = "browser_mode")]
        public bool BrowserMode { get; set; }

        [JsonProperty(PropertyName = "browser_url")]
        public string BrowserURL { get; set; }
        [JsonProperty(PropertyName = "browser_allowed_domains")]
        public string[] BrowserAllowedDomains { get; set; }
        [JsonProperty(PropertyName = "browser_popups")]
        public bool BrowserPopups { get; set; }

        [JsonProperty(PropertyName = "allowed_processes")]
        public string[] AllowedProcesses { get; set; }
        [JsonProperty(PropertyName = "blacklisted_processes")]
        public string[] BlacklistedProcesses { get; set; }

        [JsonIgnore]
        private Thread _logicThread;
        [JsonIgnore]
        private List<int> _knownProcesses;

        public void Start()
        {
            _logicThread = new Thread(LogicThread)
            {
                IsBackground = true
            };
            _logicThread.Start();
        }

        public void LogicThread()
        {
            _knownProcesses = new List<int>
            {
                Process.GetCurrentProcess().Id
            };
            foreach (var process in Process.GetProcesses())
            {
                _knownProcesses.Add(process.Id);
            }

            try
            {
                while (true)
                {
                    Thread.Sleep(100);

                    try
                    {
                        if (!Enabled)
                        {
                            Thread.Sleep(1000);
                            break;
                        }

                        var allProcceses = Process.GetProcesses();
                        var sessionId = WindowsAPI.WTSGetActiveConsoleSessionId();

                        foreach (var process in allProcceses)
                        {
                            if (!AllowUI && process.ProcessName.ToLower() == "explorer")
                            {
                                process.Kill();
                            }
                            else if (!AllowBrowsers && KnownBrowsers.Contains(process.ProcessName.ToLower()))
                            {
                                process.Kill();
                                NotifyIllegalApp("Internet browsers are not permitted.");
                            }
                            else if (process.ProcessName.ToLower() == "taskmgr")
                            {
                                process.Kill();
                                NotifyIllegalApp("Task Manager is not permitted.");
                            }
                            else if (AllowedProcesses != null && AllowedProcesses.Length > 0 && !_knownProcesses.Contains(process.Id) && !AllowedProcesses.Contains(process.ProcessName.ToLower()))
                            {
                                process.Kill();
                                NotifyIllegalApp(process.ProcessName + " is not permitted.");
                            }
                            else if (BlacklistedProcesses != null && BlacklistedProcesses.Contains(process.ProcessName.ToLower()))
                            {
                                process.Kill();
                                NotifyIllegalApp(process.ProcessName + " is not permitted.");
                            }
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(@"Examination mode exception: {0}", exception);
                    }
                }
            }
            catch (ThreadAbortException) {}
        }

        private void NotifyIllegalApp(string message)
        {
            new Thread(() => {
                WindowsAPI.MessageBox(new IntPtr(0), message + "\nContact an administrator if you believe this is wrong.", "", (uint)0x00000030L);
            }).Start();
        }
    }
}
