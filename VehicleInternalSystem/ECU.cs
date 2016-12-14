using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehicleInternalSystem
{
    public class ECU
    {

        private TcpListener ecuSocket = null;
        private static IPAddress ipAddress;

        private TcpClient bcuSocket;
        private static BinaryReader bcuReader;
        private static BinaryWriter bcuWriter;
        //private key for encrypting from ecu to bcu
        private static RSAParameters bcu_ecuPriv;
        //public key to send to bcu
        private static RSAParameters bcu_ecuPub;
        //public key to decrypt messages from bcu
        private static RSAParameters bcuPubKey;


        private TcpClient tcuSocket;
        private static BinaryReader tcuReader;
        private static BinaryWriter tcuWriter;
        //private key for encrypting from ecu to tcu
        private static RSAParameters tcu_ecuPriv;
        //public key to send to tcu
        private static RSAParameters tcu_ecuPub;
        //public key to decrypt messages from tcu
        private static RSAParameters tcuPubKey;

        //timestamps for validation if BCU message freshness 1s
        private static int BcuValidTime = 10000;
        //timestamps for validation if TCUmessage freshness 6s
        private static int TcuValidTime = 60000;

        //TESTING PURPOSES
        private string _lastmessage;
        public string LastMessage { get { return _lastmessage; } set { _lastmessage = value; } }

        public void Replay()
        {
            bcuWriter.Write(LastMessage);
        }

        public ECU(RSAParameters _bcu_ecuPriv, RSAParameters _tcu_ecuPriv, RSAParameters _bcuPubKey, RSAParameters _tcuPubKey)
        {  
            bcu_ecuPriv = _bcu_ecuPriv;
            tcu_ecuPriv = _tcu_ecuPriv;
            bcuPubKey = _bcuPubKey;
            tcuPubKey = _tcuPubKey;           
        }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ECUForm(this));
        }

        public string InitECU()
        {
            if (ecuSocket == null)
            {
                ipAddress = IPAddress.Parse("127.0.0.1");
                ecuSocket = new TcpListener(ipAddress, 500);
                ecuSocket.Start();//exection not handle TO DO

                return ipAddress.ToString();
            }
            else return null;
        }
        
        public string HandleConnection()
        {

            TcpClient tempSocket = ecuSocket.AcceptTcpClient();
            BinaryWriter writer = new BinaryWriter(tempSocket.GetStream());
            BinaryReader reader = new BinaryReader(tempSocket.GetStream());

            //sends every combination to all requested connection
            string ecuID = "ID? ECU";
            string ecuIDtime = addTimestamp(ecuID);
           
            string sendBCU = EncryptMessage("BCU", ecuIDtime);
            string sendTCU = EncryptMessage("TCU", ecuIDtime);
 
            writer.Write(sendBCU); 
            writer.Write(sendTCU);
            string id = reader.ReadString();
 
            string BCU = "BCU";
            string TCU = "TCU";
        
            string idBCUtime = DecryptMessage(BCU, id);
            string idBCU = removeTimestamp(idBCUtime, BCU);

            string idTCUtime = DecryptMessage(TCU, id);
            string idTCU = removeTimestamp(idTCUtime, TCU);
 
            //type is BCU if speaking with BCU, and TCU for TCU
            string type = null;
            //BCU key decriptes well the message
            if (idBCU != null) {type = BCU;}
            //TCU key decriptes well the message
            if (idTCU != null) {type = TCU;}

            writer.Write(EncryptMessage(type, addTimestamp("OK?")));

            string encriptedResponse = reader.ReadString();

            string responseTime = DecryptMessage(type, encriptedResponse);

            string response = removeTimestamp(responseTime, type);
            if (!response.Equals("OK"))
            {return (type + " tried to connect, but was denied\n");}
            switch (type) { 
                case "BCU":
                    bcuSocket = tempSocket;
                    bcuReader = reader;
                    bcuWriter = writer;
                    return "CONNECTED TO BCU\n";
                
                case "TCU":
                    
                    tcuSocket = tempSocket;
                    tcuReader = reader;
                    tcuWriter = writer;
                    return "CONNECTED TO TCU";
                    
            }

            return (id + " tried to connect, but was denied\n");
        }

        

        //returns a timestamp
        public static String GetTimestamp(DateTime value)
        {

            return value.ToString("yyyyMMddHHmmssffff");
        }

        //add timestamp on the begin of the message before encripting
        public string addTimestamp(string message) {

            message = GetTimestamp(DateTime.Now)+"-" + message;
            return message;
        }

        //remove timestamp from the message and validates the timestamp, 
        //if timestamp is valid return  message without timestamp, if not returns null
        public string removeTimestamp(string decriptedmessage, string type)
        {
            //decription was not sucessfull, because it's a wrong message
            if (decriptedmessage == null)
            { return null; }
            string[] messageparts = decriptedmessage.Split('-');

            long actualTime = long.Parse(GetTimestamp(DateTime.Now));
            long messageTime = long.Parse(messageparts[0]);
            long differenceTime = actualTime - messageTime;

            switch (type)
            {
                case "BCU":

                    if (differenceTime < BcuValidTime) { return messageparts[1]; }
                    break;
                case "TCU":

                    if (differenceTime < TcuValidTime) { return messageparts[1]; }
                    break;
            }
            return null;
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
                    csp.ImportParameters(bcuPubKey);
                    //for encryption, always handle bytes...
                    bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(message);
                    //apply pkcs#1.5 padding and encrypt our data 
                    bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
                    //we might want a string representation of our cypher text... base64 will do
                    cypherText = Convert.ToBase64String(bytesCypherText);
                    break;
                case "TCU":
                    csp.ImportParameters(tcuPubKey);
                    //for encryption, always handle bytes...
                    bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(message);
                    //apply pkcs#1.5 padding and encrypt our data 
                    bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
                    //we might want a string representation of our cypher text... base64 will do
                    cypherText = Convert.ToBase64String(bytesCypherText);
                    break;
            }
            //TEST
            LastMessage = cypherText;
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
                    try { bytesPlainTextData = csp.Decrypt(bytesCypherText, false); }
                    //if the decripting is not successefull enters the exception 
                    catch (Exception) { return null; }
                    //get our original plainText back...
                    plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
                    //decoded text
                    break;
                case "TCU":
                    bytesCypherText = Convert.FromBase64String(cypherText);
                    //we want to decrypt, therefore we need a csp and load our private key
                    csp.ImportParameters(tcu_ecuPriv);
                    //decrypt and strip pkcs#1.5 padding
                    try { bytesPlainTextData = csp.Decrypt(bytesCypherText, false); }
                    //if the decripting is not successefull enters the exception 
                    catch (Exception) { return null; }
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
            return removeTimestamp (DecryptMessage("TCU", response), "BCU");
        }

        public string BrakeCmd()
        {
            string response;
            try
            {
                bcuWriter.Write(EncryptMessage("BCU",addTimestamp("BRAKE")));
                response = bcuReader.ReadString();
                return removeTimestamp (DecryptMessage("BCU", response), "BCU");
            }
            catch (Exception)
            {
                response = "BCU not connected";
                return response;
            }
        }
    }
}
