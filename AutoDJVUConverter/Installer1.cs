using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace AutoDJVUConverter
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        //private EventLogInstaller eventLogInstaller;
        private ServiceInstaller serviceInstaller;
        private ServiceProcessInstaller processInstaller;

        public Installer1()
        {
            InitializeComponent();
            //eventLogInstaller = new EventLogInstaller() { Source = "AutoDJVUConverter", Log = "AutoDJVUConverter" };
            serviceInstaller = new ServiceInstaller()
            {
                StartType = ServiceStartMode.Automatic,
                ServiceName = "DJVUConverter",
                Description = "Converts a PDF-FullText to djvu",
            };
            processInstaller = new ServiceProcessInstaller()
            {
                Account = ServiceAccount.LocalSystem
            };
            //Installers.Add(eventLogInstaller);
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
