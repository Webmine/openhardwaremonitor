using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OpenHardwareMonitorServerService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!Environment.UserInteractive)
            {
                ServiceBase[] servicesToRun;

                // More than one NT Service may run within the same process. To add
                // another service to this process, change the following line to
                // create a second service object. For example,
                //
                //   ServicesToRun = New System.ServiceProcess.ServiceBase () {New ServiceMain, New MySecondUserService}
                //
                servicesToRun = new ServiceBase[] { new HeadlessMonitorService() };

                ServiceBase.Run(servicesToRun);
            }
            else
            {
                var service = new HeadlessMonitorService();
                service.Init();
            }
        }
    }
}
