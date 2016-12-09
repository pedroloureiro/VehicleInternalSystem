using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehicleInternalSystem
{
    public class Ignition
    {
        //RSA Keys 
        //BCU 
        //private key 
        private static RSAParameters bcuPriv;
        //public key 
        private static RSAParameters bcuPub;
        //TCU
        //private key 
        private static RSAParameters tcuPriv;
        //public key 
        private static RSAParameters tcuPub;
        //ECUB - ECU comunicating with BCU
        //private key 
        private static RSAParameters ecubPriv;
        //public key 
        private static RSAParameters ecubPub;
        //ECUT - ECU comunicating with TCU
        //private key 
        private static RSAParameters ecutPriv;
        //public key 
        private static RSAParameters ecutPub;



        [STAThread]
        static void Main()
        {
            var thread0 = new Thread(GenerateKeys);
            thread0.Start();
            Thread.Sleep(5000); //so thread0 executes and ends before others start 1 2 3  
            var thread1 = new Thread(ThreadECU);
            var thread2 = new Thread(ThreadBCU);
            var thread3 = new Thread(ThreadTCU);

            thread1.TrySetApartmentState(ApartmentState.STA);
            thread1.Start();

            thread2.TrySetApartmentState(ApartmentState.STA);
            thread2.Start();

            thread3.TrySetApartmentState(ApartmentState.STA);
            thread3.Start();
        }

        private static void ThreadECU()
        {
            //ECU ecu = new ECU();
            //ECU ecu = new ECU(ecubPriv,ecutPriv, bcuPub, tcuPub);
            ECU ecu = new ECU(ecubPriv, bcuPriv, bcuPub, tcuPub);//edited for tests
            ecu.Run();       
        }

        private static void ThreadBCU()
        {
            //BCU bcu = new BCU();
            BCU bcu = new BCU(bcuPriv, ecubPub);
            bcu.Run();
        }

        private static void ThreadTCU()
        {
            //TCU tcu = new TCU();
            TCU tcu = new TCU(tcuPriv, ecutPub);
            tcu.Run();
        }


        private static string ConvertKeyToString(RSAParameters key)
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
        //To generate each pair of RSA Keys
        private static void GenerateKeys()
        {
            var cspBCU = new RSACryptoServiceProvider(2048);
            var cspTCU = new RSACryptoServiceProvider(2048);
            var cspECUB = new RSACryptoServiceProvider(2048);
            var cspECUT = new RSACryptoServiceProvider(2048);

            //how to get the private keys
            bcuPriv = cspBCU.ExportParameters(true);
            tcuPriv = cspTCU.ExportParameters(true);
            ecubPriv = cspECUB.ExportParameters(true);
            ecutPriv = cspECUT.ExportParameters(true);
            //and the public key ...
            bcuPub = cspBCU.ExportParameters(false);
            tcuPub = cspTCU.ExportParameters(false);
            ecubPub = cspECUB.ExportParameters(false);
            ecutPub = cspECUT.ExportParameters(false);

            /*
            Console.WriteLine("<<<BEGIN  bcuPriv");
            Console.WriteLine(ConvertKeyToString(bcuPriv));
            Console.WriteLine("<<<BEGIN  tcuPriv");
            Console.WriteLine(ConvertKeyToString(tcuPriv));
            Console.WriteLine("<<<BEGIN  ecuPriv");
            Console.WriteLine(ConvertKeyToString(ecuPriv));
            Console.WriteLine("<<<BEGIN  bcuPub");
            Console.WriteLine(ConvertKeyToString(bcuPub));
            Console.WriteLine("<<<BEGIN  tcuPub");
            Console.WriteLine(ConvertKeyToString(tcuPub));
            Console.WriteLine("<<<BEGIN  ecuPub");
            Console.WriteLine(ConvertKeyToString(ecuPub));
            Console.WriteLine("<<<BEGIN  ENNDDD");
            */

        }

    }
}
