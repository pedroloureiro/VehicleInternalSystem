namespace VehicleInternalSystem
{
    partial class ECUForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TextView = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.brakeBtn = new System.Windows.Forms.Button();
            this.connectBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.FrontLeftTire = new System.Windows.Forms.TextBox();
            this.FrontRightTire = new System.Windows.Forms.TextBox();
            this.BackLeftTire = new System.Windows.Forms.TextBox();
            this.BackRightTire = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TextView
            // 
            this.TextView.Location = new System.Drawing.Point(12, 12);
            this.TextView.Multiline = true;
            this.TextView.Name = "TextView";
            this.TextView.ReadOnly = true;
            this.TextView.Size = new System.Drawing.Size(386, 116);
            this.TextView.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 210);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "BCU Controls";
            // 
            // brakeBtn
            // 
            this.brakeBtn.Location = new System.Drawing.Point(24, 226);
            this.brakeBtn.Name = "brakeBtn";
            this.brakeBtn.Size = new System.Drawing.Size(75, 23);
            this.brakeBtn.TabIndex = 2;
            this.brakeBtn.Text = "BRAKE";
            this.brakeBtn.UseVisualStyleBackColor = true;
            this.brakeBtn.Click += new System.EventHandler(this.brakeBtn_Click);
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(170, 134);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 3;
            this.connectBtn.Text = "Enable";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(283, 210);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "TCU Controls";
            // 
            // FrontLeftTire
            // 
            this.FrontLeftTire.Location = new System.Drawing.Point(244, 249);
            this.FrontLeftTire.Name = "FrontLeftTire";
            this.FrontLeftTire.ReadOnly = true;
            this.FrontLeftTire.Size = new System.Drawing.Size(45, 20);
            this.FrontLeftTire.TabIndex = 5;
            // 
            // FrontRightTire
            // 
            this.FrontRightTire.Location = new System.Drawing.Point(353, 249);
            this.FrontRightTire.Name = "FrontRightTire";
            this.FrontRightTire.ReadOnly = true;
            this.FrontRightTire.Size = new System.Drawing.Size(45, 20);
            this.FrontRightTire.TabIndex = 6;
            // 
            // BackLeftTire
            // 
            this.BackLeftTire.Location = new System.Drawing.Point(244, 309);
            this.BackLeftTire.Name = "BackLeftTire";
            this.BackLeftTire.ReadOnly = true;
            this.BackLeftTire.Size = new System.Drawing.Size(45, 20);
            this.BackLeftTire.TabIndex = 7;
            // 
            // BackRightTire
            // 
            this.BackRightTire.Location = new System.Drawing.Point(353, 309);
            this.BackRightTire.Name = "BackRightTire";
            this.BackRightTire.ReadOnly = true;
            this.BackRightTire.Size = new System.Drawing.Size(45, 20);
            this.BackRightTire.TabIndex = 8;
            // 
            // ECUForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 403);
            this.Controls.Add(this.BackRightTire);
            this.Controls.Add(this.BackLeftTire);
            this.Controls.Add(this.FrontRightTire);
            this.Controls.Add(this.FrontLeftTire);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.brakeBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextView);
            this.Name = "ECUForm";
            this.Text = "ECUForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button brakeBtn;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FrontLeftTire;
        private System.Windows.Forms.TextBox FrontRightTire;
        private System.Windows.Forms.TextBox BackLeftTire;
        private System.Windows.Forms.TextBox BackRightTire;
    }
}

