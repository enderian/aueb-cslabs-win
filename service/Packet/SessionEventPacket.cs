using commons;
using cslabs_win.common;
using cslabs_win.common.packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace cslabs_win.service.packet
{
    public class SessionEventPacket : Packet
    {

        private int sessionId;
        private string reason;

        public SessionEventPacket(SessionChangeDescription description) : this(description.SessionId, description.Reason.ToString()) { }

        public SessionEventPacket(int v1, string v2) : base("SessionEvent", 3)
        {
            this.sessionId = v1;
            this.reason = v2;
        }

        public override void HandlePacket(string[] arguments, Connection connection)
        {
            throw new NotImplementedException();
        }

        public override string[] SendPacket(Connection connection)
        {
            string[] arguments = new string[3];

            arguments[0] = reason;
            arguments[1] = sessionId.ToString();
            arguments[2] = WindowsAPI.GetUsername(sessionId);

            return arguments;
        }
    }
}
