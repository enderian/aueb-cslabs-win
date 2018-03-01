using cslabs_win.common.packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace cslabs_win.common
{
    public class Connection
    {
        public Dictionary<string, Packet> KnownPackets = new Dictionary<string, Packet>();
        public Func<Connection, BindPacket> BindHandler { private get; set; }
        public Action<Connection> PostBindHandler { private get; set; } = (_) => { };
        public Action<Exception> ExceptionHandler { private get; set; } = (_) => { };

        private readonly Thread _networkThread;
        private readonly Thread _pingThread;
        private CancellationTokenSource _cts;

        public StreamWriter Writer { get; private set; }

        public string ServerAddress { private get; set; }
        public int ServerPort { private get; set; }

        public Connection()
        {
            _cts = new CancellationTokenSource();
            _networkThread = new Thread(NetworkThread);
            _pingThread = new Thread(PingThread);
        }

        public void Connect()
        {

            _networkThread.Start();
            _pingThread.Start();
        }

        public void WaitConnect()
        {
 
        }

        public void Kill()
        {
            Writer.Close();
            _networkThread.Abort();
            _pingThread.Abort();
        }

        public void SendPacket(Packet packet)
        {
            var arguments = packet.SendPacket(this);
            if (Writer == null) return;
            Writer.WriteLine(packet.Signature);
            foreach (var t in arguments)
            {
                Writer.WriteLine(t);
            }
            Writer.Flush();

            Console.WriteLine(packet.Signature + " packet sent.");
        }

        public void PingThread()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        if (Writer != null)
                        {
                            Writer.WriteLine("Ping");
                            Writer.Flush();
                        }
                    }
                    catch (ThreadAbortException exception)
                    {
                        throw exception;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Network ping exception information: {0}", exception);
                        ExceptionHandler.Invoke(exception);
                        Writer = null;
                    }
                    Thread.Sleep(2500);
                }
            } catch (ThreadAbortException) { }
        }



        public void NetworkThread()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        var client = new TcpClient();
                        client.Connect(ServerAddress, ServerPort);

                        using (var stream = client.GetStream())
                        using (var reader = new StreamReader(stream))
                        using (var writer = new StreamWriter(stream))
                        {
                            Writer = writer;

                            if (BindHandler != null)
                            {
                                var bindPacket = BindHandler.Invoke(this);
                                SendPacket(bindPacket);
                            }
                            else
                            {
                                Console.WriteLine("Skipped binding packet.");
                            }

                            PostBindHandler.Invoke(this);

                            while (!reader.EndOfStream)
                            {
                                var packetName = reader.ReadLine();

                                if (!KnownPackets.ContainsKey(packetName)) continue;
                                var packet = KnownPackets[packetName];
                                var arguments = new string[packet.PacketLength];
                                for (var i = 0; i < arguments.Length; i++)
                                {
                                    arguments[i] = reader.ReadLine();
                                }
                                packet.HandlePacket(arguments, this);
                            }
                            Console.WriteLine("Exited logic.");
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        throw;
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine("Network exception information: {0}", exception);
                        ExceptionHandler.Invoke(exception);
                    }
                    Writer = null;
                    Thread.Sleep(2500);
                }
            }
            catch (ThreadAbortException) { }
        }
    }
}
