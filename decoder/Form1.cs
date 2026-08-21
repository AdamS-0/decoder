/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 20.08.2026
 * Time: 11:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using VisBox;

namespace decoder {
    /// <summary>
    /// Description of Form1.
    /// </summary>
    public partial class Form1 : Form {
        
        Decoder _decCurrent = null;
        
        public Form1() {
            InitializeComponent();

        }
        
        
        public void BindDecoder(Decoder dec) {
            _decCurrent = dec;
            structInfoView1.Nodes.Clear();
            
            foreach ( DecoderField decField in dec._fields ) {
                
                TreeDetailsViewItem tdvi = structInfoView1.AddNode( decField.Name );
                tdvi.AddSubitem( decField.GetTypeAsString() );
                tdvi.AddSubitem( decField.GetSizeInBits().ToString() );
                
                if ( decField.NoElements > 1 ) {
                    for( int elId = 0; elId < decField.NoElements; elId++ ) {
                        /* Iterate by every element in array */
                        TreeDetailsViewItem tdvsn = tdvi.AddSubNode( decField.Name + "[" + elId + "]" );
                        tdvsn.AddSubitem( decField.GetElementTypeAsString() );
                        tdvsn.AddSubitem( decField.SizeBits.ToString() );
                        TreeDetailsViewSubItem tdvsnsi = tdvsn.AddSubitem( "[...]" );
                        tdvsnsi.ShowRelatedObject = true;
                        tdvsnsi.Tag = elId;
                        tdvsnsi.GetRelatedObject = () => ((object[])decField.Value)[ (int)tdvsnsi.Tag ];
                        
                        TreeDetailsViewSubItem tdvsnsi_raw = tdvsn.AddSubitem( "[...]" );
                        tdvsnsi_raw.ShowRelatedObject = true;
                        tdvsnsi_raw.Tag = elId;
                        tdvsnsi_raw.GetRelatedObject = delegate() {
                            try {
                                object obj = ((object[])decField.Value)[ (int)tdvsnsi.Tag ];
                                return "0x" + ((UInt64)obj).ToString("X8");
                            } catch {}
                            return "---";
                        };
                    }
                } else {
                    TreeDetailsViewSubItem tdvsi = tdvi.AddSubitem( "[...]" );
                    tdvsi.ShowRelatedObject = true;
                    tdvsi.GetRelatedObject = () => decField.Value;
                    
                    TreeDetailsViewSubItem tdvsnsi_raw = tdvi.AddSubitem( "[...]" );
                    tdvsnsi_raw.ShowRelatedObject = true;
                    tdvsnsi_raw.GetRelatedObject = delegate() {
                        try {
                            object obj = decField.Value;
                            return "0x" + ((UInt64)obj).ToString("X8");
                        } catch {}
                        return "---";
                    };
                }
            }
            structInfoView1.Refresh();
            
        }
        
        
        void RtbRawDataTextChanged ( object sender, EventArgs e ) {
            TryParseRawData();
        }
        
        
        
        private void TryParseRawData() {
            try {
                if ( _decCurrent == null ) { 
                    /* Decoder not defined */
                    return;
                }
                
                string strData = rtbRawData.Text;
                
                strData = strData.Replace(" 0x", " ");
                strData = strData.Replace(" ", "");
                strData = strData.Replace("\n", "");
                strData = strData.Replace("\r", "");
                strData = strData.Replace("\t", "");
                
                Byte[] dataBytes = StringToByteArray(strData);
                _decCurrent.TryDecode( dataBytes );
                structInfoView1.Refresh();
            } catch (Exception ex) {
                int dummyInt = 0;
            }
        }
        
        static byte[] StringToByteArray(string hex) {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            
            try {
                for (int i = 0; i < length; i += 2) {
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                }
            } catch {}
            return bytes;
        }
        
        
        VisBox.PushButtonManager _pushBtnMngr_decoder = new PushButtonManager();
        private static int _pushBtnTotCntr = 0;
        public void AddDecoder(ref Decoder dec) {
            
            VisBox.PushButton pushBtn = new PushButton();
            pushBtn.Text = dec.Name.Length == 0 ? ( "pattern" + (++_pushBtnTotCntr).ToString() ) : dec.Name;
            pushBtn.Tag = dec;
            pushBtn.Dock = DockStyle.Top;
            pushBtn.ColorButton = SystemColors.Control;
            pushBtn.Height = 20;
            pushBtn.Click += delegate (object sender, EventArgs e) {
                if ( pushBtn.Tag != null ) {
                    try {
                        this.BindDecoder( (Decoder)(pushBtn.Tag) );
                        TryParseRawData();
                    } catch {}
                }
            };
            
            _pushBtnMngr_decoder.AddButton( pushBtn );
            pushBtn.PushButtonManager = _pushBtnMngr_decoder;
            
            grpBxPatterns.Controls.Add( pushBtn );
            BindDecoder( dec );
            _pushBtnMngr_decoder.OneElementChanged( pushBtn );
        }
        
        
    }
}
