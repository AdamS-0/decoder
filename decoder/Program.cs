/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 16.08.2026
 * Time: 07:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace decoder {
    class Program {
        
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();
        const uint ATTACH_PARENT_PROCESS = 0xFFFFFFFF;

        
        [STAThread]
        private static void Main(string[] args) {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            Init( args );
        }
        
        static void RestartConsoleInOutStreams () {
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput(), Console.OutputEncoding);
            standardOutput.AutoFlush = true;
            Console.SetOut( standardOutput );

            StreamWriter standardError = new StreamWriter(Console.OpenStandardError(), Console.OutputEncoding);
            standardError.AutoFlush = true;
            Console.SetError( standardError );
        }
        
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Init(string[] args){
            bool gotConsole = false;
            if ( AttachConsole(ATTACH_PARENT_PROCESS) ) {
                RestartConsoleInOutStreams();
                gotConsole = true;
            }
            
            Options opt = CommandLineParser.CommandLineParser.ParseArguments<Options>(args);
            
            if ( opt.help    != null ) { CommandLineParser.CommandLineParser.PrintHelp<Options>(); }
            if ( opt.version != null ) { CommandLineParser.CommandLineParser.PrintVersion<Options>(); }
            
            if ( !gotConsole && ( opt.noGui != null ) ) {
                /* Required console, so create one */
                AllocConsole();
                RestartConsoleInOutStreams();
            }
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainListener listener = new MainListener();
            
            Application.Run( listener );

        }
        
        private static Assembly CurrentDomain_AssemblyResolve ( object sender, ResolveEventArgs args ) {
            string assemblyName = new AssemblyName(args.Name).Name;
            string resourceName = "decoder." + assemblyName + ".dll";
            Console.WriteLine( "Looking for: " + assemblyName + "." );

            using ( var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream( resourceName ) ) {
                if ( stream == null ) return null;
                byte[] assemblyData = new byte[stream.Length];
                stream.Read( assemblyData, 0, assemblyData.Length );
                return Assembly.Load( assemblyData );
            }
        }
    }
}