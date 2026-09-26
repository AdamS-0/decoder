/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 05.09.2026
 * Time: 15:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace decoder {
    /// <summary>
    /// Description of ListExtension.
    /// </summary>
    public static class ListExtension {
        public static bool RemoveIfContains<T>(this List<T> list, T item) {
            for ( int idx = 0; idx < list.Count; idx++ ) {
                if ( list[idx].Equals(item) ) {
                    list.RemoveAt(idx);
                    return true;
                }
            }
            return false;
        }
    }
    
    
    public static class StringExtensions {
        /// <summary>
        /// Finds index of parity correct end of scope
        /// </summary>
        /// <param name="strIn"></param>
        /// <param name="charSOS"></param>
        /// <param name="charEOS"></param>
        /// <param name="idxStart"></param>
        /// <returns>Index of parity scope EOS, or -1 when not found</returns>
        public static int FindParityIndex(this string strIn, char charSOS, char charEOS, int idxStart = 0) {
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
        
    }
}
