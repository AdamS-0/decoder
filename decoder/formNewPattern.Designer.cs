/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 30.08.2026
 * Time: 17:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace decoder
{
    partial class formNewPattern
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnProceed;
        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.RichTextBox rtbNewPattern;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtBxPatternName;
        private System.Windows.Forms.Label label2;
        
        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnProceed = new System.Windows.Forms.Button();
            this.btnCancle = new System.Windows.Forms.Button();
            this.rtbNewPattern = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBxPatternName = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnProceed);
            this.panel1.Controls.Add(this.btnCancle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 231);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(284, 30);
            this.panel1.TabIndex = 0;
            // 
            // btnProceed
            // 
            this.btnProceed.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnProceed.Location = new System.Drawing.Point(206, 3);
            this.btnProceed.Name = "btnProceed";
            this.btnProceed.Size = new System.Drawing.Size(75, 24);
            this.btnProceed.TabIndex = 0;
            this.btnProceed.Text = "Proceed";
            this.btnProceed.UseVisualStyleBackColor = true;
            this.btnProceed.Click += new System.EventHandler(this.BtnProceedClick);
            // 
            // btnCancle
            // 
            this.btnCancle.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnCancle.Location = new System.Drawing.Point(3, 3);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(75, 24);
            this.btnCancle.TabIndex = 1;
            this.btnCancle.Text = "Cancle";
            this.btnCancle.UseVisualStyleBackColor = true;
            this.btnCancle.Click += new System.EventHandler(this.BtnCancleClick);
            // 
            // rtbNewPattern
            // 
            this.rtbNewPattern.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbNewPattern.Location = new System.Drawing.Point(0, 39);
            this.rtbNewPattern.Name = "rtbNewPattern";
            this.rtbNewPattern.Size = new System.Drawing.Size(284, 192);
            this.rtbNewPattern.TabIndex = 1;
            this.rtbNewPattern.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Paste your pattern below:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtBxPatternName);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(284, 26);
            this.panel2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Pattern name:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtBxPatternName
            // 
            this.txtBxPatternName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBxPatternName.Location = new System.Drawing.Point(76, 3);
            this.txtBxPatternName.Name = "txtBxPatternName";
            this.txtBxPatternName.Size = new System.Drawing.Size(205, 20);
            this.txtBxPatternName.TabIndex = 1;
            // 
            // formNewPattern
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.rtbNewPattern);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Icon = global::decoder.Resource1.icon1;
            this.Name = "formNewPattern";
            this.Text = "Decoder - new pattern";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
