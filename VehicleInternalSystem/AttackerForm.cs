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
    public partial class AttackerForm : Form
    {
        private Attacker att;
        private string bcuLastMsg;

        delegate void SetTextCallback(string text);

        public AttackerForm(Attacker att)
        {
            this.att = att;
            InitializeComponent();
        }

        //BUTTON TO READ LAST MESSAGE

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

        private void button1_Click(object sender, EventArgs e)
        {
            AddToLog("[ECU] " + att.ShowLastMsgECU());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddToLog("[BCU] " + att.ShowLastMsgBCU());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddToLog("[TCU] " + att.ShowLastMsgTCU());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            att.ReplayECU();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            att.ReplayBCU();
        }

    }
}
