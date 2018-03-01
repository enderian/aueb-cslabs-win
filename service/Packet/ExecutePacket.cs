using cslabs_win.common.packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cslabs_win.common;
using System.Diagnostics;

namespace cslabs_win.service.packet
{
    class ExecutePacket : Packet
    {
        public ExecutePacket() : base("Execute", 2) { }

        public override void HandlePacket(string[] arguments, Connection connection)
        {
            string application = arguments[0];
            string args = arguments[1];

            Process.Start(application, args);
        }

        public override string[] SendPacket(Connection connection)
        {
            throw new NotImplementedException();
        }
    }
}
