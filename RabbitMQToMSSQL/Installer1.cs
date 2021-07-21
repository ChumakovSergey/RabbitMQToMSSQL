using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;
 
namespace RabbitMQToMSSQL
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public Installer1()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.ServiceName = Properties.Settings.Default.ServiceName;
            serviceInstaller.DisplayName = Properties.Settings.Default.ServiceName;
            serviceInstaller.Description = Properties.Settings.Default.ServiceDescription;
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}