using cslabs_win.common;
using cslabs_win.common.packet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslabs_win.service.packet
{
    class ActionPacket : Packet
    {
        public ActionPacket() : base("Action", 2) { }

        public override void HandlePacket(string[] arguments, Connection connection)
        {
            string action = arguments[0];
            int delay = int.Parse(arguments[1]);

            switch(action)
            {
                case "shutdown_hybrid":
                    {
                        Process.Start("shutdown", "/s /hybrid /t " + delay + " /f");
                        break;
                    }
                case "shutdown":
                    {
                        Process.Start("shutdown", "/s /t " + delay + " /f");
                        break;
                    }
                case "reboot":
                    {
                        Process.Start("shutdown", "/s /t " + delay + " /f");
                        break;
                    }
                case "logout":
                    {
                        Process.Start("shutdown", "/l /f");
                        break;
                    }
            }
        }

        public override string[] SendPacket(Connection connection)
        {
            throw new NotImplementedException();
        }
    }
}
