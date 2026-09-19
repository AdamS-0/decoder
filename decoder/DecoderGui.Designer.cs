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
        private System.Windows.Forms.ToolStripStatusLabel tlStrpLbl;
        private System.Windows.Forms.ToolStripDropDownButton tlStpMenuPatterns;
        private System.Windows.Forms.ToolStripMenuItem tlStrpBtnPatternNew;
        private System.Windows.Forms.ToolStripMenuItem tlStrpBtnPatternRebuild;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tlStrpBtnPatternRemove;
        private System.Windows.Forms.ContextMenuStrip ctxMnTreeDetailView;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rebuildPatternToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton tlStrpMenuData;
        private System.Windows.Forms.ToolStripMenuItem littleEndianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tlStrpDataHighlightFieldsBytes;
        private System.Windows.Forms.TreeView trVwPatterns;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel tlStrpLblStatus;
        
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
            this.components = new System.ComponentModel.Container();
            VisBox.TreeDetailsViewColumnItem treeDetailsViewColumnItem1 = new VisBox.TreeDetailsViewColumnItem();
            VisBox.TreeDetailsViewColumnItem treeDetailsViewColumnItem2 = new VisBox.TreeDetailsViewColumnItem();
            VisBox.TreeDetailsViewColumnItem treeDetailsViewColumnItem3 = new VisBox.TreeDetailsViewColumnItem();
            VisBox.TreeDetailsViewColumnItem treeDetailsViewColumnItem4 = new VisBox.TreeDetailsViewColumnItem();
            VisBox.TreeDetailsViewColumnItem treeDetailsViewColumnItem5 = new VisBox.TreeDetailsViewColumnItem();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DecoderGui));
            this.structInfoView1 = new VisBox.TreeDetailsView();
            this.ctxMnTreeDetailView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebuildPatternToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pMain = new System.Windows.Forms.Panel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.grpBxRawData = new System.Windows.Forms.GroupBox();
            this.rtbRawData = new System.Windows.Forms.RichTextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.grpBxPatterns = new System.Windows.Forms.GroupBox();
            this.trVwPatterns = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tlStpMenuPatterns = new System.Windows.Forms.ToolStripDropDownButton();
            this.tlStrpBtnPatternNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tlStrpBtnPatternRebuild = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tlStrpBtnPatternRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.tlStrpMenuData = new System.Windows.Forms.ToolStripDropDownButton();
            this.littleEndianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tlStrpDataHighlightFieldsBytes = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tlStrpLbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlStrpLblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ctxMnTreeDetailView.SuspendLayout();
            this.pMain.SuspendLayout();
            this.grpBxRawData.SuspendLayout();
            this.grpBxPatterns.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // structInfoView1
            // 
            treeDetailsViewColumnItem1.Font = new System.Drawing.Font("Arial", 10F);
            treeDetailsViewColumnItem1.ForeColor = System.Drawing.Color.Black;
            treeDetailsViewColumnItem1.SelectedBackColor = System.Drawing.Color.Gainsboro;
            treeDetailsViewColumnItem1.Text = "Name";
            treeDetailsViewColumnItem1.Width = 95;
            treeDetailsViewColumnItem2.Font = new System.Drawing.Font("Arial", 10F);
            treeDetailsViewColumnItem2.ForeColor = System.Drawing.Color.Black;
            treeDetailsViewColumnItem2.SelectedBackColor = System.Drawing.Color.Gainsboro;
            treeDetailsViewColumnItem2.Text = "Type";
            treeDetailsViewColumnItem2.Width = 80;
            treeDetailsViewColumnItem3.Font = new System.Drawing.Font("Arial", 10F);
            treeDetailsViewColumnItem3.ForeColor = System.Drawing.Color.Black;
            treeDetailsViewColumnItem3.SelectedBackColor = System.Drawing.Color.Gainsboro;
            treeDetailsViewColumnItem3.Text = "Bits";
            treeDetailsViewColumnItem3.Width = 35;
            treeDetailsViewColumnItem4.Font = new System.Drawing.Font("Arial", 10F);
            treeDetailsViewColumnItem4.ForeColor = System.Drawing.Color.Black;
            treeDetailsViewColumnItem4.SelectedBackColor = System.Drawing.Color.Gainsboro;
            treeDetailsViewColumnItem4.Text = "Value";
            treeDetailsViewColumnItem4.Width = 75;
            treeDetailsViewColumnItem5.Font = new System.Drawing.Font("Arial", 10F);
            treeDetailsViewColumnItem5.ForeColor = System.Drawing.Color.Black;
            treeDetailsViewColumnItem5.SelectedBackColor = System.Drawing.Color.Gainsboro;
            treeDetailsViewColumnItem5.Text = "Raw";
            treeDetailsViewColumnItem5.Width = 95;
            this.structInfoView1.Columns.Add(treeDetailsViewColumnItem1);
            this.structInfoView1.Columns.Add(treeDetailsViewColumnItem2);
            this.structInfoView1.Columns.Add(treeDetailsViewColumnItem3);
            this.structInfoView1.Columns.Add(treeDetailsViewColumnItem4);
            this.structInfoView1.Columns.Add(treeDetailsViewColumnItem5);
            this.structInfoView1.ContextMenuStrip = this.ctxMnTreeDetailView;
            this.structInfoView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.structInfoView1.FlatStyle = VisBox.VisElement.VisFlatStyle.Collapsed;
            this.structInfoView1.Location = new System.Drawing.Point(138, 106);
            this.structInfoView1.Name = "structInfoView1";
            this.structInfoView1.Size = new System.Drawing.Size(403, 245);
            this.structInfoView1.TabIndex = 0;
            this.structInfoView1.Text = "structInfoView1";
            this.structInfoView1.TreeLine = null;
            this.structInfoView1.Click += new System.EventHandler(this.StructInfoView1Click);
            this.structInfoView1.Paint += new System.Windows.Forms.PaintEventHandler(this.StructInfoView1Paint);
            // 
            // ctxMnTreeDetailView
            // 
            this.ctxMnTreeDetailView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expandAllToolStripMenuItem,
            this.collapseAllToolStripMenuItem,
            this.rebuildPatternToolStripMenuItem});
            this.ctxMnTreeDetailView.Name = "ctxMnTreeDetailView";
            this.ctxMnTreeDetailView.Size = new System.Drawing.Size(159, 70);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.expandAllToolStripMenuItem.Text = "Expand all";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.ExpandAllToolStripMenuItemClick);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.collapseAllToolStripMenuItem.Text = "Collapse all";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.CollapseAllToolStripMenuItemClick);
            // 
            // rebuildPatternToolStripMenuItem
            // 
            this.rebuildPatternToolStripMenuItem.Name = "rebuildPatternToolStripMenuItem";
            this.rebuildPatternToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.rebuildPatternToolStripMenuItem.Text = "Rebuild pattern";
            this.rebuildPatternToolStripMenuItem.Click += new System.EventHandler(this.TlStrpBtnRebuildPatternsClick);
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
            this.rtbRawData.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
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
            this.grpBxPatterns.Controls.Add(this.trVwPatterns);
            this.grpBxPatterns.Dock = System.Windows.Forms.DockStyle.Left;
            this.grpBxPatterns.Location = new System.Drawing.Point(3, 3);
            this.grpBxPatterns.Name = "grpBxPatterns";
            this.grpBxPatterns.Size = new System.Drawing.Size(132, 348);
            this.grpBxPatterns.TabIndex = 1;
            this.grpBxPatterns.TabStop = false;
            this.grpBxPatterns.Text = "Patterns";
            // 
            // trVwPatterns
            // 
            this.trVwPatterns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trVwPatterns.Location = new System.Drawing.Point(3, 16);
            this.trVwPatterns.Name = "trVwPatterns";
            this.trVwPatterns.Size = new System.Drawing.Size(126, 329);
            this.trVwPatterns.TabIndex = 0;
            this.trVwPatterns.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TrVwPatternsAfterSelect);
            this.trVwPatterns.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TrVwPatternsNodeMouseClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlStpMenuPatterns,
            this.tlStrpMenuData});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(544, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tlStpMenuPatterns
            // 
            this.tlStpMenuPatterns.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlStrpBtnPatternNew,
            this.tlStrpBtnPatternRebuild,
            this.toolStripSeparator1,
            this.tlStrpBtnPatternRemove});
            this.tlStpMenuPatterns.Name = "tlStpMenuPatterns";
            this.tlStpMenuPatterns.Size = new System.Drawing.Size(59, 22);
            this.tlStpMenuPatterns.Text = "Pattern";
            // 
            // tlStrpBtnPatternNew
            // 
            this.tlStrpBtnPatternNew.Name = "tlStrpBtnPatternNew";
            this.tlStrpBtnPatternNew.Size = new System.Drawing.Size(199, 22);
            this.tlStrpBtnPatternNew.Text = "New pattern";
            this.tlStrpBtnPatternNew.Click += new System.EventHandler(this.NewPatternToolStripMenuItemClick);
            // 
            // tlStrpBtnPatternRebuild
            // 
            this.tlStrpBtnPatternRebuild.Name = "tlStrpBtnPatternRebuild";
            this.tlStrpBtnPatternRebuild.Size = new System.Drawing.Size(199, 22);
            this.tlStrpBtnPatternRebuild.Text = "Rebuild current pattern";
            this.tlStrpBtnPatternRebuild.Click += new System.EventHandler(this.TlStrpBtnRebuildPatternsClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(196, 6);
            // 
            // tlStrpBtnPatternRemove
            // 
            this.tlStrpBtnPatternRemove.Name = "tlStrpBtnPatternRemove";
            this.tlStrpBtnPatternRemove.Size = new System.Drawing.Size(199, 22);
            this.tlStrpBtnPatternRemove.Text = "Remove this pattern";
            this.tlStrpBtnPatternRemove.Click += new System.EventHandler(this.TlStrpBtnRemovePatternClick);
            // 
            // tlStrpMenuData
            // 
            this.tlStrpMenuData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tlStrpMenuData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.littleEndianToolStripMenuItem,
            this.tlStrpDataHighlightFieldsBytes});
            this.tlStrpMenuData.Image = ((System.Drawing.Image)(resources.GetObject("tlStrpMenuData.Image")));
            this.tlStrpMenuData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tlStrpMenuData.Name = "tlStrpMenuData";
            this.tlStrpMenuData.Size = new System.Drawing.Size(46, 22);
            this.tlStrpMenuData.Text = "Data";
            // 
            // littleEndianToolStripMenuItem
            // 
            this.littleEndianToolStripMenuItem.Checked = true;
            this.littleEndianToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.littleEndianToolStripMenuItem.Name = "littleEndianToolStripMenuItem";
            this.littleEndianToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.littleEndianToolStripMenuItem.Text = "LittleEndian";
            // 
            // tlStrpDataHighlightFieldsBytes
            // 
            this.tlStrpDataHighlightFieldsBytes.Checked = true;
            this.tlStrpDataHighlightFieldsBytes.CheckOnClick = true;
            this.tlStrpDataHighlightFieldsBytes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tlStrpDataHighlightFieldsBytes.Name = "tlStrpDataHighlightFieldsBytes";
            this.tlStrpDataHighlightFieldsBytes.Size = new System.Drawing.Size(190, 22);
            this.tlStrpDataHighlightFieldsBytes.Text = "Highlight field\'s bytes";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlStrpLbl,
            this.toolStripStatusLabel1,
            this.tlStrpLblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 379);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(544, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tlStrpLbl
            // 
            this.tlStrpLbl.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tlStrpLbl.Name = "tlStrpLbl";
            this.tlStrpLbl.Size = new System.Drawing.Size(76, 17);
            this.tlStrpLbl.Text = "Status: done";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(328, 17);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // tlStrpLblStatus
            // 
            this.tlStrpLblStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tlStrpLblStatus.Name = "tlStrpLblStatus";
            this.tlStrpLblStatus.Size = new System.Drawing.Size(125, 17);
            this.tlStrpLblStatus.Text = "toolStripStatusLabel2";
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
            this.ctxMnTreeDetailView.ResumeLayout(false);
            this.pMain.ResumeLayout(false);
            this.grpBxRawData.ResumeLayout(false);
            this.grpBxPatterns.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
