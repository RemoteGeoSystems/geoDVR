using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteGeoSystems
{
    public class RemoteGeoUdpProtocolListener
    {
        public int Port { get; set; } = 1127;
        public volatile bool isRunning = true;

        public const string PROTOCOL_START_ELEMENT = "<GPStouch>";
        public const string PROTOCOL_END_ELEMENT = "</GPStouch>";

        private TcpListener server = null;
        private AutoResetEvent connectionWaitHandle = new AutoResetEvent( false );

        public delegate void TcpDataReceivedDelegate( object sender,string _rawData,XElement xmlData );
        public event TcpDataReceivedDelegate TcpDataReceived;

        public delegate void TcpErrorOccurredDelegate( object sender,string _error,Exception _ex );
        public event TcpErrorOccurredDelegate TcpErrorOccurred;

        public delegate void ConnectionClosedDelegate( object sender,string _msg );
        public event ConnectionClosedDelegate ConnectionClosed;

        public volatile static uint ConnectionCount = 0;

        private Thread mainRunThread = null;

        private static List<TcpClient> tcpClientsList = new List<TcpClient>();

        private bool RunOnMainThread = false;

        private int BufferSize
        {
            get
            {
                return bufferSize;
            }
            set
            {
                this.bufferSize = value;
            }
        }
        private int minBufferSize = 8192;
        private int maxBufferSize = 15 * 1024 * 1024;
        private int bufferSize = 8192;

        public RemoteGeoUdpProtocolListener( int _port = 1127,bool _onlyRunOnMainThread = false )
        {
            Port = _port;
            RunOnMainThread = _onlyRunOnMainThread;
        }

        public bool StartListening( IPAddress ipAddr = null )
        {
            try {
                if( ipAddr == null ) {
                    ipAddr = IPAddress.Any;
                }
                server = new TcpListener( ipAddr,Port );
                server.Start();

                if( !RunOnMainThread ) {
                    mainRunThread = new Thread( runThread );
                    mainRunThread.Start();
                } else {
                    runThread();
                }

                return true;
            } catch( Exception ex ) {
                Console.WriteLine( ex.Message );
                return false;
            }
        }

        private void runThread()
        {
            try {
                while( isRunning ) {
                    IAsyncResult result = server.BeginAcceptTcpClient( HandleAsyncConnection,server );
                    connectionWaitHandle.WaitOne();
                    connectionWaitHandle.Reset();
                }
                server.Stop();
            } catch( Exception ex ) {
                Console.WriteLine( ex.Message );
                try {
                    if( TcpErrorOccurred != null ) {
                        TcpErrorOccurred( this,"An error occurred in runThread().",ex );
                    }
                } catch( Exception _e ) {
                    Console.WriteLine( _e.Message );
                }
            }
        }

        public void StopListening()
        {
            try {
                isRunning = false;
                server.Stop();

                if( mainRunThread != null ) {
                    try { mainRunThread.Abort(); } catch { }
                    mainRunThread = null;
                }
            } catch( Exception ex ) {
                Console.WriteLine( ex.Message );
                try {
                    if( TcpErrorOccurred != null ) {
                        TcpErrorOccurred( this,"An error occurred in StopListening().",ex );
                    }
                } catch( Exception _e ) {
                    Console.WriteLine( _e.Message );
                }
            }
        }

        private async void HandleAsyncConnection( IAsyncResult result )
        {
            TcpClient client = null;
            uint thisConnectionId = ConnectionCount;
            try {
                ConnectionCount++;

                TcpListener listener = (TcpListener)result.AsyncState;
                client = listener.EndAcceptTcpClient( result );
                tcpClientsList.Add( client );
                connectionWaitHandle.Set(); // Inform the main thread this connection is now handled

                StreamWriter tcpWriter = new StreamWriter( client.GetStream() );
                StreamReader sr = new StreamReader( tcpWriter.BaseStream );
                tcpWriter.Flush();

                try {
                    char[] buffer = new char[BufferSize];
                    while( isRunning && client.Connected ) {
                        int bytesRead = await sr.ReadAsync( buffer,0,buffer.Length );
                        if( bytesRead > 0 ) {
                            if( bytesRead == buffer.Length ) {
                                this.BufferSize = Math.Min( this.BufferSize * 10,this.maxBufferSize );
                            } else {
                                do {
                                    int reducedBufferSize = Math.Max( this.BufferSize / 10,this.minBufferSize );
                                    if( bytesRead < reducedBufferSize )
                                        this.BufferSize = reducedBufferSize;

                                } while( bytesRead > this.minBufferSize );
                            }
                            if( TcpDataReceived != null ) {
                                char[] data = new char[bytesRead];
                                Array.Copy( buffer,data,bytesRead );
                                if( data != null && bytesRead > 0 ) {
                                    string xml = new string( data );
                                    try {
                                        if( TcpDataReceived != null ) {
                                            TcpDataReceived( this,xml,XElement.Parse( xml ) );
                                        }
                                    } catch( Exception _e ) {
                                        Console.WriteLine( _e.Message );
                                    }
                                }
                            }
                        }
                        buffer = new char[bufferSize];
                    }
                } catch( Exception e1 ) {
                    Console.WriteLine( e1.Message );
                } finally {
                    try {
                        tcpWriter.Close();
                        tcpWriter = null;
                    } catch { }
                }
            } catch( Exception ex ) {
                Console.WriteLine( ex.Message );
                try {
                    if( TcpErrorOccurred != null ) {
                        TcpErrorOccurred( this,"An error occurred in HandleAsyncConnection().",ex );
                    }
                } catch( Exception _e ) {
                    Console.WriteLine( _e.Message );
                }
            } finally {
                try {
                    if( client != null ) {
                        client.Close();
                    }
                } catch { }
                try {
                    tcpClientsList.Remove( client );
                } catch { }
                try {
                    if( ConnectionClosed != null ) {
                        ConnectionClosed( this,"Connection " + thisConnectionId.ToString() + " has closed." );
                    }
                } catch( Exception e2 ) {
                    Console.WriteLine( e2.Message );
                }
            }
        }
    }
}
