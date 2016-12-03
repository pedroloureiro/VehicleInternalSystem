using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehicleInternalSystem
{
    public class Ignition
    {
        [STAThread]
        static void Main()
        {

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
            ECU ecu = new ECU();
            ecu.Run();       
        }

        private static void ThreadBCU()
        {
            BCU bcu = new BCU();
            bcu.Run();
        }

        private static void ThreadTCU()
        {
            TCU tcu = new TCU();
            tcu.Run();
        }
    }
}
