using System.ServiceProcess;
using System.Threading;

namespace FileManagerService {
    public partial class FreeMaxService: ServiceBase {
        public FreeMaxService() {
            InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
        }

        protected override void OnStart(string[] args) {
            //logger = new Logger();
            //Thread loggerThread = new Thread(new ThreadStart(/*someFunc*/));
            //loggerThread.Start();
        }

        protected override void OnStop() {
            //logger.Stop();
            //Thread.Sleep(1000);
        }

        protected override void OnPause() {
            //do smth
        }

        protected override void OnContinue() {
            //do smth     
        }

        protected override void OnShutdown() {
            //do smth
        }
    }
}
