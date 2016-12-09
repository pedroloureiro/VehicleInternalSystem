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

        public ECU(RSAParameters _bcu_ecuPriv, RSAParameters _tcu_ecuPriv, RSAParameters _bcuPubKey, RSAParameters _tcuPubKey)
        {
            //TODO
            /*
    editar campo para receber as chaves "privateKey" , "bcuPubKey"e "tcuPubKey" 
    guardar em modo chave e não em string
    sendo que actualmente estas estão a ser geradas no GenerateSendKeys
             */
            bcu_ecuPriv = _bcu_ecuPriv;//WHAT IS THIS??
            tcu_ecuPriv = _tcu_ecuPriv;// "    "  "
            bcuPubKey = _bcuPubKey;
            tcuPubKey = _tcuPubKey;
            //Console.WriteLine("------------------BCU constructor");
            //Console.WriteLine("------------------bcu_ecuPriv");
            //Console.WriteLine(ConvertKeyToString(bcu_ecuPriv));
            //Console.WriteLine("------------------tcu_ecuPriv");
            //Console.WriteLine(ConvertKeyToString(tcu_ecuPriv));
            //Console.WriteLine("------------------bcuPubKey");
            //Console.WriteLine(ConvertKeyToString(bcuPubKey));
            //Console.WriteLine("------------------tcuPubKey");
            //Console.WriteLine(ConvertKeyToString(tcuPubKey));
            //Console.WriteLine("------------------END");
        }

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
            ecuSocket.Start();//exection not handle TO DO

            return ipAddress.ToString();
        }

        public string HandleConnection()
        {

            TcpClient tempSocket = ecuSocket.AcceptTcpClient();
            BinaryWriter writer = new BinaryWriter(tempSocket.GetStream());
            BinaryReader reader = new BinaryReader(tempSocket.GetStream());

            //sends every combination to all requested connection
            //[TODO]
            string ecuID = "ID? ECU";
            string ecuIDtime = addTimestamp(ecuID);
            //Console.WriteLine(testtime);
            //string stupid = removeTimestamp(testtime, "BCU");
            //Console.WriteLine("stupid");
            //Console.WriteLine(stupid);
            //string testtime2 = addTimestamp(ecuID);
            //Console.WriteLine(testtime2);
            //Thread.Sleep(6000);
            //string stupid2 = removeTimestamp(testtime2, "TCU");
            //if (stupid2 == null) { Console.WriteLine("snuulllllllllll2"); }
            //Console.WriteLine("stupid22");
            //Console.WriteLine(stupid2);
            string sendBCU = EncryptMessage("BCU", ecuIDtime);//test
            Console.WriteLine("sendBCU");
            Console.WriteLine(sendBCU);
            Console.WriteLine("__sendBCU");
            //Console.WriteLine(DecryptMessage("TCU", sendBCU));
            string sendTCU = EncryptMessage("TCU", ecuIDtime);
            Console.WriteLine("sendTCU");
            Console.WriteLine(sendTCU);
            Console.WriteLine("__sendTCU");
            writer.Write(sendBCU); 
            writer.Write(sendTCU);//end[todo]
            string id = reader.ReadString();
            Console.WriteLine("id");
            Console.WriteLine(id);
            //[TODO] DecryptMessage(id)
            string BCU = "BCU";
            string TCU = "TCU";
            Console.WriteLine("pass here");
            string idBCUtime = DecryptMessage(BCU, id);
            string idBCU = removeTimestamp(idBCUtime, BCU);
            //Console.WriteLine("idbcu");
            //Console.WriteLine(idBCU);
            Console.WriteLine("pass here1");
            string idTCUtime = DecryptMessage(TCU, id);
            string idTCU = removeTimestamp(idTCUtime, TCU);
            //Console.WriteLine("idtcu");
            //Console.WriteLine(idTCU);
            //type is BCU if speaking with BCU, and TCU for TCU
            string type = null;
            //BCU key decriptes well the message
            if (idBCU != null) {type = BCU;}
            //TCU key decriptes well the message
            if (idTCU != null) {type = TCU;}
            Console.WriteLine("type");
            Console.WriteLine(type);
            Console.WriteLine("after type");
            writer.Write(EncryptMessage(type, addTimestamp("OK?")));
            Console.WriteLine("pass here2");
            string encriptedResponse = reader.ReadString();
            Console.WriteLine("pass here3");
            string responseTime = DecryptMessage(type, encriptedResponse);
            Console.WriteLine("pass here4");
            string response = removeTimestamp(responseTime, type);
            if (!response.Equals("OK"))
            {return (type + " tried to connect, but was denied\n");}//end[todo]
            switch (type)//changed id to type
            {
                case "BCU"://remove

                    //if (KeyDistribution(reader, writer, id))//[TODO] not needed

                    //{
                    bcuSocket = tempSocket;
                    bcuReader = reader;
                    bcuWriter = writer;
                    return "CONNECTED TO BCU\n";
                //}
                //else return (type + " tried to connect, but was denied\n");

                case "TCU":
                    //if (KeyDistribution(reader, writer, id))//[TODO] not needed
                    //{
                    tcuSocket = tempSocket;
                    tcuReader = reader;
                    tcuWriter = writer;
                    return "CONNECTED TO TCU";
                    //}
                    //else return (id + " tried to connect, but was denied\n");
            }

            return (id + " tried to connect, but was denied\n");//[TODO]EDIT id to idBCU, idTCU or other
        }

        public bool KeyDistribution(BinaryReader reader, BinaryWriter writer, string type)//[TODO] not needed
        {
            var csp = new RSACryptoServiceProvider(2048);
            string response = null;

            switch (type)
            {
                case "BCU":
                    //how to get the private key
                    bcu_ecuPriv = csp.ExportParameters(true);
                    // Console.WriteLine("---BEGIN  bcu_ecuPriv");//to erase
                    //Console.WriteLine(ConvertKeyToString(bcu_ecuPriv));
                    //Console.WriteLine("---END    bcu_ecuPriv");
                    //and the public key ...
                    bcu_ecuPub = csp.ExportParameters(false);
                    //Console.WriteLine("<<<BEGIN  bcu_ecuPub");//to erase
                    //Console.WriteLine(bcu_ecuPub);
                    //Console.WriteLine(ConvertKeyToString(bcu_ecuPub));
                    //Console.WriteLine(">>>END    bcu_ecuPub");
                    writer.Write(ConvertKeyToString(bcu_ecuPub));

                    string bcuString = reader.ReadString();
                    bcuPubKey = ConvertStringToKey(bcuString);

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
                    tcuPubKey = ConvertStringToKey(tcuString);

                    writer.Write(EncryptMessage(type, "OK?"));
                    tcuString = reader.ReadString();
                    response = DecryptMessage(type, tcuString);
                    break;
            }

            return response.Equals("OK");
        }

        //returns a timestamp
        public static String GetTimestamp(DateTime value)
        {
            //Console.WriteLine("get time stamp");
            //Console.WriteLine(value.ToString("yyyyMMddHHmmssffff"));
            return value.ToString("yyyyMMddHHmmssffff");
        }

        //add timestamp on the begin of the message before encripting
        public string addTimestamp(string message) {
            //add timestamp
            //return a
            //DateTime date1 = new DateTime(DateTime.Now);
            //Console.WriteLine("add time stamp");
            //Console.WriteLine(message);
            message = GetTimestamp(DateTime.Now)+"-" + message;
            //Console.WriteLine(message);
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
            //Console.WriteLine("messageparts.Length");
            //Console.WriteLine(messageparts[0]);
            //Console.WriteLine(messageparts[1]);
            long actualTime = long.Parse(GetTimestamp(DateTime.Now));
            long messageTime = long.Parse(messageparts[0]);
            long differenceTime = actualTime - messageTime;
            //.WriteLine(differenceTime);
            //Console.WriteLine(type);
            switch (type)
            {
                case "BCU":
                    //Console.WriteLine("in bcu time");
                    if (differenceTime < BcuValidTime) { return messageparts[1]; }
                    break;
                case "TCU":
                    //Console.WriteLine("in tcu time");
                   // Console.WriteLine(differenceTime);
                    //Console.WriteLine(TcuValidTime);
                    //Console.WriteLine("in tcu time");
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
                    //Console.WriteLine("concerted in bcumode");
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
                    //delete the 3 printlines downhere
                    //Console.WriteLine("<<<BEGINplainTextData");
                    //Console.WriteLine(plainTextData);
                    //Console.WriteLine(">>>ENDplainTextData");
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
                    //Console.WriteLine("AKA tcu decriptes");
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
            //delete these 2 printlines
            //Console.WriteLine("LISTENresponse");
            //Console.WriteLine(response);
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
