namespace eSSP_example
{
    partial class frmSetLevel
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
            this.cbChannels = new System.Windows.Forms.ComboBox();
            this.tbValue = new System.Windows.Forms.TextBox();
            this.btnDone = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbChannels
            // 
            this.cbChannels.FormattingEnabled = true;
            this.cbChannels.Location = new System.Drawing.Point(12, 24);
            this.cbChannels.Name = "cbChannels";
            this.cbChannels.Size = new System.Drawing.Size(108, 21);
            this.cbChannels.TabIndex = 0;
            // 
            // tbValue
            // 
            this.tbValue.Location = new System.Drawing.Point(126, 24);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(72, 20);
            this.tbValue.TabIndex = 1;
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(12, 51);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(186, 23);
            this.btnDone.TabIndex = 2;
            this.btnDone.Text = "Set Level";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Channel";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(131, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Value";
            // 
            // frmSetLevel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(210, 86);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.cbChannels);
            this.Name = "frmSetLevel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Set Level";
            this.Load += new System.EventHandler(this.frmSetLevel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbChannels;
        private System.Windows.Forms.TextBox tbValue;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}