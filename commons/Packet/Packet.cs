using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslabs_win.common.packet
{
    public abstract class Packet
    {

        public string Signature { get; private set; }
        public int PacketLength { get; private set; }

        public Packet(string signature, int lentgh)
        {
            Signature = signature;
            PacketLength = lentgh;
        }

        public abstract void HandlePacket(string[] arguments, Connection connection);

        public abstract string[] SendPacket(Connection connection);

    }
}
