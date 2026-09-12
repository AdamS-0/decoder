/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 05.09.2026
 * Time: 07:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace decoder {
    /// <summary>
    /// Description of dParser.
    /// </summary>
    public class dParser {
        protected static readonly Regex _rgxSpaceCutter = new Regex(@"\s+", RegexOptions.Compiled);
        
        List<dObject> _ownTypes = new List<dObject>();
        
        public dParser() {
            
        }
        
        
        public dObject ParseSource(string strSrc) {
            /* 1. Process PRECOMPILERS */
            ProcessPrecompilers( ref strSrc );
            
            dObject root = new dObject();
            /* 2. Get into srouce header file */
            try {
                ParseBlock(strSrc, ref root, ref _ownTypes);
                if ( root.Body.Count == 1 ) {
                    root = root.Body[0];
                }
                return root;
            } catch { }
            return null;
        }
        
        /// <summary>
        /// Block is defined as "[Specifiers] [Name] [{ Body }];", where [...] is optional
        /// </summary>
        /// <param name="strSrc"></param>
        /// <param name = "parent"></param>
        /// <param name = "ownTypes"></param>
        /// <returns></returns>
        void ParseBlock(string strSrc, ref dObject parent, ref List<dObject> ownTypes) {
            
            while( strSrc.Length > 0 ) {
                strSrc = strSrc.Trim( new char[]{'\r', '\n', '\t', ' '} );
                
                int idxFirstSemicolon   = -1;
                int idxFirstSOS         = -1;
                int idxEndAtox          = -1;
                
                /* Find out which is first */
                try {
                    idxFirstSemicolon = strSrc.IndexOf(';');
                    idxFirstSOS = strSrc.IndexOf('{');
                } catch {}
//                if ( ( idxFirstSemicolon == -1 ) && ( idxFirstSOS == -1 ) ) { break; }
                
                dObject atox = new dObject();
                
                /* If idxFirstScopeEntry < idxFirstSemicolon that means this ATOX has body! So proceed it */
                if ( ( idxFirstSOS > 0 ) && ( idxFirstSOS < idxFirstSemicolon ) ) {
                    /* Find parity EOS */
                    int idxEOS = FindParityIndex( strSrc, '{', '}' );
                    if ( ( idxEOS < 0 ) || ( idxEOS >= strSrc.Length ) ) { throw new Exception( "Not found parity of ATOX in: \r\n" + strSrc ); }
                    /* Parse its childs (only its scope) */
                    string atoxScope = strSrc.Substring( idxFirstSOS + 1, idxEOS - idxFirstSOS - 1 );
                    ParseBlock( atoxScope, ref atox, ref ownTypes );
                    strSrc = strSrc.Substring( 0, idxFirstSOS) + strSrc.Substring( idxEOS + 1 );
                    
                    idxEndAtox = strSrc.IndexOf(';') + 1;
                    {
                        int idxCommentEnd = -1;
                        int idxNewLine = strSrc.IndexOf('\n', idxEndAtox);
                        idxNewLine = ( idxNewLine == -1 ) ? (strSrc.Length) : idxNewLine;
                        
                        /* Find one line comment after definition line */
                        int idxCommentStart = strSrc.Substring(0, idxNewLine).IndexOf("//");
                        
                        if ( idxCommentStart == -1 ) {
                            /* Not found line comment here, so check for start of block comment */
                            idxCommentStart = strSrc.Substring(0, idxNewLine).IndexOf("/*");
                            if ( idxCommentStart >= 0 ) {
                                /* This is block comment! */
                                idxCommentEnd = strSrc.IndexOf("*/", idxCommentStart);
                                if      ( idxCommentEnd >=  0 ) { idxCommentEnd += 2; }
                                else if ( idxCommentEnd == -1 ) { idxCommentEnd = idxNewLine; }
                                idxEndAtox = idxCommentEnd;
                            }
                        } else {
                            /* This is one line comment */
                            idxEndAtox = idxNewLine;
                        }
                    }
                } else if( ( idxFirstSemicolon > 0 ) && ( idxFirstSemicolon < strSrc.Length ) ) {
                    int idxNewLine = strSrc.IndexOf('\n', idxFirstSemicolon);
                    if ( idxNewLine == -1 ) { idxNewLine = strSrc.Length - 1; }
                    int idxCommentStart = strSrc.IndexOf("//");
                    
                    int idxCommentEnd = -1;
                    
                    if ( idxCommentStart == -1 ) {
                        idxCommentStart = strSrc.IndexOf("/*");
                        if ( idxCommentStart >= 0 ) {
                            /* This is block comment! */
                            idxCommentEnd = strSrc.IndexOf("*/", idxCommentStart);
                            if ( idxCommentEnd > 0 ) { idxCommentEnd += 1; }
                            if ( idxCommentEnd == -1) { idxCommentEnd = idxNewLine; }
                        }
                    } else {
                        /* This is one line comment */
                        if ( idxCommentStart < idxNewLine ) { idxCommentEnd = idxNewLine; }
                    }
                    
                    if ( ( idxCommentStart >= 0 ) && ( idxCommentStart < idxNewLine ) ) {
                        /* Found any comment within this sentence */
                        if ( idxCommentEnd == -1 ) { idxCommentEnd = strSrc.Length - 1; }
                        idxEndAtox = idxCommentEnd + 1;
                    } else {
                        idxEndAtox = idxFirstSemicolon + 1;
                    }
                } else {
                    /* Not foudn any '{' NOR ';', so it may be an enum body */
                    int idxComa = strSrc.IndexOf(',') + 1;
                    if ( ( idxComa <= 0 ) || ( idxComa >= strSrc.Length ) ) {
                        idxEndAtox = strSrc.Length;
                    } else {
                        idxEndAtox = idxComa;
                    }
                }
                
                /* Parse parameters (Specifiers & Name) of this ATOX */
                string strAtoxParams = "";
                if ( ( idxEndAtox >= 0 ) && ( idxEndAtox <= strSrc.Length ) ) {
                    strAtoxParams = strSrc.Substring( 0, idxEndAtox );
                    strAtoxParams = strAtoxParams.Trim(' ');
                }
                
                if ( strAtoxParams.Length > 0 ) {
                    /* Thjere were no errors during proceeding, so add this ATOX to his parent's body */
                    atox.ParseParamsOfAtox( strAtoxParams, ref ownTypes );
                    parent.Body.Add( atox );
                    if ( atox.IsTypedef ) {
                        ownTypes.Add( atox );
                    }
                }
                
                if ( idxEndAtox >= strSrc.Length ) { break; }
                if ( idxEndAtox < 0 ) { idxEndAtox = 0;  }
                /* Get only not processed part */
                strSrc = strSrc.Substring( idxEndAtox );
            }
        }
        
        /// <summary>
        /// Finds index of parity correct end of scope
        /// </summary>
        /// <param name="strIn"></param>
        /// <param name="charSOS"></param>
        /// <param name="charEOS"></param>
        /// <param name="idxStart"></param>
        /// <returns>Index of parity scope EOS, or -1 when not found</returns>
        public static int FindParityIndex(string strIn, char charSOS, char charEOS, int idxStart = 0) {
            int cntrParity = 0;
            for ( int idx = idxStart; idx < strIn.Length; idx++ ) {
                if ( strIn[idx] == charSOS ) { cntrParity++; }
                else if ( strIn[idx] == charEOS ) {
                    cntrParity--;
                    if ( cntrParity == 0 ) {
                        return idx;
                    }
                }
            }
            return -1;
        }
        
        
        void ProcessPrecompilers( ref string strSrc ) {
            const string strMacroBase = "#define";
            
            int idxMacroStart = -1;
            int idxEndLine = -1;
            while ( ( idxMacroStart = strSrc.IndexOf( strMacroBase ) ) >= 0 ) {
                idxEndLine = strSrc.IndexOf( '\n', idxMacroStart );
                if ( idxEndLine == -1 ) { idxEndLine = strSrc.Length; }
                /* Found macro */
                /* TODO At this moment only one line macros are supported */
                string strMacro = strSrc.Substring( idxMacroStart, idxEndLine - idxMacroStart );
                
                /* Cut off found macro */
                strSrc = strSrc.Substring(0, idxMacroStart) + strSrc.Substring( idxEndLine + 1 );
                
                Macro macro = Macro.ParseStr( strMacro );
                if ( macro == null ) { continue; }
                strSrc = Regex.Replace( strSrc, "\\b" + macro.Name + "\\b", macro.Value );
            }
        }
    }
    
    class Macro {
        public string Name      = "";
        public string Value     = "";
        public string ValueRaw  = "";
        
        public static Macro ParseStr(string strRawMacro) {
            int idxSpace = strRawMacro.IndexOf( ' ' );
            if ( idxSpace == -1 ) { return null; }
            strRawMacro = strRawMacro.Substring( idxSpace + 1 );
            
            if ( ( idxSpace = strRawMacro.IndexOf( ' ' ) ) == -1 ) { return null; }
            
            Macro m     = new Macro();
            m.Name      = strRawMacro.Substring( 0, idxSpace ).Trim(' ');
            m.ValueRaw  = strRawMacro.Substring( idxSpace );
            
            m.ValueRaw = m.ValueRaw.Replace( '\r', ' ' );
            m.ValueRaw = m.ValueRaw.Replace( '\t', ' ' );
            m.ValueRaw = m.ValueRaw.Replace( '\n', ' ' );
            
            DataTable dt = new DataTable();
            try {
                m.Value = dt.Compute( m.ValueRaw, "" ).ToString();
            } catch (Exception ex) { m.Value = m.ValueRaw; }
            return m;
        }
    }
}
