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
                noBits += decField.NoBits * decField.NoElements;
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
                    for ( int bitIdInEl = 0; bitIdInEl < decField.NoBits; bitIdInEl++ ) {
                        if ( idBit >= 8 ) {
                            idBit = 0; idByte++;
                            if ( idByte >= arrIn.Length ) { break; }
                            By = arrIn[idByte];
                        }
                        bi = By & 1;
                        By = By >> 1;
                        idBit++;
                        
                        if ( bi == 1 ) {
                            elRaw = elRaw + (UInt64)(bi << bitIdInEl);
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
        
        public int GetSizeBits() {
            int totalBits = 0;
            
            foreach ( DecoderField decField in _fields ) {
                totalBits += decField.SizeBits;
            }
            
            return totalBits;
        }
        
    }
    
    public class DecoderField  : ICloneable {
        private static readonly string _pattern =
            @"(?<Type>[\w\s\*]+?)\s*(?<Name>\w+)(?:\s*\[\s*(?<ArrayLen>\d*)\s*\])?(?:\s*:\s*(?<Bits>\d+))?\s*;";
        private static readonly Regex _rgxFieldC = new Regex(_pattern, RegexOptions.Compiled);
        private static readonly Regex _rgxSpaceCutter = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex _rgxNumberInStr = new Regex(@"\d+", RegexOptions.Compiled);
        
        
        #region ICloneable implementation
        public object Clone () {
            DecoderField decField = new DecoderField();
            
            decField._sizeBits = this._sizeBits;
            decField.IsPointer  = this.IsPointer;
            decField.IsSignedU2 = this.IsSignedU2;
            decField.Name = this.Name;
            decField.NoBits = this.NoBits;
            decField.NoElements = this.NoElements;
            decField.Type = this.Type;
            decField.Raw = null;
            decField.Value = null;
            return decField;
        }
        #endregion
        
        
        public static List<DecoderField> TryDecodeField(string strFieldC, List<Decoder>otherDecoders) {
            List<DecoderField> fields = new List<DecoderField>();
            /* Decode field from string liek this:
             "uint8_t name : 32; //! some comments"
             so formatter look like this:
             "{type} {name} [:noBits];" */

            foreach ( Match m in _rgxFieldC.Matches(strFieldC) ) {
                if ( !m.Success ) { continue; }
                
                DecoderField decField = new DecoderField();
                
                string strType      = m.Groups["Type"].Value.Trim();
                string strName      = m.Groups["Name"].Value;
                string strArrSize   = m.Groups["ArrayLen"].Value;
                string strNoBits    = m.Groups["Bits"].Value;
                
                if ( strType.Length == 0 ) { continue; }
                
                strType     = _rgxSpaceCutter.Replace( strType, " " );
                strName     = _rgxSpaceCutter.Replace( strName, " " );
                strArrSize  = _rgxSpaceCutter.Replace( strArrSize, " " );
                strNoBits   = _rgxSpaceCutter.Replace( strNoBits, " " );
                
                decField.ParseTypeFromString(strType, otherDecoders);
                decField.Name = strName;
                
                if ( !decField.IsPointer ) {
                    /* Check if this is a pointer with '*' near to field name */
                    decField.IsPointer = decField.Name.StartsWith("*");
                }
                
                int noBits = decField.SizeBits;
                if ( strNoBits.Length > 0 ) {
                    noBits = GetIntFromString(strNoBits, noBits);
                }
                decField.NoBits = noBits;
                
                int noElements = 1;
                if ( strArrSize.Length > 0 ) {
                    noElements = GetIntFromString(strArrSize, noElements);
                }
                decField.NoElements = noElements;
                if ( decField.ComplexDecoder != null ) {
                    Decoder baseDec = decField.ComplexDecoder[0];
                    decField.ComplexDecoder = new Decoder[decField.NoElements];
                    for( int elId = 0; elId < decField.NoElements; elId++ ) {
                        decField.ComplexDecoder[elId] = (Decoder)baseDec.Clone();
                    }
                }
                fields.Add( decField );
            }
            
            return fields;
        }
        
        public enum DecType {
            Integer,
            FloatingPoint,
            Char,
            Void,
            Complex,
        };
        
        public string   Name;
        public UInt64[] Raw;
        public object   Value;
        
        private int     _sizeBits = 0;
        public int      SizeBits {
            get {
                if ( this.Type == DecType.Complex ) {
                    if ( this.ComplexDecoder != null ) {
                        return this.ComplexDecoder[0].GetSizeBits() * this.NoElements;
                    }
                }
                return _sizeBits;
            }
        }
        
        public int      NoBits;
        public int      NoElements;
        public DecType  Type        = DecType.Integer;
        public bool     IsSignedU2  = true;
        public bool     IsPointer   = false;
        
        public Decoder[]  ComplexDecoder = null;
        
        public DecoderField() { }
        
        private void ParseTypeFromString(string strType, List<Decoder>otherDecoders) {
            strType = strType.Trim(' ');
            this.IsPointer  = strType.EndsWith("*");
            strType = strType.Replace("*", "");
            strType = strType.Trim(' ').ToLower();
            
            if          ( strType.Equals("float") || strType.Equals("single") ) {
                this.Type = DecType.FloatingPoint;
                this._sizeBits = 32;
            } else if   ( strType.Equals("double") ) {
                this.Type = DecType.FloatingPoint;
                this._sizeBits = 64;
            } else if   ( strType.Equals("char") ) {
                this.Type = DecType.Char;
                this._sizeBits = 8;
            } else if   ( strType.Equals("void") ) {
                this.Type = DecType.Void;
                this._sizeBits = 32;
            } else {
                bool foundDec = false;
                foreach ( Decoder dec in otherDecoders ) {
                    if ( dec.Name.ToLower().Equals( strType ) ) {
                        foundDec = true;
                        this.Type = DecType.Complex;
                        this.ComplexDecoder = new Decoder[1];
                        this.ComplexDecoder[0] = dec;
                        break;
                    }
                }
                
                if ( !foundDec ) {
                    this.Type = DecType.Integer;
                    this.IsSignedU2 = !strType.StartsWith("u");
                }
            }
            
            
            if ( this.IsPointer ) { this._sizeBits = 32; }
            
            /* GetNumberOfBits form type's name */
            Match m = _rgxNumberInStr.Match( strType );
            try {
                if ( m.Success ) {
                    this._sizeBits = Convert.ToInt32( m.Value );
                }
            } catch {
                this._sizeBits = 0;
            }
        }
        
        
        private static int GetIntFromString(string strIn, int defaultVal = 0) {
            strIn = strIn.Trim(' ');
            /* GetNumberOfBits */
            int number = defaultVal;
            try {
                Match m = _rgxNumberInStr.Match( strIn );
                if ( m.Success ) {
                    number = Convert.ToInt32( m.Value );
                }
            } catch { number = defaultVal; }
            return number;
        }
        
        public int GetSizeInBits() {
            return this.NoElements * this.NoBits;
        }
        
        public string GetTypeAsString() {
            string strType = GetElementTypeAsString();
            
            if ( this.NoElements > 1 ) {
                strType += "[" + this.NoElements + "]";
            }
            
            return strType;
        }
        
        public string GetElementTypeAsString() {
            string strType = "";
            
            if ( this.IsPointer ) {
                strType += "*";
            }
            
            if ( this.Type == DecType.Integer ) {
                if ( !this.IsSignedU2 ) { strType += "u"; }
            }
            
            switch ( this.Type ) {
                case DecType.FloatingPoint:
                    if ( this._sizeBits == 64 )  { strType += "double"; }
                    else                        { strType += "float"; }
                    
                    if ( ( this._sizeBits == 64 ) && ( this._sizeBits == 32 ) ) {
                        strType += this._sizeBits;
                    }
                    
                    break;
                case DecType.Char:
                    strType += "char"; break;
                case DecType.Void:
                    strType += "void"; break;
                default:
                    if ( this.Type == DecType.Complex ) {
                        if ( this.ComplexDecoder != null ) {
                            strType += this.ComplexDecoder[0].Name; break;
                        }
                    }
                    
                    strType += "int";
                    strType += this._sizeBits;
                    strType += " : " + this.NoBits;
                    break;
            }
            
            return strType;
        }
        
        
        public void ConvertRawToVal() {
            try {
                switch ( this.Type ) {
                    case DecType.Char:
                        for ( int idx = 0; idx < this.NoElements; idx++ ) {
                            ((object[])this.Value)[idx] = (object)this.Raw[idx];
                        }
                        break;
                    
                    case DecType.FloatingPoint:
                        for ( int idx = 0; idx < this.NoElements; idx++ ) {
                            if ( this._sizeBits == 64 ) {
                                unsafe {
                                    fixed (ulong* pRawDouble = &( this.Raw[idx] ) ) {
                                        ((object[])this.Value)[idx] = (object)(*(double*)pRawDouble);
                                    }
                                }
                            } else if ( this._sizeBits == 32 ) {
                                uint rawInt32 = (uint)this.Raw[idx];
                                unsafe {
                                    fixed (ulong* pRawFloat = &this.Raw[idx]) {
                                        ((object[])this.Value)[idx] = (object)(*(float*)pRawFloat);
                                    }
                                }
                            }
                        }
                        break;
                    
                    case DecType.Void:
                        for ( int idx = 0; idx < this.NoElements; idx++ ) {
                            ((object[])this.Value)[idx] = this.Raw[idx];
                        }
                        break;
                        
                    case DecType.Integer:
                        for ( int idx = 0; idx < this.NoElements; idx++ ) {
                            ((object[])this.Value)[idx] = InterpretBits( (ulong)this.Raw[idx], this.IsSignedU2, this._sizeBits );
                        }
                        break;
                    default:
                        this.Value = this.Raw; break;
                }
                
                if ( this.IsPointer ) {
                    for ( int idx = 0; idx < this.NoElements; idx++ ) {
                        ((object[])this.Value)[idx] = this.Raw[idx];
                    }
                }
            } catch ( Exception ex ) {}
        }
        
        
        public static object InterpretBits ( ulong raw, bool isSigned, int numOfBits ) {
            if ( numOfBits <= 0 ) return (ulong)0;
            if ( numOfBits > 64 ) numOfBits = 64;

            if ( numOfBits == 64 ) {
                if ( isSigned ) {
                    return (long)raw;
                }
                return raw;
            }

            ulong mask = ( 1UL << numOfBits ) - 1UL;
            ulong maskedValue = raw & mask;

            if ( !isSigned ) { return maskedValue; }

            ulong signBitMask = 1UL << ( numOfBits - 1 );

            if ( ( maskedValue & signBitMask ) != 0 ) {
                ulong higherBitsMask = ~mask;
                ulong negativeRaw = maskedValue | higherBitsMask;

                return (long)negativeRaw;
            }
            return (long)maskedValue;
        }
        
        
    }
}
