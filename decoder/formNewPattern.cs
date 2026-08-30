/*
 * Created by SharpDevelop.
 * User: AdamS-0
 * Date: 30.08.2026
 * Time: 17:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace decoder {
    /// <summary>
    /// Description of formNewPattern.
    /// </summary>
    public partial class formNewPattern : Form {
        
        public delegate void NewPatternEvent(string newPattern, string patternName);
        public NewPatternEvent NewPattern;
        
        public formNewPattern() {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        
        void BtnCancleClick ( object sender, EventArgs e ) {
            this.Close();
        }
        
        void BtnProceedClick ( object sender, EventArgs e ) {
            if ( NewPattern != null ) {
                NewPattern( rtbNewPattern.Text, txtBxPatternName.Text );
            }
        }
    }
}
