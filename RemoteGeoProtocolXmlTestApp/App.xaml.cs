using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RGSGpsXmlProtocolTestClient
{
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            try {
                // App run
                RGSGpsXmlProtocolTestClient.App app = new RGSGpsXmlProtocolTestClient.App();
                app.Run();
            } catch( Exception ex ) {
                try {
                    MessageBox.Show( "A critical error occurred.  The application has crashed.  The detailed error message is:\n\n" + ex.Message,"Critical Error" );
                } catch { }
            }
        }

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );
            try {
                MainWindow mw = new MainWindow();
                mw.ShowDialog();
            } catch( Exception ex ) {
                MessageBox.Show( "A critical error occurred.  The application has crashed.  The detailed error message is:\n\n" + ex.Message,"Critical Error" );
            }

            this.Shutdown();
        }
    }
}
