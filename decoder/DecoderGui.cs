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
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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
                tlStrpLbl.Text = "Item: " + node.Text + " @ " + node.Tag3 + " bits, offset: " + node.Tag2 + " bits";
                
                if ( !tlStrpDataHighlightFieldsBytes.Checked ) { return; }
                rtbRawData.SuspendDrawing();
                
                int bitOffsetStart = (int)node.Tag2;
                int bitOffsetStop = bitOffsetStart + (int)node.Tag3 - 1;
                
                int byteId = 0;
                int bitId = 0;
                bool isMsbNibble = true;
                
                bitOffsetStart  = ( ( bitOffsetStart ) / 4 ) * 4;
                bitOffsetStop   = ( ( bitOffsetStop ) / 4 ) * 4;
                
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
            rtbRawData.ResumeDrawing();
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
            structInfoView1.SuspendDrawing();
            structInfoView1.Nodes.Clear();
            int bitOffset = 0;
            BindDecoderSub( dec, ref bitOffset );
            structInfoView1.ResumeDrawing();
            structInfoView1.Refresh();
        }
        
        
        void RtbRawDataTextChanged ( object sender, EventArgs e ) {
            TryParseRawData();
        }
        
        public void SetRawDataAndTryParse(string strRawData) {
            rtbRawData.Text = strRawData;
        }
        
        
        
        private static readonly Regex _rgxOneNibbles = new Regex(@" ([0-9a-fA-F]) ", RegexOptions.Compiled);
        public void TryParseRawData() {
            try {
                if ( _decCurrent == null ) { 
                    /* Decoder not defined */
                    return;
                }
                
                string strData = rtbRawData.Text;
                
                strData = strData.Replace(" 0x", " ");
                strData = strData.Replace("\n", "");
                strData = strData.Replace("\r", "");
                strData = strData.Replace("\t", "");
                strData = _rgxOneNibbles.Replace( strData, "0$1" );
                strData = strData.Replace(" ", "");
                
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
            try {
                _decCurrent.RebuildPattern();
                BindDecoder( _decCurrent );
            } catch {}
        }
        
        
        void TlStrpBtnRemovePatternClick ( object sender, EventArgs e ) {
            if ( _decCurrent == null ) { return; }
            for ( int idx = 0; idx < grpBxPatterns.Controls.Count; idx++ ) {
                PushButton pBtn = grpBxPatterns.Controls[idx] as PushButton;
                if ( pBtn.Tag.Equals( _decCurrent ) ) {
                    pBtn.Dispose();
                    break;
                }
            }
            _decoders.Remove( _decCurrent );
            _decCurrent = _decoders[0];
            BindDecoder( _decCurrent );
        }
        
        void NewPatternToolStripMenuItemClick ( object sender, EventArgs e ) {
            formNewPattern fNewPattern = new formNewPattern();
            fNewPattern.NewPattern += newPatternFromForm;
            fNewPattern.Show();
        }
        
        private void newPatternFromForm(string newPattern, string patternName) {
            
            Decoder dec = new Decoder( newPattern );
            if ( dec != null ) {
                AddDecoder( ref dec );
                TryParseRawData();
            }
        }
        
        void StructInfoView1Click ( object sender, EventArgs e ) {
            
        }
        
        void ExpandAllToolStripMenuItemClick ( object sender, EventArgs e ) {
            ChangeCollapseNodes( false );
        }
        
        void CollapseAllToolStripMenuItemClick ( object sender, EventArgs e ) {
            ChangeCollapseNodes( true );
        }
        
        private void ChangeCollapseNodes(bool collapse) {
            if ( _decCurrent == null ) { return; }
            
            foreach ( TreeDetailsViewItem node in structInfoView1.Nodes ) {
                changeCollapseSubNodes( node, collapse );
            }
            structInfoView1.NumberOfVisibleNodesChanged();
        }
        
        private void changeCollapseSubNodes( TreeDetailsViewItem node, bool collapse ) {
            node.Collapsed = collapse;
            foreach ( TreeDetailsViewItem subNode in node.Nodes ) {
                changeCollapseSubNodes( subNode, collapse );
            }
        }
        
        private int _cntrPaints = 0;
        void StructInfoView1Paint ( object sender, PaintEventArgs e ) {
            _cntrPaints++;
            Text = "Pains: " + _cntrPaints;
        }

        
        
    }
    
    
    
    public static class ControlExtensions {
        [DllImport( "user32.dll")]
        private static extern IntPtr SendMessage ( IntPtr hWnd, int msg, IntPtr wp, IntPtr lp );

        private const int WM_SETREDRAW = 0x000B;

        /// <summary>
        /// Zamraża odświeżanie graficzne kontrolki.
        /// </summary>
        public static void SuspendDrawing ( this Control control ) {
            if ( control != null && !control.IsDisposed && control.IsHandleCreated ) {
                SendMessage( control.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero );
            }
        }

        /// <summary>
        /// Odmraża odświeżanie graficzne kontrolki i wymusza jej przerysowanie.
        /// </summary>
        public static void ResumeDrawing ( this Control control ) {
            if ( control != null && !control.IsDisposed && control.IsHandleCreated ) {
                SendMessage( control.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero );
                control.Refresh(); // Odświeża natychmiast całą kontrolkę wraz z dziećmi
            }
        }
    }
}
