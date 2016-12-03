using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehicleInternalSystem
{

    public class TCU
    {
        private TcpClient tcuSocket;
        private static IPAddress ipAddress;

        private static BinaryReader reader;
        private static BinaryWriter writer;

        public TCU() { }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TCUForm(this));
        }

        public bool Connect()
        {
            tcuSocket = new TcpClient();
            ipAddress = IPAddress.Parse("127.0.0.1");

            try
            {
                tcuSocket.Connect(ipAddress, 500);
            }
            catch (Exception)
            {
                return false;
            }

            //is this necessary?
            while (tcuSocket.Connected == false)
            {
                //do nothing
            }
            reader = new BinaryReader(tcuSocket.GetStream());
            writer = new BinaryWriter(tcuSocket.GetStream());
            string response = reader.ReadString();

            if (response.Equals("ID? ECU"))
            {
                writer.Write("TCU");
            }
            else
            {
                reader.Close();
                writer.Close();
                tcuSocket.Close();
                return false;
            }

            response = reader.ReadString();

            if (response.Equals("OK")) return true;
            else return false;

        }

        public string Listen()
        {
            return reader.ReadString();
        }

        public void TireStatus(string status)
        {
            writer.Write(status);
        }
    }
}
