/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 16.08.2026
 * Time: 07:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace decoder {
    /// <summary>
    /// Description of AppInfo.
    /// </summary>
    public class AppInfo {
        
        public const string Name = "decoder";
        public const string Version = "0.1.0-" + vn.BUILD_NUM + "_" +
            vn.BUILD_YEAR + vn.BUILD_MON + vn.BUILD_DAY + "." +
            vn.BUILD_HOUR + vn.BUILD_MIN;
        public const string Author = "AdamS-0 2026";
        public const string Description = "Frame decoder";
        
        public AppInfo() {
        }
    }
}
