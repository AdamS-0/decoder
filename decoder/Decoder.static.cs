/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 30.08.2026
 * Time: 10:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace decoder {
    /// <summary>
    /// Description of Decoder_static.
    /// </summary>
    partial class Decoder {
        private static readonly Regex _rgxStruct = new Regex(@"^struct\s+([a-zA-Z_][a-zA-Z0-9_]*)\s*\{", RegexOptions.Compiled);
        private static readonly Regex _rgxStructTypedefEnd = new Regex(@"^\}\s*([a-zA-Z_][a-zA-Z0-9_]*)\s*;", RegexOptions.Compiled);
        
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
        
    }
}
