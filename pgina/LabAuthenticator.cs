using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pGina.Shared.Types;
using cslabs_win.common;

namespace cslabs_win.pgina
{
    public class LabAuthenticator : pGina.Shared.Interfaces.IPluginAuthentication
    {
        public string Name => "CSLabs PGina";

        public string Description => "Provides authentication through the CSLabs Go server.";

        public string Version => "0.1";

        public Guid Uuid => Guid.Parse("{3f7965e5-fe54-4956-9a92-7fcdbe145efd}");

        public BooleanResult AuthenticateUser(SessionProperties properties)
        {
            var userInfo = properties.GetTrackedSingle<UserInformation>();
            var connection = new PGinaConnection()
            {
                ServerAddress = Properties.Settings.Default.ServerAddress,
                ServerPort = Properties.Settings.Default.ServerPort,
            };
            return connection.SendCredentials(userInfo);
        }

        public void Starting()
        {
            
        }

        public void Stopping()
        {
            
        }
    }
}
