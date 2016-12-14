using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
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

        private static RSAParameters privateKey;
        private static RSAParameters publicKey;
        private static RSAParameters ecuPubKey;
        //timestamps for validation if ECU message freshness 1s
        private static int EcuValidTime = 10000;

        //TESTING PURPOSES
        private string _lastmessage;
        public string LastMessage { get { return _lastmessage; } set { _lastmessage = value; } }

        public void Replay()
        {
            writer.Write(LastMessage);
        }

        public BCU(RSAParameters _privateKey, RSAParameters _ecuPubKey)
        {     //TODO
              /*
      editar campo para receber as chaves "privateKey" e "ecuPubKey" 
      guardar em modo chave e não em string
      sendo que actualmente estas estão a ser geradas no GenerateSendKeys 
               */
            privateKey = _privateKey;
            ecuPubKey = _ecuPubKey;

        }

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

            while(bcuSocket.Connected == false)
            {
                //do nothing
            }
            reader = new BinaryReader(bcuSocket.GetStream());
            writer = new BinaryWriter(bcuSocket.GetStream());
            string response  = reader.ReadString(); //asks for id

            response = removeTimestamp(DecryptMessage(response));
            string response2 = reader.ReadString();//asks for id 
            response2 = removeTimestamp(DecryptMessage(response2));

            //if the first message was not sucefull desencripted the second message was
            if (response == null) { response = response2; }
            if (response.Equals("ID? ECU"))//if it's ecu id 
            {
                //encrypt the message
                string encmessage = EncryptMessage(addTimestamp("BCU"));

                writer.Write(encmessage);//send i'm bcu
               
            }
            else
            {
                reader.Close();
                writer.Close();
                bcuSocket.Close();
                return false;
            }
            
            string cMessage = reader.ReadString();
          
            response = DecryptMessage(cMessage);
            response = removeTimestamp(response);
            
            if (response.Equals("OK?"))
            {
                writer.Write(EncryptMessage(addTimestamp("OK")));
                return true;
            }
            else
            {
                reader.Close();
                writer.Close();
                bcuSocket.Close();
                return false;
            }

        }

        //returns a timestamp
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        //add timestamp on the begin of the message before encripting
        public string addTimestamp(string message)
        {
            //add timestamp
            //return a
            //DateTime date1 = new DateTime(DateTime.Now);
            message = GetTimestamp(DateTime.Now) + "-" + message;
            return message;
        }

        //remove timestamp from the message and validates the timestamp, 
        //if timestamp is valid return  message without timestamp, if not returns null
        public string removeTimestamp(string decriptedmessage)
        {
            //decription was not sucessfull, because it's a wrong message
            if (decriptedmessage == null)
            { return null; }

            string[] messageparts = decriptedmessage.Split('-');
            long actualTime = long.Parse(GetTimestamp(DateTime.Now));
            long messageTime = long.Parse(messageparts[0]);
            long differenceTime = actualTime - messageTime;
            if (differenceTime < EcuValidTime)
            { return messageparts[1]; }

            return null;
        }


        public string EncryptMessage(string message)//[DONE] ready for ignition keys
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();

            byte[] bytesPlainTextData = null;
            byte[] bytesCypherText = null;
            string cypherText = null;

            csp.ImportParameters(ecuPubKey);
            //for encryption, always handle bytes...
            bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(message);
            //apply pkcs#1.5 padding and encrypt our data 
            bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
            //we might want a string representation of our cypher text... base64 will do
            cypherText = Convert.ToBase64String(bytesCypherText);

            //TEST
            LastMessage = cypherText;

            return cypherText;
        }

        public string DecryptMessage(string cypherText)//[DONE] ready for ignition keys
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();

            byte[] bytesPlainTextData = null;
            byte[] bytesCypherText = null;
            string plainTextData = null;
            //Console.WriteLine("cypherText");
            //Console.WriteLine(cypherText);
            //Console.WriteLine("endcypherText");
            bytesCypherText = Convert.FromBase64String(cypherText);
            //Console.WriteLine(bytesCypherText);
            //we want to decrypt, therefore we need a csp and load our private key
            csp.ImportParameters(privateKey);
            //decrypt and strip pkcs#1.5 padding
            try
            {
                bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
            }
            //if the decripting is not successefull enters the exception 
            catch (Exception)
            {
                //Console.WriteLine("null exception");
                return null;
            }
            //get our original plainText back...
            plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);

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


        public string Listen()
        {
            string response = reader.ReadString();
            response = DecryptMessage(response);
            return removeTimestamp(response);
        }

        public void Ack()
        {
            writer.Write(EncryptMessage(addTimestamp("Car Stopped")));
        }
    }
}
