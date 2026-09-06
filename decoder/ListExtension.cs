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
}
