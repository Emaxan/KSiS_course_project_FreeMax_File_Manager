using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace FileManagerService {
    [RunInstaller(true)]
    public partial class FileManagerServiceInstaller: Installer {
        public FileManagerServiceInstaller() {
            InitializeComponent();

            var processInstaller = new ServiceProcessInstaller {Account = ServiceAccount.LocalSystem};
            var serviceInstaller = new ServiceInstaller {
                                                            StartType = ServiceStartMode.Automatic,
                                                            ServiceName = "FreeMaxService"
            };

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
