using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Owin.Hosting;

namespace FileManagerService {
    public partial class FreeMaxService: ServiceBase {
        private readonly object _obj = new object();
        private const string ServerUri = "http://*:13666";
        private IDisposable SignalR { get; set; }

        public FreeMaxService() {
            InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
        }

        private void StartServer()
        {
            try
            {
                SignalR = WebApp.Start(ServerUri);
            }
            catch (TargetInvocationException e)
            {
                Logger.RecordEntry("A server is already running at " + ServerUri + ' ' + e.Message + ' ' + e.InnerException);
                return;
            }
            Logger.RecordEntry("Server started at " + ServerUri);
        }

        public void OnDebug() {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            lock(_obj) {
                new Thread(StartServer).Start();
            }
        }

        protected override void OnStop()
        {
            lock(_obj) {
                SignalR.Dispose();
                Logger.RecordEntry("Server stopped at " + ServerUri);
                Thread.Sleep(1000);
            }
        }

        protected override void OnPause()
        {
            lock(_obj) {
                SignalR.Dispose();
                Logger.RecordEntry("Server stopped at " + ServerUri);
                Thread.Sleep(1000);
            }
        }

        protected override void OnContinue()
        {
            lock(_obj) {
                new Thread(StartServer).Start();
            }
        }

        protected override void OnShutdown()
        {
            lock(_obj) {
                SignalR.Dispose();
                Logger.RecordEntry("Server stopped at " + ServerUri);
                Thread.Sleep(1000);
            }
        }
    }
}
