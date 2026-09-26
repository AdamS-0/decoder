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
    public enum DecType {
        Integer,
        FloatingPoint,
        Char,
        Void,
        Complex,
    };
    
    public partial class dObject : dObjectSub {        
        public dObject() { }
        
        public int GetTotalSizeInBits() {
            return this.NoElements * this.NoBitsUsed;
        }
        
        public ushort GetStructSizeBits() {
            ushort totalBits = 0;
            
            foreach ( dObject decField in this.Body[0] ) {
                totalBits += (ushort)decField.GetTotalSizeInBits();
            }
            
            return totalBits;
        }
        
        public string GetTypeAsString() {
            string strType = GetElementTypeAsString();
            
            if ( this.NoElements > 1 ) {
                strType += "[" + this.NoElements + "]";
            }
            
            if ( ( strType.Length > 0 ) && !strType.EndsWith(" ") ) { strType += " "; }
            
            if      ( this.IsEnum    ) { strType += "{enum}"; }
            else if ( this.IsStruct  ) { strType += "{struct}"; }
            else if ( this.IsUnion   ) { strType += "{union}"; }
            
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
                    if ( this.ElementSizeBits == 64 )  { strType += "double"; }
                    else                        { strType += "float"; }
                    
                    if ( ( this.ElementSizeBits == 64 ) && ( this.ElementSizeBits == 32 ) ) {
                        strType += this.ElementSizeBits;
                    }
                    
                    break;
                case DecType.Char:
                    strType += "char"; break;
                case DecType.Void:
                    strType += "void"; break;
                default:
                    if ( this.Type == DecType.Complex ) {
                        strType += this.TypeName; break;
                    }
                    
                    strType += "int";
                    strType += this.ElementSizeBits;
                    strType += " : " + this.NoBitsUsed;
                    break;
            }
            
            return strType;
        }
        
    }
    
    public class dObjectSub : ICloneable {
        #region ICloneable implementation
        public object CloneToObject(dObject toObj) {
            toObj._mask     = this._mask;
            toObj.Name      = this.Name;
            toObj.Type      = this.Type;
            toObj.dObjRaw   = this.dObjRaw;
            toObj.Raw       = this.Raw;
            toObj.Value     = this.Value;
            toObj.TypeName  = this.TypeName;
            
            if ( this.Type == DecType.Complex ) {
                toObj.Body.Clear();
                toObj.Body = new List<List<dObject>>();
                for( int idxElement = 0; idxElement < this.Body.Count; idxElement++ ) {
                    List<dObject> elemBodyList = new List<dObject>();
                    toObj.Body.Add( elemBodyList );
                    for( int idx = 0; idx < this.Body[idxElement].Count; idx++ ) {
                        elemBodyList.Add( (dObject)this.Body[idxElement][idx].Clone() );
                    }
                }
            }
            
            return toObj;
        }
        
        public object Clone () {
            dObject decField = new dObject();
            CloneToObject(decField);
            return decField;
        }
        #endregion
        
        public string           dObjRaw = "";
        public string           Comment = "";
        public string           Name    = "";
        public string           TypeName= "";
        
        public object           Value; /* Used only for basic types */
        public UInt64[]         Raw; /* Used only for basic types */
        
        /* Used only for complex data types */
        public List<List<dObject>>  Body = new List<List<dObject>>();
        
        /// <summary>
        /// Mask with common parameters, to minimize size of a instance
        /// Bits:
        ///  b0 - b15   => NoElements, ushort, [0 - 65535]
        /// b16 - b31   => ElementSizeBits, ushort, [0 - 65535]
        /// b32 - b47   => NoBitsUsed, ushort, [0 - 65535]
        /// b48 - b57   => Reserved
        /// B58         => IsTypedef, bool
        /// B59         => IsStruct, bool
        /// B60         => IsEnum, bool
        /// B61         => IsUnion, bool
        /// B62         => IsPointer, bool
        /// b63         => IsSignedU2, bool
        /// </summary>
        protected UInt64 _mask = 0;

        /// <summary>
        /// Number of elements
        /// </summary>
        public ushort   NoElements {
            get { return (ushort)Bitmask.Get(_mask, 0, 16); }
            set { Bitmask.Set( ref _mask, 0, 16, value ); }
        }
        
        /// <summary>
        /// Number of bits per one element
        /// </summary>
        public ushort   ElementSizeBits {
            get { return (ushort)Bitmask.Get(_mask, 16, 16); }
            set { Bitmask.Set( ref _mask, 16, 16, value ); }
        }
        
        /// <summary>
        /// Number of bits used by this field, e.g. 4 from uint16
        /// </summary>
        public ushort   NoBitsUsed {
            get { return (ushort)Bitmask.Get(_mask, 32, 16); }
            set { Bitmask.Set( ref _mask, 32, 16, value ); }
        }
        
        
        public bool     IsTypedef   {
            get { return Bitmask.GetBit(_mask, 58); }
            set { Bitmask.SetBit( ref _mask, 58, value ); }
        }
        
        public bool     IsStruct    {
            get { return Bitmask.GetBit(_mask, 59); }
            set { Bitmask.SetBit( ref _mask, 59, value ); }
        }
        
        public bool     IsEnum      {
            get { return Bitmask.GetBit(_mask, 60); }
            set { Bitmask.SetBit( ref _mask, 60, value ); }
        }
        
        public bool     IsUnion     {
            get { return Bitmask.GetBit(_mask, 61); }
            set { Bitmask.SetBit( ref _mask, 61, value ); }
        }
        
        public bool     IsPointer  {
            get { return Bitmask.GetBit(_mask, 62); }
            set { Bitmask.SetBit( ref _mask, 62, value ); }
        }
        
        public DecType  Type        = DecType.Integer;
        public bool     IsSignedU2  {
            get { return Bitmask.GetBit(_mask, 63); }
            set { Bitmask.SetBit( ref _mask, 63, value ); }
        }
        
        public dObjectSub() {
            List<dObject> newList = new List<dObject>();
            Body.Add( newList );
        }
        
        public override string ToString() { return "dObject: [" + this.Name + "]"; }
        
    }
    
}
