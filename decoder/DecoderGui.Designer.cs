/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 20.08.2026
 * Time: 11:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace decoder
{
    partial class DecoderGui
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private VisBox.TreeDetailsView structInfoView1;
        private System.Windows.Forms.Panel pMain;
        private System.Windows.Forms.GroupBox grpBxRawData;
        private System.Windows.Forms.GroupBox grpBxPatterns;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.RichTextBox rtbRawData;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        
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
            VisBox.TreeDetailsViewColumnItem treeDetailsViewColumnItem1 = new VisBox.TreeDetailsViewColumnItem();
            VisBox.TreeDetailsViewColumnItem treeDetailsViewColumnItem2 = new VisBox.TreeDetailsViewColumnItem();
            VisBox.TreeDetailsViewColumnItem treeDetailsViewColumnItem3 = new VisBox.TreeDetailsViewColumnItem();
            VisBox.TreeDetailsViewColumnItem treeDetailsViewColumnItem4 = new VisBox.TreeDetailsViewColumnItem();
            VisBox.TreeDetailsViewColumnItem treeDetailsViewColumnItem5 = new VisBox.TreeDetailsViewColumnItem();
            this.structInfoView1 = new VisBox.TreeDetailsView();
            this.pMain = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.grpBxRawData = new System.Windows.Forms.GroupBox();
            this.rtbRawData = new System.Windows.Forms.RichTextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.grpBxPatterns = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pMain.SuspendLayout();
            this.grpBxRawData.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // structInfoView1
            // 
            treeDetailsViewColumnItem1.Font = new System.Drawing.Font("Arial", 10F);
            treeDetailsViewColumnItem1.ForeColor = System.Drawing.Color.Black;
            treeDetailsViewColumnItem1.Text = "Name";
            treeDetailsViewColumnItem1.Width = 95;
            treeDetailsViewColumnItem2.Font = new System.Drawing.Font("Arial", 10F);
            treeDetailsViewColumnItem2.ForeColor = System.Drawing.Color.Black;
            treeDetailsViewColumnItem2.Text = "Type";
            treeDetailsViewColumnItem2.Width = 80;
            treeDetailsViewColumnItem3.Font = new System.Drawing.Font("Arial", 10F);
            treeDetailsViewColumnItem3.ForeColor = System.Drawing.Color.Black;
            treeDetailsViewColumnItem3.Text = "Bits";
            treeDetailsViewColumnItem3.Width = 35;
            treeDetailsViewColumnItem4.Font = new System.Drawing.Font("Arial", 10F);
            treeDetailsViewColumnItem4.ForeColor = System.Drawing.Color.Black;
            treeDetailsViewColumnItem4.Text = "Value";
            treeDetailsViewColumnItem4.Width = 75;
            treeDetailsViewColumnItem5.Font = new System.Drawing.Font("Arial", 10F);
            treeDetailsViewColumnItem5.ForeColor = System.Drawing.Color.Black;
            treeDetailsViewColumnItem5.Text = "Raw";
            treeDetailsViewColumnItem5.Width = 95;
            this.structInfoView1.Columns.Add(treeDetailsViewColumnItem1);
            this.structInfoView1.Columns.Add(treeDetailsViewColumnItem2);
            this.structInfoView1.Columns.Add(treeDetailsViewColumnItem3);
            this.structInfoView1.Columns.Add(treeDetailsViewColumnItem4);
            this.structInfoView1.Columns.Add(treeDetailsViewColumnItem5);
            this.structInfoView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.structInfoView1.FlatStyle = VisBox.VisElement.VisFlatStyle.Collapsed;
            this.structInfoView1.Location = new System.Drawing.Point(138, 106);
            this.structInfoView1.Name = "structInfoView1";
            this.structInfoView1.Size = new System.Drawing.Size(403, 245);
            this.structInfoView1.TabIndex = 0;
            this.structInfoView1.Text = "structInfoView1";
            this.structInfoView1.TreeLine = null;
            // 
            // pMain
            // 
            this.pMain.Controls.Add(this.structInfoView1);
            this.pMain.Controls.Add(this.splitter2);
            this.pMain.Controls.Add(this.grpBxRawData);
            this.pMain.Controls.Add(this.splitter1);
            this.pMain.Controls.Add(this.grpBxPatterns);
            this.pMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pMain.Location = new System.Drawing.Point(0, 25);
            this.pMain.Name = "pMain";
            this.pMain.Padding = new System.Windows.Forms.Padding(3);
            this.pMain.Size = new System.Drawing.Size(544, 354);
            this.pMain.TabIndex = 1;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(138, 103);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(403, 3);
            this.splitter2.TabIndex = 4;
            this.splitter2.TabStop = false;
            // 
            // grpBxRawData
            // 
            this.grpBxRawData.Controls.Add(this.rtbRawData);
            this.grpBxRawData.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpBxRawData.Location = new System.Drawing.Point(138, 3);
            this.grpBxRawData.Name = "grpBxRawData";
            this.grpBxRawData.Size = new System.Drawing.Size(403, 100);
            this.grpBxRawData.TabIndex = 2;
            this.grpBxRawData.TabStop = false;
            this.grpBxRawData.Text = "Raw data";
            // 
            // rtbRawData
            // 
            this.rtbRawData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbRawData.Location = new System.Drawing.Point(3, 16);
            this.rtbRawData.Name = "rtbRawData";
            this.rtbRawData.Size = new System.Drawing.Size(397, 81);
            this.rtbRawData.TabIndex = 0;
            this.rtbRawData.Text = "";
            this.rtbRawData.TextChanged += new System.EventHandler(this.RtbRawDataTextChanged);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(135, 3);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 348);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // grpBxPatterns
            // 
            this.grpBxPatterns.Dock = System.Windows.Forms.DockStyle.Left;
            this.grpBxPatterns.Location = new System.Drawing.Point(3, 3);
            this.grpBxPatterns.Name = "grpBxPatterns";
            this.grpBxPatterns.Size = new System.Drawing.Size(132, 348);
            this.grpBxPatterns.TabIndex = 1;
            this.grpBxPatterns.TabStop = false;
            this.grpBxPatterns.Text = "Patterns";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(544, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 379);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(544, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(125, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // DecoderGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 401);
            this.Controls.Add(this.pMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = global::decoder.Resource1.icon1;
            this.Name = "DecoderGui";
            this.Text = "DecoderGui";
            this.pMain.ResumeLayout(false);
            this.grpBxRawData.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
