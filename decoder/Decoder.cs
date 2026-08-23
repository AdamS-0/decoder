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
    public class Decoder {
        
        public string Name = "";
        
        public static List<DecoderField> ParseStruct(string strStructC) {
            List<DecoderField> fields = new List<DecoderField>();
            
            foreach ( string strStructLine in strStructC.Split('\n') ) {
                List<DecoderField> fieldsSub = DecoderField.TryDecodeField( strStructLine );
                foreach ( DecoderField field in fieldsSub ) {
                    if ( field == null ) { continue; }
                    fields.Add( field );
                }
            }
            
            return fields;
        }
        
        
        public  List<DecoderField>              _fields;
        private byte[]                          _rawBytes;
            
        public Decoder( string strStructC ) {
            this._fields = Decoder.ParseStruct( strStructC );
            int noBits = 0;
            foreach ( DecoderField decField in this._fields ) {
                noBits += decField.NoBits * decField.NoElements;
            }
            int noBytes = ( noBits / 8 ) + 1;
            this._rawBytes = new byte[noBytes];
        }
        
        public Decoder() {
            
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
            Array.Clear( this._rawBytes, 0, this._rawBytes.Length );
            Array.Copy( arrIn, _rawBytes, Math.Min( arrIn.Length, _rawBytes.Length ) );
            
            int idByte = 0;
            int idBit = 0;
            int bi = 0;
            int By = _rawBytes[0];
            
            foreach ( DecoderField decField in this._fields ) {
                decField.Raw = 0;
                decField.Value = null;
            }
            
            foreach ( DecoderField decField in this._fields ) {
                if ( decField.NoElements > 1 ) {
                    if ( decField.Value == null ) {
                        /* No array, so create one */
//                        object[] tempArr = new object[decField.NoElements];
                        decField.Value = new object[decField.NoElements];
                    }
                    
                    for ( int elId = 0; elId < decField.NoElements; elId++ ) {
                        UInt64 elRaw = 0;
                        for ( int bitIdInEl = 0; bitIdInEl < decField.SizeBits; bitIdInEl++ ) {
                            if ( idBit >= 8 ) {
                                idBit = 0; idByte++;
                                
                                if ( idByte > _rawBytes.Length ) { break; }
                                
                                By = _rawBytes[idByte];
                            }
                            bi = By & 1;
                            By = By >> 1;
                            idBit++;
                            
                            if ( bi == 1 ) {
                                elRaw = elRaw + (UInt64)(bi << bitIdInEl);
                            }
                        }
//                        try {
                            (((object[])decField.Value)[elId]) = (object)elRaw;
//                        } catch {}
                    }
                } else {
                    for ( int bitIdInField = 0; bitIdInField < decField.NoBits; bitIdInField++ ) {
                        if ( idBit >= 8 ) {
                            idBit = 0; idByte++;
                            
                            if ( idByte > _rawBytes.Length ) { break; }
                            
                            By = _rawBytes[idByte];
                        }
                        bi = By & 1;
                        By = By >> 1;
                        idBit++;
                        
                        if ( bi == 1 ) {
                            decField.Raw = (UInt64)decField.Raw + (UInt64)(bi << bitIdInField);
                        }
                    }
                }
                if ( idByte > _rawBytes.Length ) { break; }
            }
            
            foreach ( DecoderField decField in this._fields ) {
                decField.ConvertRawToVal();
            }
            
            return status;
        }
        
    }
    
    public class DecoderField {
        private static readonly string _pattern =
            @"(?<Type>[\w\s\*]+?)\s*(?<Name>\w+)(?:\s*\[\s*(?<ArrayLen>\d*)\s*\])?(?:\s*:\s*(?<Bits>\d+))?\s*;";
        private static readonly Regex _rgxFieldC = new Regex(_pattern, RegexOptions.Compiled);
        private static readonly Regex _rgxSpaceCutter = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex _rgxNumberInStr = new Regex(@"\d+", RegexOptions.Compiled);
        
        public static List<DecoderField> TryDecodeField(string strFieldC) {
            List<DecoderField> fields = new List<DecoderField>();
            /* Decode field from string liek this:
             "uint8_t name : 32; //! some comments"
             so formatter look like this:
             "{type} {name} [:noBits];" */
            
            strFieldC = strFieldC.Replace("\t", " ");

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
                
                decField.ParseTypeFromString(strType);
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
                fields.Add( decField );
            }
            
            return fields;
        }
        
        public enum DecType {
            Integer,
            FloatingPoint,
            Char,
            Void,
        };
        
        public string   Name;
        public UInt64   Raw;
        public object   Value;
        
        public int      SizeBits;
        public int      NoBits;
        public int      NoElements;
        public DecType  Type        = DecType.Integer;
        public bool     IsSignedU2  = true;
        public bool     IsPointer   = false;
        
        public DecoderField() { }
        
        
        public Type GetFieldType() {
            if ( this.IsPointer ) {
                switch ( this.SizeBits ) {
                    case  8: return this.NoElements > 1 ? typeof(byte[]) : typeof(byte);
                    case 16: return this.NoElements > 1 ? typeof(UInt16[]) : typeof(UInt16);
                    case 64: return this.NoElements > 1 ? typeof(UInt64[]) : typeof(UInt64);
                    default: return this.NoElements > 1 ? typeof(UInt32[]) : typeof(UInt32);
                }
            }
            
            switch ( this.Type ) {
                case DecType.FloatingPoint:
                    if ( this.SizeBits == 32 ) { return this.NoElements > 1 ? typeof(float[]) : typeof(float); }
                    return this.NoElements > 1 ? typeof(double[]) : typeof(double);
                case DecType.Char:
                    return this.NoElements > 1 ? typeof(char[]) : typeof(char);
                default:

                    if ( this.IsSignedU2 ) {
                        switch ( this.SizeBits ) {
                                case 64: return this.NoElements > 1 ? typeof(Int64[]) : typeof(Int64);
                                case 32: return this.NoElements > 1 ? typeof(Int32[]) : typeof(Int32);
                                case 16: return this.NoElements > 1 ? typeof(Int16[]) : typeof(Int16);
                                default: return this.NoElements > 1 ? typeof(sbyte[]) : typeof(sbyte);
                        }
                    }
                    
                    switch ( this.SizeBits ) {
                        case 64: return this.NoElements > 1 ? typeof(UInt64[]) : typeof(UInt64);
                        case 32: return this.NoElements > 1 ? typeof(UInt32[]) : typeof(UInt32);
                        case 16: return this.NoElements > 1 ? typeof(UInt16[]) : typeof(UInt16);
                        default: return this.NoElements > 1 ? typeof(byte  []) : typeof(byte  );
                    }
            }
        }
        
        private void ParseTypeFromString(string strType) {
            strType = strType.Trim(' ').ToLower();
            
            this.IsPointer  = strType.Contains("*");
            if ( this.IsPointer ) { this.SizeBits = 32; }
            
            if          ( strType.Contains("float") || strType.Contains("single") ) {
                this.Type = DecType.FloatingPoint;
                this.SizeBits = 32;
            } else if   ( strType.Contains("double") ) {
                this.Type = DecType.FloatingPoint;
                this.SizeBits = 64;
            } else if   ( strType.Contains("char") ) {
                this.Type = DecType.Char;
                this.SizeBits = 8;
            } else if   ( strType.Contains("void") ) {
                this.Type = DecType.Void;
                this.SizeBits = 32;
            } else {
                this.Type = DecType.Integer;
                this.IsSignedU2 = !strType.StartsWith("u");
            }
            
            
            /* GetNumberOfBits form type's name */
            Match m = _rgxNumberInStr.Match( strType );
            try {
                if ( m.Success ) {
                    this.SizeBits = Convert.ToInt32( m.Value );
                }
            } catch {
                this.SizeBits = 0;
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
                    if ( this.SizeBits == 64 )  { strType += "double"; }
                    else                        { strType += "float"; }
                    
                    if ( ( this.SizeBits == 64 ) && ( this.SizeBits == 32 ) ) {
                        strType += this.SizeBits;
                    }
                    
                    break;
                case DecType.Char:
                    strType += "char"; break;
                case DecType.Void:
                    strType += "void"; break;
                default:
                    strType += "int";
                    strType += this.SizeBits;
                    strType += " : " + this.NoBits;
                    break;
            }
            
            return strType;
        }
        
        
        public void ConvertRawToVal() {
            if ( this.NoElements > 1 ) {
                /* This is an array, so just quit, because previous call should write to Value as "object[]" */
                return;
            }
            
            this.Value = this.Raw;
            if ( this.IsPointer ) { return; }
            
            try {
                switch ( this.Type ) {
                    case DecType.Char:
                        this.Value = (char)this.Raw;
                        break;
                    
                    case DecType.FloatingPoint:
                        if ( this.SizeBits == 64 ) {
                            unsafe {
                                fixed (ulong* pRawDouble = &this.Raw) {
                                    this.Value = *(double*)pRawDouble;
                                }
                            }
                        } else if ( this.SizeBits == 32 ) {
                            uint rawInt32 = (uint)this.Raw;
                            unsafe {
                                fixed (ulong* pRawFloat = &this.Raw) {
                                    this.Value = *(float*)pRawFloat;
                                }
                            }
                        }
                        break;
                    
                    case DecType.Void:
                        this.Value = (char)this.Raw;
                        break;
                        
                    case DecType.Integer:
                        this.Value = InterpretBits( (ulong)this.Raw, this.IsSignedU2, this.SizeBits );
                        break;
                }
            } catch {}
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
