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
            
            /* Cut off attributes like "__attribute__((packed))" */
            const string strAttr = "__attribute__";
            int idxAttr = strAtoxParams.IndexOf(strAttr);
            if ( idxAttr >= 0 ) {
                int idxEOA = strAtoxParams.FindParityIndex( '(', ')', idxAttr );
                if ( ( idxEOA > 0 ) && ( idxEOA < strAtoxParams.Length ) ) {
                    strAtoxParams = strAtoxParams.Substring( 0, idxAttr ) + 
                        strAtoxParams.Substring( idxEOA + 1 );
                }
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
            strAtoxParams = strAtoxParams.Replace( ",", "" );
            
            /* Add extra spaces before and after ':' */
            strAtoxParams = strAtoxParams.Replace( ":", " : " );
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
            if ( parts.RemoveIfContains( "void"     ) ) { this.Type = DecType.Void          ; this.ElementSizeBits = 32; }
            if ( parts.RemoveIfContains( "char"     ) ) { this.Type = DecType.Char          ; this.ElementSizeBits =  8; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "float"    ) ||
                 parts.RemoveIfContains( "single"   ) ) { this.Type = DecType.FloatingPoint ; this.ElementSizeBits = 32; }
            if ( parts.RemoveIfContains( "double"   ) ) { this.Type = DecType.FloatingPoint ; this.ElementSizeBits = 64; }
            if ( parts.RemoveIfContains(  "int8_t"  ) ) { this.Type = DecType.Integer       ; this.ElementSizeBits =  8; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "uint8_t"  ) ) { this.Type = DecType.Integer       ; this.ElementSizeBits =  8; this.IsSignedU2 = false; }
            if ( parts.RemoveIfContains(  "int16_t" ) ) { this.Type = DecType.Integer       ; this.ElementSizeBits = 16; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "uint16_t" ) ) { this.Type = DecType.Integer       ; this.ElementSizeBits = 16; this.IsSignedU2 = false; }
            if ( parts.RemoveIfContains(  "int32_t" ) ) { this.Type = DecType.Integer       ; this.ElementSizeBits = 32; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "uint32_t" ) ) { this.Type = DecType.Integer       ; this.ElementSizeBits = 32; this.IsSignedU2 = false; }
            if ( parts.RemoveIfContains(  "int64_t" ) ) { this.Type = DecType.Integer       ; this.ElementSizeBits = 64; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "uint64_t" ) ) { this.Type = DecType.Integer       ; this.ElementSizeBits = 64; this.IsSignedU2 = false; }
            if ( parts.RemoveIfContains( "int"      ) ) { this.Type = DecType.Integer       ; this.ElementSizeBits = 32; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "short"    ) ) { this.Type = DecType.Integer       ; this.ElementSizeBits = 16; this.IsSignedU2 = true;  }
            if ( parts.RemoveIfContains( "long"     ) ) { this.Type = DecType.Integer       ; this.IsSignedU2 = true;
                if ( parts.RemoveIfContains( "long"     ) ) {
                    this.ElementSizeBits = 64; //! "long long" so UInt64
                } else {
                    this.ElementSizeBits = 32;
                }
            }
            if ( parts.RemoveIfContains( "signed"   ) ) { this.IsSignedU2 = true; }
            if ( parts.RemoveIfContains( "unsigned" ) ) { this.IsSignedU2 = false; }

            if ( parts.RemoveIfContains( "struct"   ) ) { this.Type = DecType.Complex; this.IsStruct = true; }
            if ( parts.RemoveIfContains( "enum"     ) ) { this.Type = DecType.Complex; this.IsEnum  = true; }
            if ( parts.RemoveIfContains( "union"    ) ) { this.Type = DecType.Complex; this.IsUnion = true; }
        }
        
        private void ProcessComplexEntities(ref List<string> parts, ref List<dObject> ownTypes) {
            if ( this.IsTypedef ) { return; }
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
                    
                    for ( int idx = 0; idx < ownTypes[idxDObj].Body[0].Count; idx++ ) {
                        if ( ownTypes[idxDObj].Body[0][idx].Name.Equals( strNoElements ) ) {
                            this.NoElements = (ushort)(ownTypes[idxDObj].Body[0][idx].Value);
                            foundReference = true;
                            break;
                        }
                    }
                }
            }
            
            if ( ( this.Type == DecType.Complex ) && ( this.NoElements > 1 ) ) {
                List<List<dObject>> bodyTemp = this.Body;
                this.Body = new List<List<dObject>>();
                
                for (int idxElement = 0; idxElement < this.NoElements; idxElement++ ) {
                    List<dObject> elBodyList = new List<dObject>();
                    this.Body.Add( elBodyList );
                    for (int idx = 0; idx < bodyTemp[0].Count; idx++ ) {
                        elBodyList.Add( (dObject)bodyTemp[0][idx].Clone() );
                    }
                }
            }
        }
        
        private void TryGetValueAfterEqualsSign(ref List<string> parts) {
            if ( parts.Contains("=") ) {
                /* this is a bitfield! */
                int idxEqualsSign = parts.IndexOf("=");
                if ( idxEqualsSign < 0 || idxEqualsSign >= ( parts.Count - 1 ) ) { throw new Exception( "Value missing!" ); }
                
                string val = "";
                while ( parts.Count > ( idxEqualsSign + 1 ) ) {
                    val += parts[idxEqualsSign + 1] + " ";
                    parts.RemoveAt( idxEqualsSign + 1 );
                }
                
                this.Value = Convert.ToInt32( val );
                parts.RemoveAt( idxEqualsSign );
            }
        }
        
        private void TryGetNoUsedBits(ref ushort noUsedBits, ref List<string> parts) {
            if ( parts.Contains(":") ) {
                /* this is a bitfield! */
                int idxBitfield = parts.IndexOf(":");
                if ( idxBitfield < 0 || idxBitfield >= ( parts.Count - 1 ) ) { throw new Exception( "Bitfield size missing!" ); }
                
                noUsedBits = Convert.ToUInt16( parts[idxBitfield + 1] );
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
        
        private void ProcessEnumTypedef( ref ushort noUsedBits, ref List<dObject> ownTypes) {
            int enumerator = 0;
            for ( int idxBody = 0; idxBody < this.Body[0].Count; idxBody++ ) {
                if ( this.Body[0][idxBody].Value == null ) {
                    this.Body[0][idxBody].Value = enumerator;
                } else {
                    try {
                        enumerator = Convert.ToInt32( this.Body[0][idxBody].Value );
                    } catch {
                        /* This is not a number, so check for existing reference (enum) */
                        if ( this.Body[0][idxBody].Value == null ) { break; }
                        if ( this.Body[0][idxBody].Value.GetType() != typeof(string) ) { break; }
                        /* Make sure we parse only string */
                        bool foundReference = false;
                        string strLastEnumVal = this.Body[0][idxBody].Value.ToString();
                        try {
                            for ( int idxDObj = 0; ( idxDObj < ownTypes.Count ) && !foundReference; idxDObj++ ) {
                                if ( !ownTypes[idxDObj].IsEnum ) { continue; }
                                
                                for ( int idx = 0; idx < ownTypes[idxDObj].Body[0].Count; idx++ ) {
                                    if ( ownTypes[idxDObj].Body[0][idx].Name.Equals( strLastEnumVal ) ) {
                                        enumerator = Convert.ToInt32( ownTypes[idxDObj].Body[0][idx].Value );
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
            if ( enumerator > 1 ) { enumerator--; }
            
            int noBitsRequired = 0;
            while ( enumerator != 0 ) {
                enumerator /= 2;
                noBitsRequired++;
            }
            
            if ( noUsedBits == 0 ) {
                noUsedBits = 32;
//                noUsedBits = (ushort)noBitsRequired;
//                this.ElementSizeBits = (ushort)( 8 * ( (noUsedBits / 8) + 1) );
            }
        }
        
        /// <summary>
        /// Parse parameters of given axtox's params string, e.g. "typedef struct MyStruct_t;" without its body of course
        /// </summary>
        /// <param name="atox"></param>
        /// <param name="strAtoxParams"></param>
        public void ParseParamsOfAtox(string strAtoxParams, ref List<dObject> ownTypes) {
            
            if ( strAtoxParams.Length == 0 ) { return; }
            
            bool    isPointer       = false;
            ushort  noUsedBits      = 0;
            string  strNoElements   = "1";
            string  strComment      = "";
            string  strDObjRaw      = strAtoxParams;
            this.ElementSizeBits    = 0;
            
            strComment = GetCommentsFromStrAtox(ref strAtoxParams);
            PrepareInputString(ref strAtoxParams);
            
            string[] partsArr = strAtoxParams.Split(' ');
            if ( partsArr.Length == 0 ) { return; }
            
            List<string> parts = new List<string>(partsArr);
            RemoveEmptyParts( ref parts );
            
            if ( parts.Count == 0 ) { return; }
            
            TryGetValueAfterEqualsSign(ref parts);
            
            TryGetNoUsedBits(ref noUsedBits, ref parts);
            TryGetNoElements(ref strNoElements, ref parts);
            isPointer = GetIsPointerType(ref parts);
            
            ProcessBasicKeyWords( ref parts );
            ProcessComplexEntities( ref parts, ref ownTypes );
            
            if ( this.IsStruct ) {
                int elSize = 0;
                for ( int idx = 0; idx < this.Body[0].Count; idx++ ) {
                    elSize += this.Body[0][idx].NoElements * this.Body[0][idx].ElementSizeBits;
                }
                this.ElementSizeBits = (ushort)elSize;
            } else if ( this.IsUnion ) {
                int elSize = 0, elSizeMax = 0;
                for ( int idx = 0; idx < this.Body[0].Count; idx++ ) {
                    elSize = this.Body[0][idx].NoElements * this.Body[0][idx].ElementSizeBits;
                    elSizeMax = Math.Max( elSizeMax, elSize );
                }
                this.ElementSizeBits = (ushort)elSizeMax;
            }
            
            TryProcessAsArray(strNoElements, ref ownTypes);
            this.IsPointer = isPointer;
            if ( this.IsPointer ) {
                this.ElementSizeBits = 32;
                this.NoBitsUsed = 32;
            }
            
            if ( this.IsTypedef && this.IsEnum ) { ProcessEnumTypedef( ref noUsedBits, ref ownTypes ); }
            if ( this.IsTypedef && this.IsStruct ) {
                int elSize = 0;
                for ( int idx = 0; idx < this.Body[0].Count; idx++ ) {
                    elSize += this.Body[0][idx].GetTotalSizeInBits();
                }
                this.ElementSizeBits = (ushort)elSize;
            }
            
            if ( this.IsUnion && ( this.TypeName.Length == 0 ) ) { this.TypeName = "union"; }
            
            this.Comment = strComment;
            this.dObjRaw = strDObjRaw;
            
            bool isAnonStruct   = false;
            bool isAnonUnion    = false;
            if ( parts.Count == 0 ) {
                this.Name = "?";
                if ( this.IsUnion ) { this.Name += "UNION"; isAnonUnion = true; }
                else if ( this.IsStruct ) { this.Name += "STRUCT"; isAnonStruct = true; }
                this.Name += "?";
            }
            
            if ( isAnonStruct ) {
                int elSize = 0;
                for ( int idx = 0; idx < this.Body[0].Count; idx++ ) {
                    elSize += this.Body[0][idx].GetTotalSizeInBits();
                }
                this.ElementSizeBits = (ushort)elSize;
            } else if ( isAnonUnion ) {
                int elSize = 0;
                int elSizeMax = 0;
                for ( int idx = 0; idx < this.Body[0].Count; idx++ ) {
                    elSize = this.Body[0][idx].GetTotalSizeInBits();
                    if ( elSizeMax < elSize ) { elSizeMax = elSize; }
                }
                this.ElementSizeBits = (ushort)elSizeMax;
            }
            
            
            if ( noUsedBits == 0 ) { noUsedBits = this.ElementSizeBits; }
            else if ( this.ElementSizeBits == 0 ) { this.ElementSizeBits = noUsedBits; }
            this.NoBitsUsed = noUsedBits;
            
            if ( parts.Count > 0 ) {
                /* There should be at least one element that was not recognised - name */
                this.Name = parts[0];
            }
        }
        
    }
}
