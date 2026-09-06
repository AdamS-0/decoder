/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 30.08.2026
 * Time: 07:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace decoder {
    partial class dObject {
        
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UInt64 SetBit ( UInt64 value, int bitIndex ) {
            return unchecked( value | ( 1UL << bitIndex ) );
        }
    
//        [MethodImpl( MethodImplOptions.AggressiveInlining)]
        public static UInt64 ClearBit ( UInt64 value, int bitIndex ) {
            return unchecked( value & ~( 1UL << bitIndex ) );
        }
        
        
        private static readonly string _pattern =
            @"(?<Type>[\w\s\*]+?)\s*(?<Name>\w+)(?:\s*\[\s*(?<ArrayLen>\d*)\s*\])?(?:\s*:\s*(?<Bits>\d+))?\s*;";
        private static readonly Regex _rgxFieldC = new Regex(_pattern, RegexOptions.Compiled);
        private static readonly Regex _rgxSpaceCutter = new Regex(@"\s+", RegexOptions.Compiled);
        private static readonly Regex _rgxNumberInStr = new Regex(@"\d+", RegexOptions.Compiled);
        
//        public static List<dObject> TryDecodeField(string strFieldC, List<Decoder>otherDecoders) {
//            List<dObject> fields = new List<dObject>();
//            /* Decode field from string liek this:
//             "uint8_t name : 32; //! some comments"
//             so formatter look like this:
//             "{type} {name} [:noBits];" */
//
//            foreach ( Match m in _rgxFieldC.Matches(strFieldC) ) {
//                if ( !m.Success ) { continue; }
//                
//                dObject decField = new dObject();
//                
//                string strType          = m.Groups["Type"].Value.Trim();
//                string strName          = m.Groups["Name"].Value;
//                string strArrSize       = m.Groups["ArrayLen"].Value;
//                string strNoBitsUsed    = m.Groups["Bits"].Value;
//                
//                if ( strType.Length == 0 ) { continue; }
//                
//                decField.dObjRaw = strFieldC;
//                
//                strType     = _rgxSpaceCutter.Replace( strType, " " );
//                strName     = _rgxSpaceCutter.Replace( strName, " " );
//                strArrSize  = _rgxSpaceCutter.Replace( strArrSize, " " );
//                strNoBitsUsed   = _rgxSpaceCutter.Replace( strNoBitsUsed, " " );
//                
//                decField.ParseTypeFromString(strType, otherDecoders);
//                decField.Name = strName;
//                
//                if ( !decField.IsPointer ) {
//                    /* Check if this is a pointer with '*' near to field name */
//                    decField.IsPointer = decField.Name.StartsWith("*");
//                }
//                
//                int noElements = 1;
//                if ( strArrSize.Length > 0 ) {
//                    noElements = GetIntFromString(strArrSize, noElements);
//                }
//                decField.NoElements = (ushort)noElements;
//                
//                int noBitsUsed = decField.ElementSizeBits;
//                if ( strNoBitsUsed.Length > 0 ) {
//                    noBitsUsed = GetIntFromString(strNoBitsUsed, noBitsUsed);
//                }
//                decField.NoBitsUsed = noBitsUsed;
//                
//                if ( decField.ComplexDecoder != null ) {
//                    Decoder baseDec = decField.ComplexDecoder[0];
//                    decField.ComplexDecoder = new Decoder[decField.NoElements];
//                    for( int elId = 0; elId < decField.NoElements; elId++ ) {
//                        decField.ComplexDecoder[elId] = (Decoder)baseDec.Clone();
//                    }
//                }
//                fields.Add( decField );
//            }
//            
//            return fields;
//        }
        
        
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
        
        
        public static object InterpretBits ( ulong raw, bool isSigned, int numOfBits ) {
            if ( numOfBits <= 0 ) return (ulong)0;
            if ( numOfBits > 64 ) numOfBits = 64;

            if ( numOfBits == 64 ) {
                if ( isSigned ) {
                    return unchecked((long)raw);
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

                return unchecked((long)negativeRaw);
            }
            return unchecked((long)maskedValue);
        }

    }
}
