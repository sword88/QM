namespace QM.Demo.RFC
{
    partial class PO
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
            this.button1 = new System.Windows.Forms.Button();
            this.sdate = new System.Windows.Forms.DateTimePicker();
            this.edate = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(255, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "CLOSE PO";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // sdate
            // 
            this.sdate.CustomFormat = "yyyyMMdd";
            this.sdate.Location = new System.Drawing.Point(25, 26);
            this.sdate.MinDate = new System.DateTime(2012, 1, 1, 0, 0, 0, 0);
            this.sdate.Name = "sdate";
            this.sdate.Size = new System.Drawing.Size(200, 21);
            this.sdate.TabIndex = 1;
            // 
            // edate
            // 
            this.edate.CustomFormat = "yyyyMMdd";
            this.edate.Location = new System.Drawing.Point(25, 53);
            this.edate.MinDate = new System.DateTime(2012, 1, 1, 0, 0, 0, 0);
            this.edate.Name = "edate";
            this.edate.Size = new System.Drawing.Size(200, 21);
            this.edate.TabIndex = 2;
            // 
            // PO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 93);
            this.Controls.Add(this.edate);
            this.Controls.Add(this.sdate);
            this.Controls.Add(this.button1);
            this.Name = "PO";
            this.Text = "PO";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker sdate;
        private System.Windows.Forms.DateTimePicker edate;
    }
}