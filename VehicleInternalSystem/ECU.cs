using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
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
        //private key for encrypting from ecu to bcu
        private static RSAParameters bcu_ecuPriv;
        //public key to send to bcu
        private static RSAParameters bcu_ecuPub;
        //public key to decrypt messages from bcu
        private static RSAParameters bcuPub;


        private TcpClient tcuSocket;
        private static BinaryReader tcuReader;
        private static BinaryWriter tcuWriter;
        //private key for encrypting from ecu to tcu
        private static RSAParameters tcu_ecuPriv;
        //public key to send to tcu
        private static RSAParameters tcu_ecuPub;
        //public key to decrypt messages from tcu
        private static RSAParameters tcuPub;

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
            string id = reader.ReadString();
            switch (id)
            {
                case "BCU":
                    
                    if(KeyDistribution(reader, writer, id))
                    {
                        bcuSocket = tempSocket;
                        bcuReader = reader;
                        bcuWriter = writer;
                        return "CONNECTED TO BCU\n";
                    }
                    else return (id + " tried to connect, but was denied\n");

                case "TCU":
                    if (KeyDistribution(reader, writer, id))
                    {
                        tcuSocket = tempSocket;
                        tcuReader = reader;
                        tcuWriter = writer;
                        return "CONNECTED TO TCU";
                    }
                    else return (id + " tried to connect, but was denied\n");
            }

            return (id + " tried to connect, but was denied\n");
        }

        public bool KeyDistribution(BinaryReader reader, BinaryWriter writer, string type)
        {
            var csp = new RSACryptoServiceProvider(2048);
            string response = null;

            switch (type)
            {
                case "BCU":
                    //how to get the private key
                    bcu_ecuPriv = csp.ExportParameters(true);
                    //and the public key ...
                    bcu_ecuPub = csp.ExportParameters(false);
                    writer.Write(ConvertKeyToString(bcu_ecuPub));

                    string bcuString = reader.ReadString();
                    bcuPub = ConvertStringToKey(bcuString);

                    writer.Write(EncryptMessage(type, "OK?"));
                    bcuString = reader.ReadString();
                    response = DecryptMessage(type, bcuString);
                    break;

                case "TCU":
                    //how to get the private key
                    tcu_ecuPriv = csp.ExportParameters(true);
                    //and the public key ...
                    tcu_ecuPub = csp.ExportParameters(false);
                    writer.Write(ConvertKeyToString(tcu_ecuPub));

                    string tcuString= reader.ReadString();
                    tcuPub = ConvertStringToKey(tcuString);

                    writer.Write(EncryptMessage(type, "OK?"));
                    tcuString = reader.ReadString();
                    response = DecryptMessage(type, tcuString);
                    break;
            }

            return response.Equals("OK");
        }

        public string EncryptMessage(string type, string message)
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();

            byte[] bytesPlainTextData = null;
            byte[] bytesCypherText = null;
            string cypherText = null;

            switch (type)
            {
                case "BCU":
                    csp.ImportParameters(bcuPub);
                    //for encryption, always handle bytes...
                    bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(message);
                    //apply pkcs#1.5 padding and encrypt our data 
                    bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
                    //we might want a string representation of our cypher text... base64 will do
                    cypherText = Convert.ToBase64String(bytesCypherText);
                    break;
                case "TCU":
                    csp.ImportParameters(tcuPub);
                    //for encryption, always handle bytes...
                    bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(message);
                    //apply pkcs#1.5 padding and encrypt our data 
                    bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
                    //we might want a string representation of our cypher text... base64 will do
                    cypherText = Convert.ToBase64String(bytesCypherText);
                    break;
            }

            return cypherText;
        }

        public string DecryptMessage(string type, string cypherText)
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();

            byte[] bytesPlainTextData = null;
            byte[] bytesCypherText = null;
            string plainTextData = null;

            switch (type)
            {
                case "BCU":
                    bytesCypherText = Convert.FromBase64String(cypherText);
                    //we want to decrypt, therefore we need a csp and load our private key
                    csp.ImportParameters(bcu_ecuPriv);
                    //decrypt and strip pkcs#1.5 padding
                    bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
                    //get our original plainText back...
                    plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
                    break;
                case "TCU":
                    bytesCypherText = Convert.FromBase64String(cypherText);
                    //we want to decrypt, therefore we need a csp and load our private key
                    csp.ImportParameters(tcu_ecuPriv);
                    //decrypt and strip pkcs#1.5 padding
                    bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
                    //get our original plainText back...
                    plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
                    break;
            }

            return plainTextData;
        }

        public string ConvertKeyToString(RSAParameters key)
        {
            //we need some buffer
            var sw = new System.IO.StringWriter();
            //we need a serializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //serialize the key into the stream
            xs.Serialize(sw, key);
            //get the string from the stream
            return sw.ToString();
        }

        public RSAParameters ConvertStringToKey(string key)
        {
            //get a stream from the string
            var sr = new System.IO.StringReader(key);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            RSAParameters pubKey = (RSAParameters)xs.Deserialize(sr);
            return pubKey;
        }

        public string ListenTCU()
        {
            string response = tcuReader.ReadString();

            return DecryptMessage("TCU", response);
        }

        public string BrakeCmd()
        {
            string response;
            try
            {
                bcuWriter.Write(EncryptMessage("BCU","BRAKE"));
                response = bcuReader.ReadString();
                return DecryptMessage("BCU", response);
            }
            catch (Exception)
            {
                response = "BCU not connected";
                return response;
            }
        }
    }
}
