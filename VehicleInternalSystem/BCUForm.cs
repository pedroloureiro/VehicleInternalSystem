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
    public partial class BCUForm : Form
    {
        private BCU bcu;

        delegate void SetTextCallback(string text);

        public BCUForm(BCU bcu)
        {
            this.bcu = bcu;
            InitializeComponent();
        }

        private void connectBtn_Click(object sender, EventArgs e)
        {
            AddToLog("Trying to connect to ECU");
            if(bcu.Connect())
            {
                AddToLog("CONNECTED");

                Thread listenThread = new Thread(() => Listen());
                listenThread.Start();
            } else
            {
                AddToLog("Connection Failed");
            }
            
        }

        private void Listen()
        {
            string cmd;

            while(true)
            {
                cmd = bcu.Listen();
                AddToLog("[ECU]" + cmd);
                bcu.Ack();
                AddToLog("Car Stopped");
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
