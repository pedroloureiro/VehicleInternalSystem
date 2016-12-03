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

    public class BCU
    {
        private TcpClient bcuSocket;
        private static IPAddress ipAddress;

        private static BinaryReader reader;
        private static BinaryWriter writer;

        public BCU() { }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BCUForm(this));
        }

        public bool Connect()
        {
            bcuSocket = new TcpClient();
            ipAddress = IPAddress.Parse("127.0.0.1");

            try
            {
                bcuSocket.Connect(ipAddress, 500);
            } catch(Exception)
            {
                return false;
            }

            //is this necessary?
            while(bcuSocket.Connected == false)
            {
                //do nothing
            }
            reader = new BinaryReader(bcuSocket.GetStream());
            writer = new BinaryWriter(bcuSocket.GetStream());
            string response  = reader.ReadString();

            if(response.Equals("ID? ECU"))
            {
                writer.Write("BCU");
            }
            else
            {
                reader.Close();
                writer.Close();
                bcuSocket.Close();
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

        public void Ack()
        {
            writer.Write("Car Stopped");
        }
    }
}
