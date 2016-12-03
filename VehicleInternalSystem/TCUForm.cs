using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VehicleInternalSystem
{
    public partial class TCUForm : Form
    {
        private TCU tcu;

        delegate void SetTextCallback(string text);

        public TCUForm(TCU tcu)
        {
            this.tcu = tcu;
            InitializeComponent();
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            AddToLog("Trying to connect to ECU");
            if (tcu.Connect())
            {
                AddToLog("CONNECTED");

                Thread tireStatusThread = new Thread(() => TireStatus());
                tireStatusThread.Start();
            }
            else
            {
                AddToLog("Connection Failed");
            }
        }

        private void TireStatus()
        {
            string response;
            while(true)
            {
                response = FrontLeftTire.Value.ToString() + " "
                    + FrontRightTire.Value.ToString() + " "
                    + BackLeftTire.Value.ToString() + " "
                    + BackRightTire.Value.ToString();
                tcu.TireStatus(response);
                Thread.Sleep(5000);
            }
        }


        private void AddToLog(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.TextView.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AddToLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.TextView.AppendText(text + "\n");
            }
        }
    }
}
