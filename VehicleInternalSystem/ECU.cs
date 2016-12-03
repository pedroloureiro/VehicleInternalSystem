using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehicleInternalSystem
{
    public class ECU
    {

        private TcpListener ecuSocket;
        private static IPAddress ipAddress;

        private TcpClient bcuSocket;
        private static BinaryReader bcuReader;
        private static BinaryWriter bcuWriter;

        private TcpClient tcuSocket;
        private static BinaryReader tcuReader;
        private static BinaryWriter tcuWriter;

        public ECU() { }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ECUForm(this));
        }

        public string InitECU()
        {
            ipAddress = IPAddress.Parse("127.0.0.1");
            ecuSocket = new TcpListener(ipAddress, 500);
            ecuSocket.Start();

            return ipAddress.ToString();
        }

        public string HandleConnection()
        {
   
                TcpClient tempSocket = ecuSocket.AcceptTcpClient();
                BinaryWriter writer = new BinaryWriter(tempSocket.GetStream());
                BinaryReader reader = new BinaryReader(tempSocket.GetStream());

                writer.Write("ID? ECU");
                string response = reader.ReadString();
                switch (response)
                {
                    case "BCU":
                        bcuSocket = tempSocket;
                        bcuReader = reader;
                        bcuWriter = writer;
                        bcuWriter.Write("OK");
                        return "CONNECTED TO BCU\n";

                    case "TCU":
                        tcuSocket = tempSocket;
                        tcuReader = reader;
                        tcuWriter = writer;
                        tcuWriter.Write("OK");
                    return "CONNECTED TO TCU";
            }

                return (response + " tried to connect, but was denied\n");
        }

        public string ListenTCU()
        {
            string response = tcuReader.ReadString();
            return response;
        }

        public string BrakeCmd()
        {
            string response;
            try
            {
                bcuWriter.Write("BRAKE");
                response = bcuReader.ReadString();
                return response;
            }
            catch (Exception)
            {
                response = "BCU not connected";
                return response;
            }
        }
    }
}
