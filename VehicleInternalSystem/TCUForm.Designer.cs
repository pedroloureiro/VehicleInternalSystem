namespace VehicleInternalSystem
{
    partial class TCUForm
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
            this.connectBtn = new System.Windows.Forms.Button();
            this.FrontRightTire = new System.Windows.Forms.NumericUpDown();
            this.FrontLeftTire = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BackLeftTire = new System.Windows.Forms.NumericUpDown();
            this.BackRightTire = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.FrontRightTire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FrontLeftTire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackLeftTire)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackRightTire)).BeginInit();
            this.SuspendLayout();
            // 
            // TextView
            // 
            this.TextView.HideSelection = false;
            this.TextView.Location = new System.Drawing.Point(12, 12);
            this.TextView.Multiline = true;
            this.TextView.Name = "TextView";
            this.TextView.ReadOnly = true;
            this.TextView.Size = new System.Drawing.Size(355, 118);
            this.TextView.TabIndex = 0;
            // 
            // connectBtn
            // 
            this.connectBtn.Location = new System.Drawing.Point(160, 136);
            this.connectBtn.Name = "connectBtn";
            this.connectBtn.Size = new System.Drawing.Size(75, 23);
            this.connectBtn.TabIndex = 1;
            this.connectBtn.Text = "connect";
            this.connectBtn.UseVisualStyleBackColor = true;
            this.connectBtn.Click += new System.EventHandler(this.connectBtn_Click);
            // 
            // FrontRightTire
            // 
            this.FrontRightTire.Location = new System.Drawing.Point(191, 202);
            this.FrontRightTire.Name = "FrontRightTire";
            this.FrontRightTire.Size = new System.Drawing.Size(63, 20);
            this.FrontRightTire.TabIndex = 2;
            this.FrontRightTire.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // FrontLeftTire
            // 
            this.FrontLeftTire.Location = new System.Drawing.Point(106, 202);
            this.FrontLeftTire.Name = "FrontLeftTire";
            this.FrontLeftTire.Size = new System.Drawing.Size(63, 20);
            this.FrontLeftTire.TabIndex = 3;
            this.FrontLeftTire.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(103, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Front Left";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(188, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Front Right";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(103, 243);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Back Left";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(188, 243);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Back Right";
            // 
            // BackLeftTire
            // 
            this.BackLeftTire.Location = new System.Drawing.Point(106, 259);
            this.BackLeftTire.Name = "BackLeftTire";
            this.BackLeftTire.Size = new System.Drawing.Size(63, 20);
            this.BackLeftTire.TabIndex = 8;
            this.BackLeftTire.Value = new decimal(new int[] {
            65,
            0,
            0,
            0});
            // 
            // BackRightTire
            // 
            this.BackRightTire.Location = new System.Drawing.Point(191, 259);
            this.BackRightTire.Name = "BackRightTire";
            this.BackRightTire.Size = new System.Drawing.Size(63, 20);
            this.BackRightTire.TabIndex = 9;
            this.BackRightTire.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // TCUForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 353);
            this.Controls.Add(this.BackRightTire);
            this.Controls.Add(this.BackLeftTire);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FrontLeftTire);
            this.Controls.Add(this.FrontRightTire);
            this.Controls.Add(this.connectBtn);
            this.Controls.Add(this.TextView);
            this.Name = "TCUForm";
            this.Text = "TCUForm";
            ((System.ComponentModel.ISupportInitialize)(this.FrontRightTire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FrontLeftTire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackLeftTire)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BackRightTire)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextView;
        private System.Windows.Forms.Button connectBtn;
        private System.Windows.Forms.NumericUpDown FrontRightTire;
        private System.Windows.Forms.NumericUpDown FrontLeftTire;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown BackLeftTire;
        private System.Windows.Forms.NumericUpDown BackRightTire;
    }
}