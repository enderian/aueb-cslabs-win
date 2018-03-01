using System;
using commons;
using cslabs_win.common;
using cslabs_win.common.packet;
using cslabs_win.service.packet;
using System.Diagnostics;
using System.ServiceProcess;

namespace cslabs_win.service
{
    public sealed partial class LabService : ServiceBase
    {

        public Connection Connection;

        public LabService()
        {
            InitializeComponent();
            CanHandleSessionChangeEvent = true;
            CanPauseAndContinue = false;

            EventLog.Source = "aueb-cslabs-service";
            EventLog.Log = "Application";
        }

        protected override void OnStart(string[] args)
        {
            StartService();
        }

        protected override void OnStop()
        {
            StopService();
        }

        public void StartService()
        {
            Connection = new Connection
            {
                ServerAddress = Properties.Settings.Default.ServerAddress,
                ServerPort = Properties.Settings.Default.ServerPort,
                BindHandler = (_) => new BindPacket("Service"),
                PostBindHandler = (_) =>
                {
                    if (WindowsAPI.WTSGetActiveConsoleSessionId() == 0) return;
                    var packet =
                        new SessionEventPacket((int) WindowsAPI.WTSGetActiveConsoleSessionId(), "SessionLogon");
                    Connection.SendPacket(packet);
                },
                ExceptionHandler = (exception) =>
                {
                    EventLog.WriteEntry(
                        "Networking exception occured: " + exception.GetType().ToString() + "\n" +
                        exception.Message, EventLogEntryType.Error);
                },
                KnownPackets =
                {
                    ["Action"] = new ActionPacket(),
                    ["Execute"] = new ExecutePacket(),
                    ["PatchFile"] = new PatchFilePacket()
                }
            };
            Connection.Connect();
        }

        public void StopService()
        {
            Connection.Kill();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription) { 
            switch(changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                case SessionChangeReason.SessionLogoff:
                case SessionChangeReason.SessionLock:
                case SessionChangeReason.SessionUnlock:
                    {
                        var packet = new SessionEventPacket(changeDescription);
                        Connection.SendPacket(packet);
                        break;
                    }
                case SessionChangeReason.ConsoleConnect:
                    break;
                case SessionChangeReason.ConsoleDisconnect:
                    break;
                case SessionChangeReason.RemoteConnect:
                    break;
                case SessionChangeReason.RemoteDisconnect:
                    break;
                case SessionChangeReason.SessionRemoteControl:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
