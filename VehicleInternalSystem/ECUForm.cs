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
    public partial class ECUForm : Form
    {
        private ECU ecu;

        delegate void SetTextCallback(string text);

        public ECUForm(ECU ecu)
        {
            this.ecu = ecu;
            InitializeComponent();
        }

        private void brakeBtn_Click(object sender, EventArgs e)
        {
            AddToLog("BRAKE");
            string response = ecu.BrakeCmd();
            AddToLog("[BCU]"+response);
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            string address = ecu.InitECU();
            AddToLog("ECU online");
            Thread connectionThread = new Thread(() => HandleConnection());
            connectionThread.Start();
        }

        private void ListenTCU()
        {
            while(true)
            {
                string status = ecu.ListenTCU();
                string[] parsed = status.Split(null);
                FLTValue(parsed[0]);
                FRTValue(parsed[1]);
                BLTValue(parsed[2]);
                BRTValue(parsed[3]);
            }
        }

        private void HandleConnection()
        {
            while(true)
            {
                string response = ecu.HandleConnection();
                if (response.Equals("CONNECTED TO TCU"))
                {
                    Thread listenTCUThread = new Thread(() => ListenTCU());
                    listenTCUThread.Start();
                }
                AddToLog(response);
            }
        }

        private void FRTValue(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.FrontRightTire.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(FRTValue);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.FrontRightTire.Text = text;
            }
        }

        private void FLTValue(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.FrontLeftTire.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(FLTValue);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.FrontLeftTire.Text = text;
            }
        }

        private void BRTValue(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.BackRightTire.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(BRTValue);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.BackRightTire.Text = text;
            }
        }

        private void BLTValue(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.BackLeftTire.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(BLTValue);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.BackLeftTire.Text = text;
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
