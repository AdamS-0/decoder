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
        
        private static void PrepareInputString(ref string strAtoxParams) {
            /* Cut off ';' and multiple spaces */
            strAtoxParams = strAtoxParams.Replace( "\r", " " );
            strAtoxParams = strAtoxParams.Replace( "\n", " " );
            strAtoxParams = strAtoxParams.Replace( "\t", " " );
            strAtoxParams = strAtoxParams.Replace( "*", " * " );
            strAtoxParams = strAtoxParams.Replace( "[", " [ " );
            strAtoxParams = strAtoxParams.Replace( "]", " ] " );
            strAtoxParams = _rgxSpaceCutter.Replace( strAtoxParams, " " );
            strAtoxParams = strAtoxParams.Replace( ";", "" );
            strAtoxParams = strAtoxParams.Replace( ",", "" );
        }
        
        private static string GetCommentsFromStrAtox(ref string strAtoxParams) {
            string strComment = "";
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
            
            return strComment;
        }
        
        private static void RemoveEmptyParts(ref List<string> parts) {
            for ( int idx = 0; idx < parts.Count; idx++ ) {
                if ( parts[idx].Length == 0 ) {
                    parts.RemoveAt( idx );
                    idx--;
                }
            }
        }
        
        private void ProcessBasicKeyWords(ref List<string> parts) {
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
        }
        
        private void ProcessComplexEntities(ref List<string> parts, ref List<dObject> ownTypes) {
            if ( this.IsTypedef && this.IsTypedef ) { return; }
            foreach ( dObject dObj in ownTypes ) {
                for ( int idx = 0; idx < parts.Count; idx++ ) {
                    if ( parts[idx].Equals( dObj.Name ) ) {
                        /* This is a type that was defined previously! */
                        dObj.CloneToObject( this );
                        this.TypeName = dObj.Name;
                        this.IsTypedef  = false;
                        parts.RemoveAt( idx );
                        idx--;
                    }
                }
                if ( parts.Count == 0 ) { break; }
            }
        }
        
        private void TryProcessAsArray(string strNoElements, ref List<dObject> ownTypes) {
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
        }
        
        private void TryGetNoUsedBits(ref int noUsedBits, ref List<string> parts) {
            if ( parts.Contains(":") ) {
                /* this is a bitfield! */
                int idxBitfield = parts.IndexOf(":");
                if ( idxBitfield < 0 || idxBitfield >= ( parts.Count - 1 ) ) { throw new Exception( "Bitfield size missing!" ); }
                
                noUsedBits = Convert.ToInt32( parts[idxBitfield + 1] );
                parts.RemoveAt( idxBitfield );
            }
        }
        
        private void TryGetNoElements(ref string strNoElements, ref List<string> parts) {
            if ( parts.Contains("[") ) {
                /* this is an array! */
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
        }
        
        private bool GetIsPointerType(ref List<string> parts) {
            if ( parts.Contains("*") ) {
                /* this is a pointer! */
                int idxAsterix = parts.IndexOf("*");
                if ( ( idxAsterix >= 0 ) && ( idxAsterix < parts.Count ) ) {
                    parts.RemoveAt( idxAsterix );
                }
                return true;
            }
            return false;
        }
        
        private void ProcessEnumTypedef(ref List<dObject> ownTypes) {
            int enumerator = 0;
                for ( int idxBody = 0; idxBody < this.Body.Count; idxBody++ ) {
                    if ( this.Body[idxBody].Value == null ) {
                        this.Body[idxBody].Value = enumerator;
                    } else {
                        try {
                            enumerator = Convert.ToInt32( this.Body[idxBody].Value );
                        } catch {
                            /* This is not a number, so check for existing reference (enum) */
                            if ( this.Body[idxBody].Value == null ) { break; }
                            if ( this.Body[idxBody].Value.GetType() != typeof(string) ) { break; }
                            /* Make sure we parse only string */
                            bool foundReference = false;
                            string strLastEnumVal = this.Body[idxBody].Value.ToString();
                            try {
                                for ( int idxDObj = 0; ( idxDObj < ownTypes.Count ) && !foundReference; idxDObj++ ) {
                                    if ( !ownTypes[idxDObj].IsEnum ) { continue; }
                                    
                                    for ( int idx = 0; idx < ownTypes[idxDObj].Body.Count; idx++ ) {
                                        if ( ownTypes[idxDObj].Body[idx].Name.Equals( strLastEnumVal ) ) {
                                            enumerator = Convert.ToInt32( ownTypes[idxDObj].Body[idx].Value );
                                            foundReference = true;
                                            break;
                                        }
                                    }
                                }
                            } catch {}
                        }
                    }
                    enumerator++;
                }
        }
        
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
            this._elementSizeBits   = 32;
            
            strComment = GetCommentsFromStrAtox(ref strAtoxParams);
            PrepareInputString(ref strAtoxParams);
            
            string[] partsArr = strAtoxParams.Split(' ');
            if ( partsArr.Length == 0 ) { return; }
            
            List<string> parts = new List<string>(partsArr);
            RemoveEmptyParts( ref parts );
            
            if ( partsArr.Length == 0 ) { return; }
            
            TryGetNoUsedBits(ref noUsedBits, ref parts);
            TryGetNoElements(ref strNoElements, ref parts);
            isPointer = GetIsPointerType(ref parts);
            
            ProcessBasicKeyWords( ref parts );
            ProcessComplexEntities( ref parts, ref ownTypes );
            TryProcessAsArray(strNoElements, ref ownTypes);
            this.IsPointer = isPointer;
            if ( this.IsPointer ) {
                this._elementSizeBits = 32;
                this.NoBitsUsed = 32;
            }
            
            if ( this.IsEnum && this.IsTypedef ) { ProcessEnumTypedef( ref ownTypes ); }
            
            this.Comment = strComment;
            this.dObjRaw = strDObjRaw;
            
            if ( parts.Count == 0 ) {
                this.Name = "?";
                if ( this.IsUnion ) { this.Name += "UNION"; }
                else if ( this.IsStruct ) { this.Name += "STRUCT"; }
                this.Name += "?";
                return;
            }
            if ( noUsedBits == -1 ) { noUsedBits = this._elementSizeBits; }
            this.NoBitsUsed = noUsedBits;
            
            /* There should be at least one element that was not recognised - name */
            this.Name = parts[0];
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
