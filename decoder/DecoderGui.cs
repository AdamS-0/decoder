/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 20.08.2026
 * Time: 11:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VisBox;

namespace decoder {
    /// <summary>
    /// Description of Form1.
    /// </summary>
    public partial class DecoderGui : Form {
        
        public List<Decoder> _decoders = new List<Decoder>();
        
        public Decoder _decCurrent = null;
        
        public DecoderGui() {
            InitializeComponent();
            structInfoView1.ItemMouseEnter += ItemMouseEnter;
        }
        
        private void ItemMouseEnter( object sender, EventArgs e ) {
            TreeDetailsViewItem node = sender as TreeDetailsViewItem;
            try {
                Text = node.Text + ", BitOffset: " + node.Tag2 + ", bitSize: " + node.Tag3;
            } catch {
                Text = node.Text;
            }
            
            try {
                int bitOffsetStart = (int)node.Tag2;
                int bitOffsetStop = bitOffsetStart + (int)node.Tag3 - 1;
                
                int byteId = 0;
                int bitId = 0;
                char lastCorrectChar = ' ';
                bool isMsbNibble = true;
                
                bitOffsetStart  = ( ( bitOffsetStart ) / 4 ) * 4;
                bitOffsetStop   = ( ( bitOffsetStop ) / 4 ) * 4;
                
                Text = node.Text + ", BitOffset: " + node.Tag2 + ", bitSize: " + node.Tag3 + "/" + "Start: " + bitOffsetStart + ", Stop: " + bitOffsetStop;
                
                int idxCorrectCharLast = -1;
                
                rtbRawData.Select( 0, rtbRawData.Text.Length );
                rtbRawData.SelectionColor = SystemColors.WindowText;
                
                for ( int idx = 0; idx < rtbRawData.Text.Length; idx++ ) {
                    char c = rtbRawData.Text[idx];
                    if ( !( ( ( c >= '0' ) && ( c <= '9' ) ) ||
                            ( ( c >= 'a' ) && ( c <= 'f' ) ) ||
                            ( ( c >= 'A' ) && ( c <= 'F' ) ) || ( c == ' ' ) ) )
                    {
                        continue;
                    }
                    
                    int idxToColor = 0;
                    if ( c == ' ' ) {
                        if ( !isMsbNibble ) {
                            /* Previously was Lsb nibble! e.g. "AA B_78" */
                            idxToColor = idxCorrectCharLast;
                        } else { idxToColor = -1; }
                        isMsbNibble = true;
                    } else {
                        bitId = byteId * 8 + ( isMsbNibble ? 4 : 0 );
                        idxToColor = idx;
                    }
                    
                    /* found correct nibble */
                    if ( ( idxToColor >= 0 ) && ( idxToColor < rtbRawData.Text.Length ) ) {
                        if ( ( bitId >= bitOffsetStart ) && ( bitId <= bitOffsetStop ) ) {
                            rtbRawData.Select( idxToColor, 1 );
                            rtbRawData.SelectionColor = Color.Red;
                        }
                    }
                    
                    if ( bitId > ( bitOffsetStop + 8 ) ) { break; }
                    
                    if ( c != ' ' ) {
                        isMsbNibble = !isMsbNibble;
                        if ( isMsbNibble ) { byteId++; }
                    }
                    idxCorrectCharLast = idx;
                }
                
                rtbRawData.DeselectAll();
                
            } catch (Exception ex) {}
        }

        private void BindDecoderSub(Decoder dec, ref int bitOffset, TreeDetailsViewItem parentNode = null ) {
            foreach ( DecoderField decField in dec._fields ) {
                TreeDetailsViewItem node = parentNode == null ? structInfoView1.AddNode( decField.Name ) : parentNode.AddSubNode( decField.Name );
                node.AddSubitem( decField.GetTypeAsString() );
                node.AddSubitem( decField.GetTotalSizeInBits().ToString() );
                node.Tag2 = bitOffset;
                node.Tag3 = decField.GetTotalSizeInBits();
                
                if ( ( decField.NoElements > 1 ) && ( decField.Type == DecoderField.DecType.Char ) ) {
                    TreeDetailsViewSubItem subItemValCharArr = node.AddSubitem( "[...]" );
                    subItemValCharArr.ShowRelatedObject = true;
                    subItemValCharArr.GetRelatedObject = delegate() {
                        try {
                            string strCharArr = "\"";
                            for ( int idx = 0; idx < decField.NoElements; idx++ ) {
                                strCharArr += (char)decField.Raw[idx];
                            }
                            return strCharArr + "\"";
                        } catch {}
                        return "---";
                    };
                }
                
                int numberOfNibbles = ( ( decField.NoBitsUsed - 1) / 4 ) + 1;
                for( int elId = 0; elId < decField.NoElements; elId++ ) {
                    TreeDetailsViewItem nodeI = node;
                    if ( decField.NoElements > 1 ) {                        
                        nodeI = node.AddSubNode( decField.Name + "[" + elId + "]" );
                        nodeI.AddSubitem( decField.GetElementTypeAsString() );
                        nodeI.AddSubitem( decField.ElementSizeBits.ToString() );
                        nodeI.Tag3 = (int)decField.ElementSizeBits;
                    
                    }
                    
                    nodeI.Tag2 = bitOffset;
                    
                    if ( decField.Type == DecoderField.DecType.Complex ) {
                        if ( decField.ComplexDecoder != null ) {
                            BindDecoderSub( decField.ComplexDecoder[elId], ref bitOffset, nodeI );
                        }
                    } else {
                        TreeDetailsViewSubItem subItemVal = nodeI.AddSubitem( "[...]" );
                        subItemVal.ShowRelatedObject = true;
                        subItemVal.Tag = elId;
                        subItemVal.GetRelatedObject = () => ((object[])decField.Value)[ (int)subItemVal.Tag ];
                        
                        TreeDetailsViewSubItem subItemRaw = nodeI.AddSubitem( "[...]" );
                        subItemRaw.ShowRelatedObject = true;
                        subItemRaw.Tag = elId;
                        subItemRaw.GetRelatedObject = delegate() {
                            try {
                                object obj = ((ulong[])decField.Raw)[ (int)subItemRaw.Tag ];
                                return "0x" + ((UInt64)obj).ToString("X" + numberOfNibbles);
                            } catch {}
                            return "---";
                        };
                        
                        bitOffset += decField.NoBitsUsed;
                    }
                }

            }
            
        }
        
        public void BindDecoder(Decoder dec) {
            _decCurrent = dec;
            structInfoView1.Nodes.Clear();
            int bitOffset = 0;
            BindDecoderSub( dec, ref bitOffset );
            structInfoView1.Refresh();
            
        }
        
        
        void RtbRawDataTextChanged ( object sender, EventArgs e ) {
            TryParseRawData();
        }
        
        public void SetRawDataAndTryParse(string strRawData) {
            rtbRawData.Text = strRawData;
        }
        
        public void TryParseRawData() {
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
                _decCurrent.TryDecodeData( dataBytes );
                structInfoView1.Refresh();
            } catch (Exception ex) { }
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
            
            _decoders.Add( dec );
            
            BindDecoder( dec );
            _pushBtnMngr_decoder.OneElementChanged( pushBtn );
        }
        
        
        void TlStrpBtnRebuildPatternsClick ( object sender, EventArgs e ) {
            _decCurrent.RebuildPattern();
            BindDecoder( _decCurrent );
        }
        
        
    }
}
