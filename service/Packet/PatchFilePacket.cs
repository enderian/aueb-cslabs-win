using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using cslabs_win.common;
using cslabs_win.common.packet;

namespace cslabs_win.service.packet
{
    public class PatchFilePacket : Packet
    {
        public PatchFilePacket() : base("PatchFile", 2) {}

        public override void HandlePacket(string[] arguments, Connection connection)
        {
            var source = arguments[0];
            var destination = arguments[1];

            if (destination.ToLower() == "self")
            {
                var batchFile = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\update.bat";
                var exeDest = System.Reflection.Assembly.GetEntryAssembly().Location;
                var newExeDest = System.Reflection.Assembly.GetEntryAssembly().Location + ".updated";

                new WebClient().DownloadFile(source, newExeDest);

                using (var batchUpdater = new StreamWriter(batchFile))
                {
                    batchUpdater.WriteLine("@echo off");
                    batchUpdater.WriteLine("echo Updating service...");
                    batchUpdater.WriteLine("timeout 3 > NUL");
                    batchUpdater.WriteLine("xcopy /y \"" + newExeDest + "\" \"" + exeDest + "\" ");
                    batchUpdater.WriteLine("del /F /Q \"" + newExeDest + "\"");
                    batchUpdater.WriteLine("shutdown /r /t 60 /f");
                    batchUpdater.WriteLine("del \"%~f0\"");
                    batchUpdater.Flush();
                }

                Process.Start(batchFile);
                Environment.Exit(0);
            } else if (source.ToLower() == "delete")
            {
                File.Delete(destination);
            }
            else
            {
                new WebClient().DownloadFileAsync(new Uri(source), destination);
            }
        }

        public override string[] SendPacket(Connection connection)
        {
            throw new NotImplementedException();
        }
    }
}
