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
        
        public List<dObject> _ownTypes = new List<dObject>();
        
        public dParser() {
            
        }
        
        
        public dObject ParseSource(string strSrc) {
            /* 1. Process PRECOMPILERS */
            ProcessPrecompilers( ref strSrc );
            
            dObject root = new dObject();
            /* 2. Get into srouce header file */
            RemoveComments( ref strSrc );
            ParseBlock(strSrc, ref root, ref _ownTypes);
            if ( root.Body[0].Count == 1 ) {
                root = root.Body[0][0];
            }
            return root;
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
                idxFirstSemicolon = strSrc.IndexOf(';');
                idxFirstSOS = strSrc.IndexOf('{');
                
                if ( idxFirstSOS > 0 ) {
                    /* Find parity EOS */
                    int idxEOS = strSrc.FindParityIndex( '{', '}' );
                    if ( ( idxEOS < 0 ) || ( idxEOS >= strSrc.Length ) ) { throw new Exception( "Parity not found for ATOX in: \r\n\"\r\n" + strSrc + "\r\n\"\r\n" ); }
                }
                
                dObject atox = new dObject();
                
                /* If idxFirstScopeEntry < idxFirstSemicolon that means this ATOX has body! So proceed it */
                if ( ( idxFirstSOS > 0 ) && ( idxFirstSOS < idxFirstSemicolon ) ) {
                    /* Find parity EOS */
                    int idxEOS = strSrc.FindParityIndex( '{', '}' );
                    if ( ( idxEOS < 0 ) || ( idxEOS >= strSrc.Length ) ) { throw new Exception( "Parity not found for ATOX in: \r\n\"\r\n" + strSrc + "\r\n\"\r\n" ); }
                    /* Parse its childs (only its scope) */
                    string atoxScope = strSrc.Substring( idxFirstSOS + 1, idxEOS - idxFirstSOS - 1 );
                    ParseBlock( atoxScope, ref atox, ref ownTypes );
                    strSrc = strSrc.Substring( 0, idxFirstSOS) + strSrc.Substring( idxEOS + 1 );
                    
                    idxEndAtox = strSrc.IndexOf(';') + 1;
                } else if( ( idxFirstSemicolon > 0 ) && ( idxFirstSemicolon < strSrc.Length ) ) {
                    idxEndAtox = idxFirstSemicolon + 1;
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
                    parent.Body[0].Add( atox );
                    if ( atox.IsTypedef ) {
                        dObject prevAtox = ownTypes.Find( o => o.Name == atox.Name );
                        if ( prevAtox == null ) {
                            ownTypes.Add( atox );
                        } else {
                            /* Replace with new instance */
                            prevAtox = atox;
                        }
                    }
                }
                
                if ( idxEndAtox >= strSrc.Length ) { break; }
                if ( idxEndAtox < 0 ) { idxEndAtox = 0;  }
                /* Get only not processed part */
                strSrc = strSrc.Substring( idxEndAtox );
            }
        }
        
        
        void ProcessPrecompilers( ref string strSrc ) {
            RemoveSeqToEndLine( ref strSrc, "#if" );
            RemoveSeqToEndLine( ref strSrc, "#el" );
            RemoveSeqToEndLine( ref strSrc, "#en" );
            RemoveSeqToEndLine( ref strSrc, "#include" );
            
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
        
        void RemoveComments( ref string strSrc ) {
            RemoveSeqToEndLine( ref strSrc, "//" );
            int idxCommentStart = 0;
            while ( ( idxCommentStart = strSrc.IndexOf("/*", idxCommentStart) ) >= 0 ) {
                int idxCommentEnd = strSrc.IndexOf( "*/" );
                if ( ( idxCommentEnd > idxCommentStart ) && ( idxCommentEnd < strSrc.Length - 2 ) ) {
                    strSrc = strSrc.Substring( 0, idxCommentStart ) +
                        strSrc.Substring( idxCommentEnd + 2 );
                }
            }
        }
        
        void RemoveSeqToEndLine( ref string strSrc, string strSeq ) {
            int             idxStart    = 0;
            int             idxNewLine  = -1;
            
            while ( ( idxStart = strSrc.IndexOf( strSeq, idxStart ) ) >= 0 ) {
                idxNewLine = strSrc.IndexOfAny( new char[] { '\r', '\n' }, idxStart );
                if ( idxNewLine < 0 || idxStart > strSrc.Length ) {
                    idxNewLine = strSrc.Length - 1;
                }
                strSrc = strSrc.Substring(0, idxStart) + strSrc.Substring( idxNewLine );
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
            
            try {
                m.Value = UInt64Expression.Evaluate( m.ValueRaw ).ToString();
            } catch { m.Value = m.ValueRaw; }
            return m;
        }
    }
}
