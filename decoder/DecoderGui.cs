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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VisBox;
using System.Security.Cryptography;

namespace decoder {
    /// <summary>
    /// Description of Form1.
    /// </summary>
    public partial class DecoderGui : Form {
        
        dParser _parser = new dParser();
        
        public List<dObject> _decoders = new List<dObject>();
        
        public dObject _decCurrent = null;
        
        public DecoderGui() {
            InitializeComponent();
            structInfoView1.ItemMouseEnter += ItemMouseEnter;
        }
        
        private void ItemMouseEnter( object sender, EventArgs e ) {
            int selStart = rtbRawData.SelectionStart;
            int selLen   = rtbRawData.SelectionLength;
            cntrHighlightChanged++;
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
                
            } catch { }
            rtbRawData.SelectionStart = selStart;
            rtbRawData.SelectionLength = selLen;
            rtbRawData.ResumeDrawing();
        }

        private void BindDecoderSub(dObject decField, ref int bitOffset, TreeDetailsViewItem parentNode = null, int embdLvl = 0 ) {
            cntrBinds++;
            UpdateStats();
            
            int bitOffsetUnion = bitOffset;
            int bitOffsetUnionMax = bitOffset;
            
            /* Print all fields in this object */
            TreeDetailsViewItem node = parentNode == null ? structInfoView1.AddNode( decField.Name ) : parentNode.AddSubNode( decField.Name );
            node.AddSubitem( decField.GetTypeAsString() );
            node.AddSubitem( decField.GetTotalSizeInBits().ToString() );
            node.Tag2 = bitOffset;
            node.Tag3 = decField.GetTotalSizeInBits();
            
            if ( decField.NoElements > 10 ) { node.Collapsed = true; }
            if ( embdLvl > 1 ) { node.Collapsed = true; }
            
            if ( ( decField.NoElements > 1 ) && ( decField.Type == DecType.Char ) ) {
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
                
                if ( ( decField.Type == DecType.Complex ) && !( decField.IsEnum ) ) {
                    if ( decField.Body != null ) {
                        if ( decField.IsUnion ) {
                            for ( int idx = 0; idx < decField.Body[elId].Count; idx++ ) {
                                bitOffset = bitOffsetUnion;
                                BindDecoderSub( decField.Body[elId][idx], ref bitOffset, nodeI, embdLvl + 1 );
                                /* Update max bit offset */
                                if ( bitOffsetUnionMax < bitOffset ) { bitOffsetUnionMax = bitOffset; }
                                else { bitOffset = bitOffsetUnionMax; }
                            }
                        } else {
                            if ( elId < decField.Body.Count ) {
                                for ( int idx = 0; idx < decField.Body[elId].Count; idx++ ) {
                                    BindDecoderSub( decField.Body[elId][idx], ref bitOffset, nodeI, embdLvl + 1 );
                                }
                            }
                        }
                    }
                } else {
                    TreeDetailsViewSubItem subItemVal = nodeI.AddSubitem( "[...]" );
                    subItemVal.ShowRelatedObject = true;
                    subItemVal.Tag = elId;
                    subItemVal.GetRelatedObject = delegate() {
                        if ( decField.Value == null ) { return "---"; }
                        return ((object[])decField.Value)[ (int)subItemVal.Tag ];
                    };
                    
                    TreeDetailsViewSubItem subItemRaw = nodeI.AddSubitem( "[...]" );
                    subItemRaw.ShowRelatedObject = true;
                    subItemRaw.Tag = elId;
                    subItemRaw.GetRelatedObject = delegate() {
                        try {
                            if ( decField.Raw == null ) { return "---"; }
                            object obj = ((ulong[])decField.Raw)[ (int)subItemRaw.Tag ];
                            return "0x" + ((UInt64)obj).ToString("X" + numberOfNibbles);
                        } catch {}
                        return "---";
                    };
                    
                    bitOffset += decField.NoBitsUsed;
                }
            }
            
        }
        
        public void BindDecoder(dObject dec) {
            _decCurrent = dec;
            structInfoView1.SuspendDrawing();
            structInfoView1.Visible = false;
            structInfoView1.Nodes.Clear();
            int bitOffset = 0;
            BindDecoderSub( dec, ref bitOffset );
            structInfoView1.NumberOfVisibleNodesChanged();
            structInfoView1.Visible = true;
            structInfoView1.ResumeDrawing();
            structInfoView1.Refresh();
        }
        
        
        void RtbRawDataTextChanged ( object sender, EventArgs e ) {
            TryDecodeRawData();
        }
        
        public void SetRawDataAndTryParse(string strRawData) {
            rtbRawData.Text = strRawData;
        }
        
        
        
        private static readonly Regex _rgxOneNibbles = new Regex(@" ([0-9a-fA-F]) ", RegexOptions.Compiled);
        string _strLastHash = "";
        public void TryDecodeRawData(bool skipHash = false) {
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
                
                strData = strData.Trim( new char[] { ' ', '\r', '\n', '\t' } );
                
                string strHash = ComputeHash( strData );
                if ( !_strLastHash.Equals( strHash ) || skipHash ) {
                    _strLastHash = strHash;
                    Byte[] dataBytes = StringToByteArray(strData);
                    cntrDecodes++;
                    UpdateStats();
                    _decCurrent.TryDecodeData( dataBytes );
                    structInfoView1.Refresh();
                }
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
        
        private string ComputeHash ( string input ) {
            if ( input == null ) return string.Empty;

            // .NET 4.0 requires explicitly creating and disposing the provider
            using ( SHA256 sha256 = SHA256.Create() ) {
                byte[] inputBytes = Encoding.UTF8.GetBytes( input );
                byte[] hashBytes = sha256.ComputeHash( inputBytes );
            
                // .NET 4.0 alternative to Convert.ToHexString
                StringBuilder sb = new StringBuilder();
                for ( int i = 0; i < hashBytes.Length; i++ ) {
                    sb.Append( hashBytes [ i ].ToString( "X2" ) );
                }
                return sb.ToString();
            }
        }
        
        int _patternCntr = 0;
        public void AddDecoder(ref dObject dec) {
            if ( dec.IsEnum ) {
                TreeNode nodePattern = new TreeNode();
                nodePattern.Text = dec.Name.Length == 0 ? ( "enum pattern" + (++_patternCntr).ToString() ) : dec.Name;                
                trVwPatterns.Nodes.Add( nodePattern );
                for ( int idx = 0; idx < dec.Body[0].Count; idx++ ) {
                    TreeNode nodeEnumI = new TreeNode();
                    nodeEnumI.Text = dec.Body[0][idx].Name + " = " + dec.Body[0][idx].Value;
                    nodePattern.Nodes.Add( nodeEnumI );
                }
            } else if ( dec.IsStruct || dec.IsUnion ) {
                TreeNode nodePattern = new TreeNode();
                
                nodePattern.Text = dec.Name.Length == 0 ? ( "struct pattern" + (++_patternCntr).ToString() ) : dec.Name;
                nodePattern.Tag = dec;
                
                trVwPatterns.Nodes.Add( nodePattern );
                _decoders.Add( dec );
                
                BindDecoder( dec );
            } else {
                for ( int idx = 0; idx < dec.Body[0].Count; idx++ ) {
                    dObject dObj = dec.Body[0][idx];
                    AddDecoder( ref dObj );
                }
            }
        }
        
        
        void TlStrpBtnRebuildPatternsClick ( object sender, EventArgs e ) {
            try {
                _decCurrent.RebuildPattern();
                BindDecoder( _decCurrent );
            } catch {}
        }
        
        
        void TlStrpBtnRemovePatternClick ( object sender, EventArgs e ) {
            if ( _decCurrent == null ) { return; }
            
            TreeNode node = trVwPatterns.SelectedNode;
            if ( node == null ) { return; }
            node.Remove();
            
            try {
                 _parser._ownTypes.Remove(_decCurrent);
            } catch {}

            _decoders.Remove( _decCurrent );
            if ( _decoders.Count > 0 ) {
                _decCurrent = _decoders[0];
                BindDecoder( _decCurrent );
            }
        }
        
        void NewPatternToolStripMenuItemClick ( object sender, EventArgs e ) {
            formNewPattern fNewPattern = new formNewPattern();
            fNewPattern.NewPattern += newPatternFromForm;
            fNewPattern.Show();
        }
        
        private void newPatternFromForm(string newPattern, string patternName) {
            TryParseNewPattern( newPattern, patternName );
        }
        
        public void TryParseNewPattern(string newPattern, string patternName = "") {
            dObject dec = _parser.ParseSource( newPattern );
            if ( dec != null ) {
                AddDecoder( ref dec );
                TryDecodeRawData( skipHash: true );
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
        
        void StructInfoView1Paint ( object sender, PaintEventArgs e ) {
            cntrPaints++;
            UpdateStats();
        }

        void TrVwPatternsNodeMouseClick ( object sender, TreeNodeMouseClickEventArgs e ) {
//            TreeNode node = trVwPatterns.SelectedNode;
//            if ( node == null ) { return; }
//            
//            if ( node.Tag != null ) {
//                try {
//                    this.BindDecoder( (dObject)(node.Tag) );
//                    TryDecodeRawData();
//                } catch {}
//            }
        }
        
        void TrVwPatternsAfterSelect ( object sender, TreeViewEventArgs e ) {
            TreeNode node = trVwPatterns.SelectedNode;
            if ( node == null ) { return; }
            
            if ( node.Tag != null ) {
                try {
                    this.BindDecoder( (dObject)(node.Tag) );
                    TryDecodeRawData( skipHash: true );
                } catch {}
            }
        }
        
        int cntrPaints  = 0;
        int cntrDecodes = 0;
        int cntrBinds   = 0;
        int cntrHighlightChanged = 0;
        
        void UpdateStats() {
            tlStrpLblStatus.Text = "Paints: " + cntrPaints + ", Decodes: " + cntrDecodes + ", Binds: " + cntrBinds + ", Highlight: " + cntrHighlightChanged;
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
