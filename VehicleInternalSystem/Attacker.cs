using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehicleInternalSystem
{

    public class Attacker
    {
        private ECU ecu;
        private TCU tcu;
        private BCU bcu;

        public Attacker(ECU ecu, TCU tcu, BCU bcu)
        {
            this.ecu = ecu;
            this.tcu = tcu;
            this.bcu = bcu;
        }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AttackerForm(this));
        }

        public string ShowLastMsgECU()
        {
            return ecu.LastMessage;
        }

        public string ShowLastMsgBCU()
        {
            return bcu.LastMessage;
        }

        public string ShowLastMsgTCU()
        {
            return tcu.LastMessage;
        }

        public void ReplayECU()
        {
            ecu.Replay();
        }

        public void ReplayBCU()
        {
            bcu.Replay();
        }

        public void ReplayTCU()
        {
            tcu.Replay();
        }
    }
}
