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

        //FOR TESTING PURPOSES
        private static ECU ecu;
        private static TCU tcu;
        private static BCU bcu;



        [STAThread]
        static void Main()
        {
            var thread0 = new Thread(GenerateKeys);
            thread0.Start();
            thread0.Join();
            var thread1 = new Thread(ThreadECU);
            var thread2 = new Thread(ThreadBCU);
            var thread3 = new Thread(ThreadTCU);
            var thread4 = new Thread(ThreadATT);


            thread1.TrySetApartmentState(ApartmentState.STA);
            thread1.Start();

            thread2.TrySetApartmentState(ApartmentState.STA);
            thread2.Start();

            thread3.TrySetApartmentState(ApartmentState.STA);
            thread3.Start();

            Thread.Sleep(1000);

            thread4.TrySetApartmentState(ApartmentState.STA);
            thread4.Start();
        }

        private static void ThreadECU()
        {
            ecu = new ECU(ecubPriv,ecutPriv, bcuPub, tcuPub);
            ecu.Run();       
        }

        private static void ThreadBCU()
        {
            bcu = new BCU(bcuPriv, ecubPub);
            bcu.Run();
        }

        private static void ThreadTCU()
        {
            tcu = new TCU(tcuPriv, ecutPub);
            tcu.Run();
        }

        private static void ThreadATT()
        {
            Attacker att = new Attacker(ecu, tcu, bcu);
            att.Run();
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


        }

    }
}
