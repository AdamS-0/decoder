/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 16.08.2026
 * Time: 07:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;
using CommandLineParser;

namespace decoder{

    [AppInfo(
        AppName = AppInfo.Name,
        AppVersion = AppInfo.Version,
        AppAuthor = AppInfo.Author,
        AppDescription = AppInfo.Description)]
    public class Options {
        [Parameter(ParameterAlias = "h", HelpText = "Show this page", ArgumentRequired = false)]
        public string help { get; set; }
        
        [Parameter(ParameterAlias = "v", HelpText = "Show version of the application", ArgumentRequired = false)]
        public string version { get; set; }
        
        [Parameter(HelpText = "Do not show gui",
                   ArgumentName = "", ArgumentRequired = false)]
        public string noGui { get; set; }
        

    }
}
