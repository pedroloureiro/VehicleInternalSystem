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

    public class TCU
    {
        private TcpClient tcuSocket;
        private static IPAddress ipAddress;

        private static BinaryReader reader;
        private static BinaryWriter writer;

        private static RSAParameters privateKey;
        private static RSAParameters publicKey;
        private static RSAParameters ecuPubKey;

        //timestamps for validation if ECU message freshness 1s
        private static int EcuValidTime = 10000;

        public TCU(RSAParameters _privateKey, RSAParameters _ecuPubKey)
        {
            //TODO
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
            string response = reader.ReadString();//asks for id//[TODO]decript with bcuPrivateKey 
            response = DecryptMessage(response);
            response = removeTimestamp(response);
            string response2 = reader.ReadString();//asks for id//[TODO]decript with bcuPrivateKey 
            response2 = DecryptMessage(response2);
            response2 = removeTimestamp(response2);
            //Console.WriteLine("RESPONSE");
            //Console.WriteLine(response);
            //if the first message was not sucefull desencripted the second message was
            if (response == null) { response = response2; }
            //Console.WriteLine("RESPONSEafter filtrued bad arsponse");
            //Console.WriteLine(response);
            if (response.Equals("ID? ECU"))//if it's bcu id 
            {
                //add timestamp
                string encmessage = addTimestamp("TCU");
                //encrypt the message
                encmessage = EncryptMessage(encmessage);
                
                //Console.WriteLine("EncryptMessage(TCU");
                //Console.WriteLine(encmessage);
                writer.Write(encmessage);//send i'm tcu//[TODO]encript with ecutPublicKey
            }
            else
            {
                reader.Close();
                writer.Close();
                tcuSocket.Close();
                return false;
            }

            string encMessage = reader.ReadString();//is this needed?//[TODO]change ecuKey to encMessage
            response = DecryptMessage(encMessage);//[TODO] DecryptMessage(encMessage)
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
                tcuSocket.Close();
                return false;
            }

        }

        //returns a timestamp
        public static String GetTimestamp(DateTime value)
        {
            //Console.WriteLine("get time stamp");
            //Console.WriteLine(value.ToString("yyyyMMddHHmmssffff"));
            return value.ToString("yyyyMMddHHmmssffff");
        }

        //add timestamp on the begin of the message before encripting
        public string addTimestamp(string message)
        {
            //add timestamp
            //return a
            //DateTime date1 = new DateTime(DateTime.Now);
            //Console.WriteLine("add time stamp");
            //Console.WriteLine(message);
            message = GetTimestamp(DateTime.Now) + "-" + message;
            //Console.WriteLine(message);
            return message;
        }

        //remove timestamp from the message and validates the timestamp, 
        //if timestamp is valid return  message without timestamp, if not returns null
        public string removeTimestamp(string decriptedmessage)
        {
            //Console.WriteLine("decriptedmessage.Length");
            //Console.WriteLine(decriptedmessage);
            string[] messageparts = null;
            try
            {
                messageparts = decriptedmessage.Split('-');
            }
            catch (Exception) { return null; }
            //Console.WriteLine("messageparts.Length");
            //Console.WriteLine(messageparts[0]);
            //Console.WriteLine(messageparts[1]);
            long actualTime = long.Parse(GetTimestamp(DateTime.Now));
            long messageTime = long.Parse(messageparts[0]);
            long differenceTime = actualTime - messageTime;
            //Console.WriteLine(differenceTime);
            //Console.WriteLine("in bcu time");
            if (differenceTime < EcuValidTime)
            { return messageparts[1]; }
 
            return null;
        }


        public string GenerateSendKeys(string ecuKey)//[TODO] not needed
        {
            var csp = new RSACryptoServiceProvider(2048);

            privateKey = csp.ExportParameters(true);
            //and the public key ...
            publicKey = csp.ExportParameters(false);
            ecuPubKey = ConvertStringToKey(ecuKey);
            //BEGIN trying to understand how rsakeys work//to erase
            /*Console.Write("privateKey:");
            Console.WriteLine(ConvertKeyToString(privateKey));
            Console.Write("publicKey:");
            Console.WriteLine(ConvertKeyToString(publicKey));*/
            //END trying to understand how rsakeys work
            writer.Write(ConvertKeyToString(publicKey));
            string encryptedResponse = reader.ReadString();

            return DecryptMessage(encryptedResponse);
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

            return cypherText;
        }

        public string DecryptMessage(string cypherText)//[DONE] ready for ignition keys
        {
            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();

            byte[] bytesPlainTextData = null;
            byte[] bytesCypherText = null;
            string plainTextData = null;
            
            bytesCypherText = Convert.FromBase64String(cypherText);
            //we want to decrypt, therefore we need a csp and load our private key
            csp.ImportParameters(privateKey);
            //decrypt and strip pkcs#1.5 padding
            try
            {
                //Console.WriteLine("--bytesPlainTextData");
                //Console.WriteLine(bytesPlainTextData);
                bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
                //Console.WriteLine("bytesPlainTextData");
                //Console.WriteLine(bytesPlainTextData);
            }
            //if the decripting is not successefull enters the exception 
            catch (Exception)
            {
                //Console.WriteLine("null exception");
                return null;
            }

            //get our original plainText back...
            //Console.WriteLine("11111plainTextData");
            //Console.WriteLine(plainTextData);
            plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
            //Console.WriteLine("2222plainTextData");
            //Console.WriteLine(plainTextData);
            //Console.WriteLine("------BEGIN  TCUprivateKey");//to erase
            //Console.WriteLine(privateKey);
            //Console.WriteLine(csp.ImportParameters(privateKey);));
            //nsole.WriteLine("------END    TCUprivateKey");



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
            return reader.ReadString();
        }

        public void TireStatus(string status)
        {
            writer.Write(EncryptMessage(addTimestamp(status)));
        }
    }
}
