using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslabs_win.common.packet
{
    public class BindPacket: Packet 
    {

        private readonly string _type;

        public BindPacket(string type) : base("Bind", 2) {
            _type = type;
        }

        public override void HandlePacket(string[] arguments, Connection connection)
        {
            throw new NotImplementedException();
        }

        public override string[] SendPacket(Connection connection)
        {
            var arguments = new string[2];
            arguments[0] = _type;
            arguments[1] = Environment.MachineName;
            return arguments;
        }
    }
}
