using cslabs_win.common;
using cslabs_win.common.packet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cslabs_win.client.packet
{
    class ClientInitializePacket : Packet
    {

        private MiniWindow miniWindow;

        public ClientInitializePacket(MiniWindow miniWindow) : base("ClientInitialize", 3)
        {
            this.miniWindow = miniWindow;
        }

        public override void HandlePacket(string[] arguments, Connection connection)
        {
            string username = arguments[0];
            string fullName = arguments[1];
            ExaminationMode examination = JsonConvert.DeserializeObject<ExaminationMode>(arguments[2]);

            Thread thread = new Thread(() =>
            {
                miniWindow.UpdateWindow(username, fullName, examination);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public override string[] SendPacket(Connection connection)
        {
            throw new NotImplementedException();
        }
    }
}
