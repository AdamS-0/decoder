/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 16.08.2026
 * Time: 07:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using System.Windows.Forms;

namespace decoder {
    /// <summary>
    /// Description of Decoder.
    /// </summary>
    public class Decoder : ICloneable {
        private static readonly Regex _rgxStruct = new Regex(@"^struct\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*\{", RegexOptions.Compiled);
        private static readonly Regex _rgxStructTypedefEnd = new Regex(@"^\}\s*([a-zA-Z_][a-zA-Z0-9_]*)\s*;", RegexOptions.Compiled);
        
        #region ICloneable implementation
        public object Clone () {
            Decoder dec = new Decoder();
            dec._fields = new List<DecoderField>();
            foreach( DecoderField decField in this._fields ) {
                dec._fields.Add( (DecoderField)decField.Clone() );
            }
            
            dec.Name = this.Name;
            dec.RawBytes = new byte[ this.RawBytes.Length ];
            return dec;
        }
        #endregion
        
        
        public string Name = "";
        
        public static List<Decoder> AllDecoders = new List<Decoder>();
        
        public static Decoder FindByName(string decName) {
            foreach ( Decoder dec in AllDecoders ) {
                if ( dec.Name.Equals( decName ) ) {
                    return dec;
                }
            }
            return null;
        }
        
        public static List<DecoderField> ParseStruct(string strStructC, ref string strStructName) {
            List<DecoderField> fields = new List<DecoderField>();
            
            string strStructCSplitted = strStructC.Replace("{", "{\n");
            strStructCSplitted = strStructCSplitted.Replace(";", ";\n");
            
            bool gotTypedef = false;
            DecoderGui decGui = MainListener.GetCurrGui();
            foreach ( string strStructLine in strStructCSplitted.Split('\n') ) {
                string lineI = strStructLine;
                lineI = lineI.Replace("\t", " ");
                lineI = lineI.Trim( ' ' );
                
                if ( strStructName.Length == 0 ) {
                    if ( lineI.StartsWith("typedef ") ) {
                        gotTypedef = true;
                    }
                    Match m = null;
                    if ( ( m = _rgxStruct.Match( lineI ) ).Success ) {
                        strStructName = m.Groups[1].ToString();
                    } else if ( ( m = _rgxStructTypedefEnd.Match( lineI ) ).Success && gotTypedef ) {
                        strStructName = m.Groups[1].ToString();
                    }
                }
                
                List<DecoderField> fieldsSub = DecoderField.TryDecodeField( lineI, decGui._decoders );
                foreach ( DecoderField field in fieldsSub ) {
                    if ( field == null ) { continue; }
                    fields.Add( field );
                }
            }
            
            return fields;
        }
        
        
        public  List<DecoderField>              _fields;
        public  byte[]                          RawBytes;
            
        public Decoder( string strStructC, string decName = "" ) {
            string structName = decName;
            this._fields = Decoder.ParseStruct( strStructC, ref structName );
            int noBits = 0;
            foreach ( DecoderField decField in this._fields ) {
                noBits += decField.NoBitsUsed * decField.NoElements;
            }
            int noBytes = ( noBits / 8 ) + 1;
            this.RawBytes = new byte[noBytes];
            
            this.Name = structName;
        }
        
        public Decoder() {
            
        }
        
        
        public void ClearRaws() {
            foreach ( DecoderField decField in this._fields ) {
                if ( decField.Type == DecoderField.DecType.Complex ) {
                    if ( decField.ComplexDecoder != null ) {
                        for( int elId = 0; elId < decField.NoElements; elId++ ) {
                            decField.ComplexDecoder[elId].ClearRaws();
                        }
                        continue;
                    }
                }
                
                decField.Raw = null;
                decField.Value = null;
            }
        }
        
        
        /// <summary>
        /// Now Bytes stored as LittleEndian and MSB
        /// </summary>
        /// <param name="arrIn"></param>
        /// <returns></returns>
        public bool TryDecode(byte[] arrIn) {
            bool status = false;
            
            if ( arrIn.Length <= 0 ) { return false; }
            if ( this._fields == null ) { return status; }
            Array.Clear( this.RawBytes, 0, this.RawBytes.Length );
            Array.Copy( arrIn, RawBytes, Math.Min( arrIn.Length, RawBytes.Length ) );
            
            int idByte = 0;
            int idBit = 0;
            return this.TryDecodeInternal( ref this.RawBytes, ref idByte, ref idBit );
        }
        
        
        protected bool TryDecodeInternal(ref Byte[] arrIn, ref int idByte, ref int idBit) {
            bool status = false;
            int bi = 0;
            int By = arrIn[idByte];
            
            By = By >> ( bi % 8 );
            
            ClearRaws();
            
            foreach ( DecoderField decField in this._fields ) {
                if ( decField.Value == null ) {
                    /* No array, so create one */
                    decField.Value = new object[decField.NoElements];
                    decField.Raw = new ulong[decField.NoElements];
                }
                
                if ( decField.Type == DecoderField.DecType.Complex ) {
                    if ( decField.ComplexDecoder != null ) {
                        for ( int elId = 0; elId < decField.NoElements; elId++ ) {
                            decField.ComplexDecoder[elId].TryDecodeInternal( ref arrIn, ref idByte, ref idBit );
                        }
                        continue;
                    }
                }
                
                /* Decode as array (because everything is an array)*/
                for ( int elId = 0; elId < decField.NoElements; elId++ ) {
                    UInt64 elRaw = 0;
                    for ( int bitIdInEl = 0; bitIdInEl < decField.NoBitsUsed; bitIdInEl++ ) {
                        if ( idBit >= 8 ) {
                            idBit = 0; idByte++;
                            if ( idByte >= arrIn.Length ) { break; }
                            By = arrIn[idByte];
                        }
                        bi = By & 1;
                        By = By >> 1;
                        idBit++;
                        
                        if ( bi == 1 ) {
                            try {
                                unchecked {
                                    elRaw = elRaw + (UInt64)(1UL << bitIdInEl);
                                }
                            } catch (Exception ex) {
                                int dummyInt22 = 0;
                            }
                        }
                    }
                    decField.Raw[elId] = elRaw;   
                }
                if ( idByte > arrIn.Length ) { break; }
            }
            
            foreach ( DecoderField decField in this._fields ) {
                decField.ConvertRawToVal();
            }
            
            return status;
        }
        
        public ushort GetSizeBits() {
            ushort totalBits = 0;
            
            foreach ( DecoderField decField in _fields ) {
                totalBits += decField.ElementSizeBits;
            }
            
            return totalBits;
        }
        
        
        public void RebuildPattern() {
            List<DecoderField> fields = new List<DecoderField>();
            DecoderGui decGui = MainListener.GetCurrGui();
            
            foreach ( DecoderField orgField in this._fields ) {
                List<DecoderField> fieldsSub = DecoderField.TryDecodeField( orgField.BasePattern, decGui._decoders );
                foreach ( DecoderField field in fieldsSub ) {
                    if ( field == null ) { continue; }
                    fields.Add( field );
                }
            }
            this._fields.Clear();
            this._fields = fields;
        }
        
    }
    
}
