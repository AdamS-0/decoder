/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 30.08.2026
 * Time: 07:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CsBitmask;

namespace decoder {
    public partial class DecoderField  : ICloneable {        
        #region ICloneable implementation
        public object Clone () {
            DecoderField decField = new DecoderField();
            
            decField._mask = this._mask;
            decField.Name = this.Name;
            decField.Type = this.Type;
            decField.BasePattern = this.BasePattern;
            decField.Raw = null;
            decField.Value = null;
            
            if ( this.Type == DecType.Complex ) {
                if ( this.ComplexDecoder != null ) {
                    decField.ComplexDecoder = new Decoder[this.NoElements];
                    for( int idx = 0; idx < this.NoElements; idx++ ) {
                        decField.ComplexDecoder[idx] = (Decoder)this.ComplexDecoder[idx].Clone();
                    }
                }
            }
            
            return decField;
        }
        #endregion
        
        public enum DecType {
            Integer,
            FloatingPoint,
            Char,
            Void,
            Complex,
        };
        
        public string   BasePattern;
        public string   Name;
        public UInt64[] Raw;
        public object   Value;
        
        /// <summary>
        /// Mask with common parameters, to minimize size of a instance
        /// Bits:
        ///  b0 - b15   => NoElements, ushort, [0 - 65535]
        /// b16 - b31   => _sizeBits, ushort, [0 - 65535]
        /// b32 - b47   => NoBitsUsed, ushort, [0 - 65535]
        /// b48 - b61   => Reserved
        /// B62         => IsPointer, bool
        /// b63         => IsSignedU2, bool
        /// </summary>
        protected UInt64 _mask = 0;
        
        /// <summary>
        /// Number of bits used by this field, e.g. 4 from uint16
        /// </summary>
        public int      NoBitsUsed {
            get { return (ushort)Bitmask.Get(_mask, 32, 16); }
            set { Bitmask.Set( ref _mask, 32, 16, value ); }
        }
        
        private ushort  _elementSizeBits {
            get { return (ushort)Bitmask.Get(_mask, 16, 16); }
            set { Bitmask.Set( ref _mask, 16, 16, value ); }
        }
        
        
        /// <summary>
        /// Number of bits per one element
        /// </summary>
        public ushort   ElementSizeBits {
            get {
                if ( this.Type == DecType.Complex ) {
                    if ( this.ComplexDecoder != null ) {
                        return this.ComplexDecoder[0].GetStructSizeBits();
                    }
                }
                return _elementSizeBits;
            }
        }
        
        public ushort   NoElements {
            get { return (ushort)Bitmask.Get(_mask, 0, 16); }
            set { Bitmask.Set( ref _mask, 0, 16, value ); }
        }
        
        public DecType  Type        = DecType.Integer;
        public bool     IsSignedU2  {
            get { return Bitmask.GetBit(_mask, 63); }
            set { Bitmask.SetBit( ref _mask, 63, value ); }
        }
        
        public bool     IsPointer  {
            get { return Bitmask.GetBit(_mask, 62); }
            set { Bitmask.SetBit( ref _mask, 62, value ); }
        }
        
        public Decoder[]  ComplexDecoder = null;
        
        public DecoderField() { }
        
        private void ParseTypeFromString(string strType, List<Decoder>otherDecoders) {
            strType = strType.Trim(' ');
            this.IsPointer  = strType.EndsWith("*");
            strType = strType.Replace("*", "");
            strType = strType.Trim(' ').ToLower();
            
            if          ( strType.Equals("float") || strType.Equals("single") ) {
                this.Type = DecType.FloatingPoint;
                this._elementSizeBits = 32;
            } else if   ( strType.Equals("double") ) {
                this.Type = DecType.FloatingPoint;
                this._elementSizeBits = 64;
            } else if   ( strType.Equals("char") ) {
                this.Type = DecType.Char;
                this._elementSizeBits = 8;
            } else if   ( strType.Equals("void") ) {
                this.Type = DecType.Void;
                this._elementSizeBits = 32;
            } else {
                bool foundDec = false;
                foreach ( Decoder dec in otherDecoders ) {
                    if ( dec.Name.ToLower().Equals( strType ) ) {
                        foundDec = true;
                        this.Type = DecType.Complex;
                        this.ComplexDecoder = new Decoder[1];
                        this.ComplexDecoder[0] = dec;
                        this._elementSizeBits = this.ComplexDecoder[0].GetStructSizeBits();
                        break;
                    }
                }
                
                if ( !foundDec ) {
                    this.Type = DecType.Integer;
                    this.IsSignedU2 = !strType.StartsWith("u");
                }
            }
            
            
            if ( this.IsPointer ) { this._elementSizeBits = 32; }
            
            /* GetNumberOfBits form type's name */
            Match m = _rgxNumberInStr.Match( strType );
            try {
                if ( m.Success ) {
                    this._elementSizeBits = Convert.ToUInt16( m.Value );
                }
            } catch {
                this._elementSizeBits = 0;
            }
        }
        
        public int GetTotalSizeInBits() {
            return this.NoElements * this.NoBitsUsed;
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
                    if ( this._elementSizeBits == 64 )  { strType += "double"; }
                    else                        { strType += "float"; }
                    
                    if ( ( this._elementSizeBits == 64 ) && ( this._elementSizeBits == 32 ) ) {
                        strType += this._elementSizeBits;
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
                    strType += this._elementSizeBits;
                    strType += " : " + this.NoBitsUsed;
                    break;
            }
            
            return strType;
        }
        
        
        public void ConvertRawToVal() {
            try {
                switch ( this.Type ) {
                    case DecType.Char:
                        for ( int idx = 0; idx < this.NoElements; idx++ ) {
                            ((object[])this.Value)[idx] = "'" + (char)this.Raw[idx] + "'";
                        }
                        break;
                    
                    case DecType.FloatingPoint:
                        for ( int idx = 0; idx < this.NoElements; idx++ ) {
                            if ( this._elementSizeBits == 64 ) {
                                unsafe {
                                    fixed (ulong* pRawDouble = &( this.Raw[idx] ) ) {
                                        ((object[])this.Value)[idx] = (object)(*(double*)pRawDouble);
                                    }
                                }
                            } else if ( this._elementSizeBits == 32 ) {
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
                            ((object[])this.Value)[idx] = InterpretBits( (ulong)this.Raw[idx], this.IsSignedU2, this._elementSizeBits );
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
        
    }
}
