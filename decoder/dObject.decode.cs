/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 05.09.2026
 * Time: 15:22
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace decoder {
    
    /// <summary>
    /// Description of dObject_decode.
    /// </summary>
    partial class dObject {
        
        public void ClearRaws() {
            if ( this.Type == dObject.DecType.Complex ) {
                foreach ( dObject dObj in this.Body ) { dObj.ClearRaws(); }
            }
            
            
            for( int elId = 0; elId < this.Body.Count; elId++ ) {
                this.Body[elId].ClearRaws();
            }
            
            this.Raw = null;
            this.Value = null;
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
        
        public bool TryDecodeDataInternal(ref Byte[] arrIn, ref int idByte, ref int idBit) {
            bool status = false;
            int bi = 0;
            int By = arrIn[idByte];
            
            By = By >> ( bi % 8 );
            
            ClearRaws();
            if ( this.Body.Count > 0 ) {
                for ( int idx = 0; idx < this.Body.Count; idx++ ) {
                    this.Body[idx].TryDecodeDataInternal( ref arrIn, ref idByte, ref idBit );
                }
            } else {
                if ( this.Value == null ) {
                    /* No array, so create one */
                    this.Value = new object[this.NoElements];
                    this.Raw = new UInt64[this.NoElements];
                }
                
                if ( this.Type == dObject.DecType.Complex ) {
                    for ( int elId = 0; elId < this.NoElements; elId++ ) {
                        this.Body[elId].TryDecodeDataInternal( ref arrIn, ref idByte, ref idBit );
                    }
                }
                
                /* Decode as array (because everything is an array)*/
                for ( int elId = 0; elId < this.NoElements; elId++ ) {
                    UInt64 elRaw = 0;
                    for ( int bitIdInEl = 0; bitIdInEl < this.NoBitsUsed; bitIdInEl++ ) {
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
                            } catch (Exception ex) { }
                        }
                    }
                    this.Raw[elId] = elRaw;
                }
                this.ConvertRawToVal();
            }
            
            return status;
        }
        
        /// <summary>
        /// Now Bytes stored as LittleEndian and MSB
        /// </summary>
        /// <param name="arrIn"></param>
        /// <returns></returns>
        public bool TryDecodeData(byte[] arrIn) {
            bool status = false;
            
            if ( arrIn.Length <= 0 ) { return false; }
            if ( this.Body == null ) { return status; }
            
            int idByte = 0;
            int idBit = 0;
            for ( int idx = 0; idx < this.Body.Count; idx++ ) {
                this.Body[idx].TryDecodeDataInternal( ref arrIn, ref idByte, ref idBit );
            }
            return true;
        }
    }
}
