using System;
using System.ServiceProcess;
using cslabs_win.service.packet;

namespace cslabs_win.service
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            if (Environment.UserInteractive)
            {
                //string[] arguments = { "https://raw.githubusercontent.com/enderian/aueb-cslabs-go/master/start.sh", "self" };
                //new PatchFilePacket().HandlePacket(arguments, null);

                var service = new LabService();
                service.StartService();
            }
            else
            {
                var servicesToRun = new ServiceBase[]
                {
                    new LabService()
                };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
