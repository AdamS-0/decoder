/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 16.08.2026
 * Time: 07:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace decoder {
    class Program {
        public static void Main ( string[] args ) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Options opt = CommandLineParser.CommandLineParser.ParseArguments<Options>(args);
            
            if ( opt.help    != null ) { CommandLineParser.CommandLineParser.PrintHelp<Options>(); }
            if ( opt.version != null ) { CommandLineParser.CommandLineParser.PrintVersion<Options>(); }
            
            string strStructC1 = "" +
                "typedef struct {" +
//                "    uint8_t  Sync1;" +
//                "    uint8_t  Sync2;" +
//                "    uint8_t  Class;" +
//                "    uint8_t  Id;" +
//                "    uint16_t  Length;" +
//                "    uint32_t  iTow;" +
//                "    uint16_t  year;" +
//                "    uint8_t   month;" +
//                "    uint8_t   day;" +
//                
//                "    uint16_t siemka : 4;" +
//                "    uint16_t reserved : 12;" +
//                "    float    *pFloat; \\ wskaznik do floatu!" +
//                "    char*    pName; /* Nazwa */" +
                "    char     Name[10]; /* Nazwa2 */" +
                "    char     Name[10]; /* Nazwa2 */" +
                "    char     Name[10]; /* Nazwa2 */" +
                "    char     Name[10]; /* Nazwa2 */" +
                "    char     Name[10]; /* Nazwa2 */" +
                "    char     Name[10]; /* Nazwa2 */" +
                "} testStruct_t;";
            
            Decoder dec1 = new Decoder( strStructC1 );
            
            string strStructC2 = "" +
                "typedef struct {" +
                "    uint8_t  Sync1;" +
                "    uint8_t  Sync2;" +
                "    uint8_t  Class;" +
                "    uint8_t  Id;" +
                "    uint16_t  Length;" +
                "    uint32_t  iTow;" +
                "    uint16_t  year;" +
                "    uint8_t   month;" +
                "    uint8_t   day;" +
                
                "    uint16_t siemka : 4;" +
                "    uint16_t reserved : 12;" +
                "    float    *pFloat; \\ wskaznik do floatu!" +
                "    char*    pName; /* Nazwa */" +
                "    char     Name[10]; /* Nazwa2 */" +
                "} testStruct_t;";
            
            Decoder dec2 = new Decoder( strStructC2 );
            
            Form1 frm = new Form1();
//            frm.BindDecoder( ref dec1 );
            frm.AddDecoder( ref dec1 );
            frm.AddDecoder( ref dec2 );

            Application.Run(frm);
            
        }
    }
}