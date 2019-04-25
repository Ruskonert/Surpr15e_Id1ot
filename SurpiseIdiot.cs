using System;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Surpr15e_Id1ot
{
    class SurpiseIdiot
    {
        public static void Main(string[] args)
        {
            RemotePrintController controller = new RemotePrintController("../test.txt").Initialize();
            if(controller.WaitForAccept().Result)
            {
                
            }
            else
            {

            }
        }
    }

    internal class RemotePrintController
    {
        public readonly static byte[] MAGIC_NUMBER = { 0x4a, 0x75, 0x6e, 0x67, 0x44, 0x61, 0x55, 0x6e };
        public readonly static int PRINT_REMOTE_PORT = 4444;
        private readonly string file = null;
        private readonly StreamReader targetOfPrintStreamer = null;

        private PrintDocument printDocument = null;

        private Font printFont = new Font("Consolas", 12);
        public Font PrintFont { get => printFont; set => printFont = value; }

        private TcpListener listener = null;
        private TcpClient client = null;
        private NetworkStream stream = null;
        public TcpListener Listener { get => listener; set => listener = value; }

        public static bool IsAvailablePort(int port = 0)
        {
            if (port == 0) port = RemotePrintController.PRINT_REMOTE_PORT;
            IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnectionInformation = iPGlobalProperties.GetActiveTcpConnections();
            foreach(TcpConnectionInformation tcpi in tcpConnectionInformation)
            {
                if(tcpi.LocalEndPoint.Port == port)
                {
                    return false;
                }
            }
            return true;
        }

        protected bool Print()
        {

            return true;
        }

        public RemotePrintController(string fileOfPath)
        {
            MemoryStream stream = null;
            this.file = fileOfPath;
            if(! File.Exists(file)) {
                Console.WriteLine($"{this.file} not exist file or directory, Replacing sample message.");
                this.file = null;
                byte[] message = { 0x49, 0x20, 0x53, 0x45, 0x45, 0x20, 0x59, 0x4F, 0x55 };
                stream = new MemoryStream(message);
            }

            if (this.file == null) this.targetOfPrintStreamer = new StreamReader(file);
            else this.targetOfPrintStreamer = new StreamReader(stream);
        }

        public RemotePrintController Initialize(int port = 0)
        {
            if (port == 0) port = RemotePrintController.PRINT_REMOTE_PORT;
            while(! RemotePrintController.IsAvailablePort(port)) {
                port += 1;
            }

            this.Listener = new TcpListener(IPAddress.Any, port);
            this.printDocument = new PrintDocument();
            return this;
        }

        public Task GetReceiveData(int sizeOf = 1024)
        {
            byte[] buffer = new byte[sizeOf];
            Task t = Task.Run(() => {
                GetBuffer(buffer);
            });
            return t;
        }

        private void GetBuffer(byte[] buff)
        {
            while ((_ = this.stream.Read(buff, 0, buff.Length)) > 0) { }
        }
        
        public async Task<bool> WaitForAccept()
        {
            var task = Task.Run(() => _Accept);
            return await task;
        }

        private bool _Accept
        {
            get
            {
                try
                {
                    this.client = this.listener.AcceptTcpClient();
                    this.stream = this.client.GetStream();
                    return true;
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                    return false;
                }
            }
        }
    }
}
