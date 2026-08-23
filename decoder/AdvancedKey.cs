/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 15.11.2020
 * Time: 18:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace decoder {
    
    /// <summary>
    /// Description of AdvancedKey.
    /// </summary>
    public class AdvancedKey {
        
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        
        public bool KeyDown = false;
        public bool KeyActiveLast = false;
        
        public bool KeyDownFirst = false;
        public bool KeyDownLast = false;
        
        public Keys[] Key;
        
        public AdvancedKey( Keys[] key ) { Key = key; }
        public AdvancedKey( Keys key ) { Key = new Keys[] { key }; }
        
        public void SetNewKeys( Keys[] key ) { Key = key; }
        public void SetNewKey( Keys key ) { Key = new Keys[] { key }; }
        
        public bool TryKey(){
            KeyDownFirst = false;
            KeyDownLast = false;
            
            if( KeyDown != KeyActiveLast ){
                
                if( KeyDown )     KeyDownFirst = true;
                else             KeyDownLast = true;
                
                KeyActiveLast = KeyDown;
            }
            
            bool keyPressed = true;
            
            foreach( Keys key in Key ) if( GetAsyncKeyState( key ) == 0 ) keyPressed = false;
            
            
            if( keyPressed ) { if( !KeyDown ) { KeyDown = true; } }
            else { KeyDown = false; }
            
            return KeyDown;
        }
        
    }
}