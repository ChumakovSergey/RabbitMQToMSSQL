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
            serviceInstaller.ServiceName = "RabbitMQToMSSQL";
            serviceInstaller.DisplayName = "MSSQL Server RabbitMQ listener";
            serviceInstaller.Description = "Перенаправляет сообщения из RabbitMQ в MSSQL Server";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}