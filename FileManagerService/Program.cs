using System.ServiceProcess;

namespace FileManagerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main() {
#if DEBUG
            var fms = new FreeMaxService();
            fms.OnDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            var servicesToRun = new ServiceBase[]
                                          {
                                              new FreeMaxService()
                                          };
            ServiceBase.Run(servicesToRun);
#endif
        }
    }
}
