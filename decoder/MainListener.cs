/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 22.08.2026
 * Time: 18:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace decoder {
    /// <summary>
    /// Description of MainListener.
    /// </summary>
    public partial class MainListener : System.Windows.Forms.Form {
        
        private NotifyIcon _notifyIcon;
        private ContextMenu _notificationMenu;
        
        AdvancedKey Key_Activate = new AdvancedKey( new Keys[] { Keys.LShiftKey , Keys.RButton } );
        System.Windows.Forms.Timer timerKeys = new System.Windows.Forms.Timer();
        
        ContextMenuStrip _leftMenu = new ContextMenuStrip();
        
        static List<DecoderGui> _decGui = new List<DecoderGui>();
        static DecoderGui _curDecGui = null;
        
        public MainListener() {
            InitializeComponent();
            
            _notifyIcon = new NotifyIcon();
            _notificationMenu = new ContextMenu(InitializeMenu());
            
            _notifyIcon.DoubleClick += IconDoubleClick;
            
            _notifyIcon.Icon = global::decoder.Resource1.icon1;
            _notifyIcon.ContextMenu = _notificationMenu;
            _notifyIcon.Visible = true;
            
            _leftMenu.Items.Add( "New decoder", null, leftMenu_newDecoder );
            _leftMenu.Items.Add( "Get pattern", null, leftMenu_getPattern );
            _leftMenu.Items.Add( "Get data", null, leftMenu_getData );
            
            timerKeys.Interval = 50;
            timerKeys.Tick += timerKeys_TestForKeys;
            timerKeys.Start();
        }
        
        private MenuItem[] InitializeMenu() {
            MenuItem[] menu = new MenuItem[] {
                new MenuItem("New decoder", menuNewDecoder),
                new MenuItem("About", menuAboutClick),
                new MenuItem("Exit", menuExitClick)
            };
            return menu;
        }
        
        void MainListenerLoad ( object sender, EventArgs e ) {
            this.Hide();
            this.Visible = false;
        }
        
        void timerKeys_TestForKeys(object sender, EventArgs e) {
            
            Key_Activate.TryKey();
            if( Key_Activate.KeyDownFirst ) {
                /* Show leftContextMenu */
                _leftMenu.Show( Cursor.Position.X - _leftMenu.Width, Cursor.Position.Y );
                _leftMenu.BringToFront();
            }
        }
        
        private void menuAboutClick ( object sender, EventArgs e ) {
            MessageBox.Show( "Application to decode raw hexdecimal byte array by given patterns, based on C struct templates.\r\nDecoder byAdamS-0 2026" );
        }
        
        private void menuExitClick ( object sender, EventArgs e ) {
            Application.Exit();
        }
        
        private void menuNewDecoder( object sender, EventArgs e ) {
            AddNewGui().Show();
        }
        
        
        private void IconDoubleClick ( object sender, EventArgs e ) {
            MessageBox.Show( "The icon was double clicked" );
        }
        
        
        public void AddDecoder(ref dObject decNew) {
            DecoderGui decGui = GetCurrGui();
            decGui.AddDecoder( ref decNew );
        }
        
        private static DecoderGui AddNewGui() {
            DecoderGui newDecGui = new DecoderGui();
            newDecGui.Activated += delegate (object sender, EventArgs e) { _curDecGui = (DecoderGui)sender; };
            _decGui.Add( newDecGui );
            _curDecGui = newDecGui;
            return newDecGui;
        }
        
        public static DecoderGui GetCurrGui() {
            if ( _curDecGui == null ) {
                DecoderGui newGui = AddNewGui();
                newGui.Show();
                return newGui;
            }
            
            foreach ( DecoderGui gui in _decGui ) {
                if ( gui.ContainsFocus ) {
                    return gui;
                }
            }
            
            return _curDecGui;
        }
        
        private void leftMenu_newDecoder ( object sender, EventArgs e ) {
            AddNewGui().Show();
        }
        
        private void leftMenu_getPattern ( object sender, EventArgs e ) {
            _leftMenu.Hide();
            string strPattern = GetSelectedText();
            _leftMenu.Close();
            
            DecoderGui gui = GetCurrGui();
            gui.TryParseNewPattern( strPattern );
        }
        
        private void leftMenu_getData ( object sender, EventArgs e ) {
            _leftMenu.Hide();
            string strData = GetSelectedText();
            _leftMenu.Close();
            DecoderGui gui = GetCurrGui();
            gui.SetRawDataAndTryParse( strData );
        }
        
        private string GetSelectedText() {
            Thread.Sleep(100);
            SendKeys.Send("^c");
            return Clipboard.GetText();
        }
    }
}

