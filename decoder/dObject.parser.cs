/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 05.09.2026
 * Time: 15:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace decoder {
    /// <summary>
    /// Description of dObject_parser.
    /// </summary>
    partial class dObject {
        
        /// <summary>
        /// Parse parameters of given axtox's params string, e.g. "typedef struct MyStruct_t;" without its body of course
        /// </summary>
        /// <param name="atox"></param>
        /// <param name="strAtoxParams"></param>
        public void ParseParamsOfAtox(string strAtoxParams, ref List<dObject> ownTypes) {
            bool    isPointer       = false;
            int     noUsedBits      = -1;
            string  strNoElements   = "1";
            string  strComment      = "";
            string  strDObjRaw      = strAtoxParams;
            
            /* Get comments from sentence */
            int idxCommentStart = -1;
            while ( ( idxCommentStart = strAtoxParams.IndexOf("//") ) >= 0 ) {
                int idxCommentEnd = strAtoxParams.IndexOf('\n', idxCommentStart);
                if ( idxCommentEnd == -1 ) { idxCommentEnd = strAtoxParams.Length - 1; }
                
                if ( ( idxCommentEnd > 0 ) && ( idxCommentEnd < strAtoxParams.Length ) ) {
                    strComment += strAtoxParams.Substring( idxCommentStart, idxCommentEnd - idxCommentStart + 1 ) + "\r\n";
                    string newStrAtox = strAtoxParams.Substring( 0, idxCommentStart );
                    newStrAtox += strAtoxParams.Substring( idxCommentEnd + 1 );
                    strAtoxParams = newStrAtox;
                }
            }
            
            while ( ( idxCommentStart = strAtoxParams.IndexOf("/*") ) >= 0 ) {
                int idxCommentEnd = strAtoxParams.IndexOf("*/");
                
                if ( ( idxCommentEnd <= 0 ) || ( idxCommentEnd >= strAtoxParams.Length ) ) { break; }
                
                strComment += strAtoxParams.Substring( idxCommentStart, idxCommentEnd - idxCommentStart + 2 ) + "\r\n";
                string newStrAtox = strAtoxParams.Substring( 0, idxCommentStart );
                newStrAtox += strAtoxParams.Substring( idxCommentEnd + 2 );
                strAtoxParams = newStrAtox;
            }
            
            /* Cut off ';' and multiple spaces */
            strAtoxParams = strAtoxParams.Replace( "\r", " " );
            strAtoxParams = strAtoxParams.Replace( "\n", " " );
            strAtoxParams = strAtoxParams.Replace( "\t", " " );
            strAtoxParams = strAtoxParams.Replace( "*", " * " );
            strAtoxParams = strAtoxParams.Replace( "[", " [ " );
            strAtoxParams = strAtoxParams.Replace( "]", " ] " );
            strAtoxParams = _rgxSpaceCutter.Replace( strAtoxParams, " " );
            strAtoxParams = strAtoxParams.Replace( ";", "" );
            
            string[] partsArr = strAtoxParams.Split(' ');
            if ( partsArr.Length == 0 ) { return; }
            
            List<string> parts = new List<string>(partsArr);
            
            for ( int idx = 0; idx < parts.Count; idx++ ) {
                if ( parts[idx].Length == 0 ) {
                    parts.RemoveAt( idx );
                    idx--;
                }
            }
            
            if ( parts.Contains(":") ) {
                /* this is a bitfield! */
                int idxBitfield = parts.IndexOf(":");
                if ( idxBitfield < 0 || idxBitfield >= ( parts.Count - 1 ) ) { throw new Exception( "Bitfield size missing!" ); }
                
                noUsedBits = Convert.ToInt32( parts[idxBitfield + 1] );
                parts.RemoveAt( idxBitfield );
            }
            
            if ( parts.Contains("[") ) {
                /* this is a bitfield! */
                int idxNoElements = parts.IndexOf("[");
                if ( idxNoElements < 0 || idxNoElements >= ( parts.Count - 1 ) ) { throw new Exception( "Size array missing!" ); }
                
                strNoElements = parts[idxNoElements + 1];
                parts.RemoveAt( idxNoElements );
                int idxEOS = parts.IndexOf("]");
                if ( ( idxEOS >= 0 ) && ( idxEOS < parts.Count ) ) {
                    parts.RemoveAt( idxEOS );
                }
            } else {
                strNoElements = "1";
            }
            
            if ( parts.Contains("*") ) {
                /* this is a pointer! */
                int idxAsterix = parts.IndexOf("*");
                if ( ( idxAsterix >= 0 ) && ( idxAsterix < parts.Count ) ) {
                    parts.RemoveAt( idxAsterix );
                }
                isPointer = true;
            } else {
                isPointer = false;
            }
            
            /* Storage class */
            if ( parts.RemoveIfContains( "typedef"  ) ) { this.IsTypedef = true; }
            if ( parts.RemoveIfContains( "extern"   ) ) { }
            if ( parts.RemoveIfContains( "static"   ) ) { }
            if ( parts.RemoveIfContains( "auto"     ) ) { }
            if ( parts.RemoveIfContains( "register" ) ) { }
            
            /* Type qualifiers */
            if ( parts.RemoveIfContains( "const"    ) ) { }
            if ( parts.RemoveIfContains( "volatile" ) ) { }
            if ( parts.RemoveIfContains( "restrict" ) ) { }
            
            /* Data types */
            if ( parts.RemoveIfContains( "void"     ) ) { this.Type = DecType.Void          ; this._elementSizeBits = 32; }
            if ( parts.RemoveIfContains( "char"     ) ) { this.Type = DecType.Char          ; this._elementSizeBits =  8; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "float"    ) ||
                 parts.RemoveIfContains( "single"   ) ) { this.Type = DecType.FloatingPoint ; this._elementSizeBits = 32; }
            if ( parts.RemoveIfContains( "double"   ) ) { this.Type = DecType.FloatingPoint ; this._elementSizeBits = 64; }
            if ( parts.RemoveIfContains(  "int8_t"  ) ) { this.Type = DecType.Integer       ; this._elementSizeBits =  8; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "uint8_t"  ) ) { this.Type = DecType.Integer       ; this._elementSizeBits =  8; this.IsSignedU2 = false; }
            if ( parts.RemoveIfContains(  "int16_t" ) ) { this.Type = DecType.Integer       ; this._elementSizeBits = 16; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "uint16_t" ) ) { this.Type = DecType.Integer       ; this._elementSizeBits = 16; this.IsSignedU2 = false; }
            if ( parts.RemoveIfContains(  "int32_t" ) ) { this.Type = DecType.Integer       ; this._elementSizeBits = 32; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "uint32_t" ) ) { this.Type = DecType.Integer       ; this._elementSizeBits = 32; this.IsSignedU2 = false; }
            if ( parts.RemoveIfContains(  "int64_t" ) ) { this.Type = DecType.Integer       ; this._elementSizeBits = 64; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "uint64_t" ) ) { this.Type = DecType.Integer       ; this._elementSizeBits = 64; this.IsSignedU2 = false; }
            if ( parts.RemoveIfContains( "int"      ) ) { this.Type = DecType.Integer       ; this._elementSizeBits = 32; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "short"    ) ) { this.Type = DecType.Integer       ; this._elementSizeBits = 16; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "long"     ) ) { this.Type = DecType.Integer       ; this.IsSignedU2 = true;
                if ( parts.RemoveIfContains( "long"     ) ) {
                    this._elementSizeBits = 64; //! "long long" so UInt64
                } else {
                    this._elementSizeBits = 32;
                }
            }
            if ( parts.RemoveIfContains( "signed"   ) ) { this.IsSignedU2 = true; }
            if ( parts.RemoveIfContains( "unsigned" ) ) { this.IsSignedU2 = false; }

            if ( parts.RemoveIfContains( "struct"   ) ) { this.Type = DecType.Complex; this.IsStruct = true; }
            if ( parts.RemoveIfContains( "enum"     ) ) { this.Type = DecType.Complex; this.IsEnum  = true; }
            if ( parts.RemoveIfContains( "union"    ) ) { this.Type = DecType.Complex; this.IsUnion = true; }
            
            foreach ( dObject dObj in ownTypes ) {
                
                for ( int idx = 0; idx < parts.Count; idx++ ) {
                    if ( parts[idx].Equals( dObj.Name ) ) {
                        /* This is a type that was defined previously! */
                        dObj.CloneToObject( this );
                        this.IsTypedef  = false;
                        parts.RemoveAt( idx );
                        idx--;
                    }
                }
                if ( parts.Count == 0 ) { break; }
            }
            
            this.IsPointer = isPointer;
            
            try {
                this.NoElements = Convert.ToUInt16( strNoElements );
            } catch {
                bool foundReference = false;
                /* This is not a number, so check for existing reference (enum) */
                for ( int idxDObj = 0; ( idxDObj < ownTypes.Count ) && !foundReference; idxDObj++ ) {
                    if ( !ownTypes[idxDObj].IsEnum ) { continue; }
                    
                    for ( int idx = 0; idx < ownTypes[idxDObj].Body.Count; idx++ ) {
                        if ( ownTypes[idxDObj].Body[idx].Name.Equals( strNoElements ) ) {
                            this.NoElements = (ushort)(ownTypes[idxDObj].Body[idx].Value);
                            foundReference = true;
                            break;
                        }
                    }
                    
                }
            }
            
            if ( parts.Count == 0 ) { return; }
            if ( noUsedBits == -1 ) { noUsedBits = this._elementSizeBits; }
            this.NoBitsUsed = noUsedBits;
            
            /* There should be at least one element that was not found - name */
            this.Name       = parts[0];
            this.Comment    = strComment;
            this.dObjRaw    = strDObjRaw;
        }
        
        
        public void RebuildPattern() {
            List<dObject> fields = new List<dObject>();
            DecoderGui decGui = MainListener.GetCurrGui();
            /* TODO restore this functionality */
            
//            foreach ( dObject orgField in this.Body ) {
//                List<dObject> fieldsSub = dObject.TryDecodeField( orgField.dObjRaw, decGui._decoders );
//                foreach ( dObject field in fieldsSub ) {
//                    if ( field == null ) { continue; }
//                    fields.Add( field );
//                }
//            }
//            this.Body.Clear();
//            this.Body = fields;
        }
    }
}
