using cslabs_win.common.packet;
using pGina.Shared.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace cslabs_win.pgina
{
    public class PGinaConnection
    {
        public string ServerAddress { private get; set; }
        public int ServerPort { private get; set; }

        public BooleanResult SendCredentials(UserInformation userInfo)
        {
            var success = 0;
            var extra = "Unable to communicate with the CSLabs server.";

            try
            {
                var attempts = 5;
                while (attempts-- > 0)
                {
                    try
                    {
                        var client = new TcpClient();
                        client.Connect(ServerAddress, ServerPort);

                        using (var stream = client.GetStream())
                        using (var reader = new StreamReader(stream))
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.WriteLine("LoginRequest");
                            writer.WriteLine(Environment.MachineName);
                            writer.WriteLine(userInfo.Username);
                            writer.WriteLine(userInfo.Password);
                            writer.Flush();

                            while (!reader.EndOfStream)
                            {
                                var packetName = reader.ReadLine();
                                if (packetName != "LoginResponse") continue;

                                success = int.Parse(reader.ReadLine() ?? throw new InvalidOperationException());
                                reader.ReadLine();
                                extra = reader.ReadLine();

                                if (success == 1 && extra != "")
                                {
                                    userInfo.Fullname = extra;
                                }
                                break;
                            }
                            Console.WriteLine("Exited logic.");
                        }
                        break;
                    }
                    catch (ThreadAbortException exception)
                    {
                        throw exception;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Network PGina exception information: {0}", exception);

                    }
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException) { }
            return new BooleanResult()
            {
                Success = success == 1,
                Message = extra,
            };
        }
    }
}
