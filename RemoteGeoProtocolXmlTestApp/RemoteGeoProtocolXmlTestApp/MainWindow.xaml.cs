using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using RemoteGeoSystems;

namespace RGSGpsXmlProtocolTestClient
{
    public partial class MainWindow : Window
    {
        public const uint TIME_BETWEEN_RETRY_CONNECTS = 5000;  // In millisecs

        public bool IsRecieving { get; private set; } = false;

        private System.Windows.Threading.DispatcherTimer statusBarDisplayTimer = null;

        public delegate void RemoveStatusMessageDelegate();
        public delegate void AddStatusMessageDelegate( string _msg,int _secondsToDisplay );

        private TcpClient tcpClient = null;
        private StreamReader clientStreamReader = null;
        private StreamWriter clientStreamWriter = null;

        private DateTime timeOfLastRetryConnect = DateTime.MinValue;
        private System.Net.IPAddress gpsIpAddr = null;
        private int gpsPort = 1121;

        private System.Windows.Threading.DispatcherTimer sendTimer = null;

        public bool IsConnected {
            get {
                return tcpClient != null && tcpClient.Connected;
            }
        }

        private int BufferSize {
            get {
                return this.bufferSize;
            }
            set {
                this.bufferSize = value;
                if( this.tcpClient != null ) {
                    this.tcpClient.ReceiveBufferSize = value;
                }
            }
        }

        private int minBufferSize = 8192;
        private int maxBufferSize = 15 * 1024 * 1024;
        private int bufferSize = 8192;

        RemoteGeoUdpProtocolListener udpListener = null;

        // IMPORTANT: THIS IS HARD-CODED ALTHOUGH IT CAN BE CHANGED TO MATCH THE geoDVR SETTINGS
        private const int udpPort = 1127;

        public MainWindow()
        {
            InitializeComponent();

            // Always listen for UDP connections on port 1127 -- IMPORTANT: ONLY ONE LISTENER IS REQUIRED (EITHER TCP or UDP).  THIS APPLICATION IMPLEMENTS CONNECTIONS
            // TO BOTH geoDVR PROTOCOLS (TCP & UDP) IN ORDER TO DEMONSTRATE HOW THE CONNECTIONS ARE MADE.  HOWEVER, ONLY ONE SHOULD BE IMPLEMENTED.
            // Either can be used, although in general, the UDP connection is recommended as it can be implemented  as shown below.
            udpListener = new RemoteGeoUdpProtocolListener( udpPort,false );
            udpListener.TcpDataReceived += UdpListener_TcpDataReceived; ;
            udpListener.TcpErrorOccurred += UdpListener_TcpErrorOccurred; ;
            if( udpListener.StartListening( IPAddress.Any ) ) {
                showStatus( "UDP Listener started.  Listening for UDP Camera Control Packets on port " + udpPort );
            }

            // Timer for displaying status message for set abount of time
            try {
                statusBarDisplayTimer = new System.Windows.Threading.DispatcherTimer();
                statusBarDisplayTimer.Tick += statusBarDisplayTimer_Tick;
                statusBarDisplayTimer.Interval = TimeSpan.FromSeconds( 20 );
                statusBarDisplayTimer.Stop();
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }
        }

        private void UdpListener_TcpErrorOccurred( object sender,string _error,Exception _ex )
        {
            showError( _error );
        }

        private void UdpListener_TcpDataReceived( object sender,string _rawData,XElement xmlData )
        {
            try {
                showStatus( "Received UDP Message.  Please see the status bar for the coordinates." );
                displayUdpCameraControlXml( _rawData );
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }
        }

        public void AddStatusMessage( string _msg,int _secondsToDisplay = 10 )
        {
            try {
                if( Dispatcher.Thread != System.Threading.Thread.CurrentThread ) {
                    AddStatusMessageDelegate swd = new AddStatusMessageDelegate( AddStatusMessage );
                    Dispatcher.BeginInvoke( swd,new object[] { _msg,_secondsToDisplay } );
                } else {
                    statusText.Text = _msg;
                    statusBarDisplayTimer.Interval = TimeSpan.FromSeconds( _secondsToDisplay );
                    statusBarDisplayTimer.Start();
                }
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }
        }

        private void statusBarDisplayTimer_Tick( object sender,EventArgs e )
        {
            try {
                statusBarDisplayTimer.Stop();
                RemoveStatusMessage();
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }
        }

        public void RemoveStatusMessage()
        {
            try {
                if( Dispatcher.Thread != System.Threading.Thread.CurrentThread ) {
                    RemoveStatusMessageDelegate rsd = new RemoveStatusMessageDelegate( RemoveStatusMessage );
                    Dispatcher.BeginInvoke( rsd );
                } else {
                    statusText.Text = string.Empty;
                }
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }
        }

        private void aboutMenuItem_Click( object sender,RoutedEventArgs e )
        {
            try {
                MessageBox.Show( this,"Copyright 2021 Remote GeoSystems, Inc.  All Rights Reserved.","Camera Protocol Test App" );
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }
        }

        private void exitMenuItem_Click( object sender,RoutedEventArgs e )
        {
            Application.Current.Shutdown();
        }

        private void closeGpsStreams()
        {
            try {
                if( sendTimer != null ) {
                    sendTimer.Stop();
                }
            } finally {
                sendTimer = null;
            }

            try {
                if( tcpClient != null ) {
                    // Not all Closes() needed but be safe...
                    try {
                        clientStreamReader.Close();
                    } catch { }
                    try {
                        clientStreamWriter.Close();
                    } catch { }
                    try {
                        tcpClient.GetStream().Close();
                    } catch { }
                        
                    tcpClient.Close();
                }
            } finally {
                clientStreamReader = null;
                clientStreamWriter = null;
                tcpClient = null;
            }

            try {
                gpsDataBox.Items.Clear();
                cameraCoordsDataBox.Items.Clear();
                previewStackPanel.Visibility = Visibility.Collapsed;
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }

            gpsIpAddr = null;
            gpsPort = 1121;
        }

        private void stopStreamingButton_Click( object sender,RoutedEventArgs e )
        {
            closeGpsStreams();
            AddStatusMessage( "Streaming Stopped...",5 );
        }

        private void MainWindowBase_Closing( object sender,System.ComponentModel.CancelEventArgs e )
        {
            closeGpsStreams();

            try {
                if( udpListener != null ) {
                    udpListener.StopListening();
                    udpListener = null;
                }
            } catch { }
        }

        private void startStreamingButton_Click( object sender,RoutedEventArgs e )
        {
            if( startStreaming() ) {
                AddStatusMessage( "Streaming Started...",5 );
            } else {
                AddStatusMessage( "Unable to start streaming.  Please check the error long..",5 );
            }
        }

        private bool startStreaming()
        {
            try {
                clearErrorBox();
                closeGpsStreams();
                previewStackPanel.Visibility = Visibility.Visible;

                // Make sure we have an IP Address and Port
                if( string.IsNullOrWhiteSpace( gpsIpAddressBox.Text ) ) {
                    throw new Exception( "Please enter an ip address." );
                }

                gpsIpAddr = null;
                if( !System.Net.IPAddress.TryParse( gpsIpAddressBox.Text,out gpsIpAddr ) ) {
                    throw new Exception( "An invalid ip address was detected.  Please check your value and try again." );
                }

                gpsPort = int.Parse( gpsPortBox.Text );
                if( gpsPort < 1 || gpsPort >= 65534 ) {
                    throw new Exception( "An invalid GPS streaming port was detected.  Please check your value and try again." );
                }

                bool res = ConnectToGpsStream();
                if( res ) {
                    sendTimer = new System.Windows.Threading.DispatcherTimer();
                    sendTimer.Tick += SendTimer_Tick;
                    sendTimer.Interval = TimeSpan.FromSeconds( 1 );
                    sendTimer.Start();
                }
                return res;
            } catch( Exception ex ) {
                showError( ex?.ToString() );
                return false;
            }
        }

        public bool SendXml( XElement _xml )
        {
            try {
                if( !IsConnected || _xml == null ) {
                    return false;
                }

                if( clientStreamWriter == null ) {
                    TimeSpan timeSinceRetryConnect = DateTime.Now - timeOfLastRetryConnect;
                    if( timeSinceRetryConnect > TimeSpan.FromMilliseconds( TIME_BETWEEN_RETRY_CONNECTS ) ) {
                        timeOfLastRetryConnect = DateTime.Now;
                        if( !ConnectToGpsStream() ) {
                            throw new Exception( "A connection could not be established to send GPS live stream coordinates." );
                        }
                    } else {
                        return false;
                    }
                }

                if( _xml != null && _xml.HasElements && tcpClient.Connected ) {
                    SendAsync( _xml.ToString() );
                    return true;
                }
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }

            return false;
        }

        public async Task SendAsync( string data )
        {
            try {
                await clientStreamWriter.WriteAsync( data );
                await clientStreamWriter.FlushAsync();
                displaySentGps( data );
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }
        }

        public bool ConnectToGpsStream()
        {
            try {
                tcpClient = new TcpClient( gpsIpAddr.ToString(),gpsPort );
                NetworkStream clientSockStream = tcpClient.GetStream();
                if( clientSockStream != null ) {
                    clientStreamReader = new StreamReader( clientSockStream );
                    clientStreamWriter = new StreamWriter( clientSockStream );

                    Receive();
                    return clientStreamWriter != null;
                }
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }

            return false;
        }

        public async Task Receive( CancellationToken token = default( CancellationToken ) )
        {
            try {
                if( !this.IsConnected || this.IsRecieving ) {
                    throw new Exception( "A connection was not detected.  Could not start the TCP listener." );
                }
                IsRecieving = true;
                char[] buffer = new char[bufferSize];
                while( IsConnected ) {
                    token.ThrowIfCancellationRequested();
                    int bytesRead = await this.clientStreamReader.ReadAsync( buffer,0,buffer.Length );
                    if( bytesRead > 0 ) {
                        if( bytesRead == buffer.Length ) {
                            this.BufferSize = Math.Min( this.BufferSize * 10,this.maxBufferSize );
                        } else {
                            do {
                                int reducedBufferSize = Math.Max( this.BufferSize / 10,this.minBufferSize );
                                if( bytesRead < reducedBufferSize )
                                    this.BufferSize = reducedBufferSize;

                            }
                            while( bytesRead > this.minBufferSize );
                        }

                        char[] data = new char[bytesRead];
                        Array.Copy( buffer,data,bytesRead );
                        if( data != null && bytesRead > 0 ) {
                            string recData = new string( data );
                            onDataReceived( recData );
                        }
                    }
                    buffer = new char[bufferSize];
                }
            } catch( Exception ex ) {
                showError( "The TCP Client Receive() method has exited.  TCP Data will no longer be received until a connection is restablished.",true );
            } finally {
                this.IsRecieving = false;
            }
        }

        private void SendTimer_Tick( object sender,EventArgs e )
        {
            try {
                if( IsConnected ) {
                    XElement toSend = NextTestGpsPoint;
                    SendXml( toSend );
                }
            } catch( Exception ex ) {
                showError( ex?.ToString() );
            }
        }

        private void showStatus( string msg )
        {
            try {
                showError( msg,true );
            } catch { }
        }

        delegate void showErrorCallback( string msg,bool isStatusUpdate );
        private void showError( string msg,bool isStatusUpdate=false )
        {
            if( Dispatcher.Thread != System.Threading.Thread.CurrentThread ) {
                showErrorCallback d = new showErrorCallback( showError );
                Dispatcher.BeginInvoke( d,System.Windows.Threading.DispatcherPriority.Background,new object[] { msg,isStatusUpdate } );
            } else {
                try {
                    if( errorBox.HasItems && errorBox.Items.Count > 20 ) {
                        errorBox.Items.Clear();
                    }

                    errorBox.Items.Insert( 0,msg );
                    if( !isStatusUpdate ) {
                        errorBox.Items.Insert( 0,"Error at " + DateTime.Now.ToString( "G" ) + ":" );
                    }
                } catch( Exception ex ) {
                }
            }
        }

        private void clearErrorBox()
        {
            Application.Current.Dispatcher.Invoke( () => {
                try {
                    errorBox.Items.Clear();
                } catch {
                }
            } );
        }

        delegate void displaySentGpsCallback( string msg );
        private void displaySentGps( string msg )
        {
            if( Dispatcher.Thread != System.Threading.Thread.CurrentThread ) {
                displaySentGpsCallback d = new displaySentGpsCallback( displaySentGps );
                Dispatcher.BeginInvoke( d,System.Windows.Threading.DispatcherPriority.Background,new object[] { msg } );
            } else {
                try {
                    if( gpsDataBox.HasItems && gpsDataBox.Items.Count > 10 ) {
                        gpsDataBox.Items.Clear();
                    }

                    gpsDataBox.Items.Insert( 0,"" );
                    gpsDataBox.Items.Insert( 0,msg );
                } catch( Exception ex ) {
                    showError( ex?.ToString() );
                }
            }
        }

        delegate void onDataReceivedCallback( string data );
        private void onDataReceived( string data )
        {
            if( Dispatcher.Thread != System.Threading.Thread.CurrentThread ) {
                onDataReceivedCallback d = new onDataReceivedCallback( onDataReceived );
                Dispatcher.BeginInvoke( d,System.Windows.Threading.DispatcherPriority.Background,new object[] { data } );
            } else {
                try {
                    if( cameraCoordsDataBox.HasItems && cameraCoordsDataBox.Items.Count > 10 ) {
                        cameraCoordsDataBox.Items.Clear();
                    }

                    cameraCoordsDataBox.Items.Insert( 0,"" );
                    cameraCoordsDataBox.Items.Insert( 0,data );
                    cameraCoordsDataBox.Items.Insert( 0,"TCP Data Received at " + DateTime.Now.ToString( "G" ) + ":" );
                } catch( Exception ex ) {
                    showError( ex?.ToString() );
                }
            }
        }

        private void displayUdpCameraControlXml( string msg )
        {
            Application.Current.Dispatcher.Invoke( () => {
                try {
                    XElement xe = XElement.Parse( msg );
                    string statusMsg = "Received UDP Message at : " + DateTime.Now.ToString( "G" ) + " Lat: " + xe.Element( "lat" ).Value + ", Long: " + xe.Element( "lon" ).Value;
                    AddStatusMessage( statusMsg );
                } catch( Exception ex ) {
                    showError( ex?.ToString() );
                }
            } );
        }

        



        // ************************************************************************************************************************** //
        // The following is for simulating the sending of GPS coordinates to the geoDVR.  It should be replaced by actual GPS points.
        // ************************************************************************************************************************** //
        private static int testXmlIndex = 0;
        private static XElement NextTestGpsPoint {
            get {
                try {
                    if( testXmlList == null || testXmlList.Count == 0 ) {
                        return new XElement( "error" );
                    }

                    XElement xe = testXmlList[testXmlIndex];
                    testXmlIndex++;

                    if( testXmlIndex >= testXmlList.Count ) {
                        testXmlIndex = 0;
                    }

                    return xe;
                } catch {
                    return new XElement( "error" );
                }
            }
        }

        private static XElement testXml = XElement.Parse( testDataToSend );
        private static List<XElement> testXmlList = testXml.Elements( "trkpt" ).ToList();
        private const string testDataToSend = @"<remotegeo>
          <trkpt>
            <lat>41.1438456602133</lat>
            <lon>-104.805596752467</lon>
            <ele>2936.37140459297</ele>
            <time>2012-09-19T20:40:44.105Z</time>
            <speed>64.6623300951608</speed>
            <course>201.27260242618451</course>
            <frame_lat>41.138308365428031</frame_lat>
            <frame_lon>-104.7855039056323</frame_lon>
            <frame_ele>1855.3612573434038</frame_ele>
            <horizontal_fov>18.652323186083773</horizontal_fov>
          </trkpt>
          <trkpt>
            <lat>41.1437269305548</lat>
            <lon>-104.805659281465</lon>
            <ele>2936.37140459297</ele>
            <time>2012-09-19T20:40:44.325Z</time>
            <speed>64.4861687913343</speed>
            <course>200.93202105745024</course>
            <frame_lat>41.138307652966262</frame_lat>
            <frame_lon>-104.78550449236552</frame_lon>
            <frame_ele>1855.3612573434038</frame_ele>
            <horizontal_fov>18.652323186083773</horizontal_fov>
          </trkpt>
          <trkpt>
            <lat>41.1436084104439</lat>
            <lon>-104.805721559006</lon>
            <ele>2936.37140459297</ele>
            <time>2012-09-19T20:40:44.545Z</time>
            <speed>55.2741646173142</speed>
            <course>200.44312199588009</course>
            <frame_lat>41.138307401509167</frame_lat>
            <frame_lon>-104.78550424090842</frame_lon>
            <frame_ele>1855.3612573434038</frame_ele>
            <horizontal_fov>18.652323186083773</horizontal_fov>
          </trkpt>
          <trkpt>
            <lat>41.1435067379584</lat>
            <lon>-104.805774616453</lon>
            <ele>2936.37140459297</ele>
            <time>2012-09-19T20:40:44.765Z</time>
            <speed>60.8007782623533</speed>
            <course>199.97070267795837</course>
            <frame_lat>41.138307191961587</frame_lat>
            <frame_lon>-104.78550474382261</frame_lon>
            <frame_ele>1855.3612573434038</frame_ele>
            <horizontal_fov>18.652323186083773</horizontal_fov>
          </trkpt>
          <trkpt>
            <lat>41.1434049397444</lat>
            <lon>-104.805827254805</lon>
            <ele>2936.37140459297</ele>
            <time>2012-09-19T20:40:44.965Z</time>
            <speed>64.4286626609925</speed>
            <course>199.71801327534908</course>
            <frame_lat>41.138306814775945</frame_lat>
            <frame_lon>-104.78550499527971</frame_lon>
            <frame_ele>1855.3612573434038</frame_ele>
            <horizontal_fov>18.652323186083773</horizontal_fov>
          </trkpt>
          <trkpt>
            <lat>41.1432859586288</lat>
            <lon>-104.805887520689</lon>
            <ele>2936.37140459297</ele>
            <time>2012-09-19T20:40:45.184Z</time>
            <speed>60.2665028381826</speed>
            <course>199.2291142137789</course>
            <frame_lat>41.138299899705828</frame_lat>
            <frame_lon>-104.78550650402228</frame_lon>
            <frame_ele>1855.3612573434038</frame_ele>
            <horizontal_fov>18.652323186083773</horizontal_fov>
          </trkpt>
          <trkpt>
            <lat>41.1431840346862</lat>
            <lon>-104.805938901755</lon>
            <ele>2936.37140459297</ele>
            <time>2012-09-19T20:40:45.386Z</time>
            <speed>60.7133398661088</speed>
            <course>199.0258640421149</course>
            <frame_lat>41.138252583862396</frame_lat>
            <frame_lon>-104.785526872047</frame_lon>
            <frame_ele>1855.3612573434038</frame_ele>
            <horizontal_fov>18.652323186083773</horizontal_fov>
          </trkpt>
          <trkpt>
            <lat>41.143081985015</lat>
            <lon>-104.80598952845</lon>
            <ele>2936.37140459297</ele>
            <time>2012-09-19T20:40:45.586Z</time>
            <timecode>1.4810</timecode>
            <course>198.72922865644313</course>
            <frame_lat>41.138119185873364</frame_lat>
            <frame_lon>-104.78557598999961</frame_lon>
            <frame_ele>1855.3612573434038</frame_ele>
            <horizontal_fov>18.657816433966584</horizontal_fov>
          </trkpt>
          <trkpt>
            <lat>41.1429798096153</lat>
            <lon>-104.806039316955</lon>
            <ele>2936.37140459297</ele>
            <time>2012-09-19T20:40:45.793Z</time>
            <timecode>1.6890</timecode>
            <course>198.72922865644313</course>
            <frame_lat>41.137895682425182</frame_lat>
            <frame_lon>-104.78565830028879</frame_lon>
            <frame_ele>1855.6649118791483</frame_ele>
            <horizontal_fov>18.644083314259557</horizontal_fov>
          </trkpt>
          <trkpt>
            <lat>41.1428775503965</lat>
            <lon>-104.806088602546</lon>
            <ele>2936.67505912871</ele>
            <time>2012-09-19T20:40:45.993Z</time>
            <speed>57.1539987503009</speed>
            <timecode>1.8890</timecode>
            <course>198.5094987411307</course>
            <frame_lat>41.137600262247759</frame_lat>
            <frame_lon>-104.7857710368865</frame_lon>
            <frame_ele>1855.6649118791483</frame_ele>
            <sensor_relative_azimuth>268.00000009778887</sensor_relative_azimuth>
            <sensor_relative_pitch>-21.726562493353413</sensor_relative_pitch>
            <sensor_relative_roll>0</sensor_relative_roll>
            <slant_range>2103.3978560248852</slant_range>
            <horizontal_fov>18.657816433966584</horizontal_fov>
          </trkpt>
        </remotegeo>";
    }
}
