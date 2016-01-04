using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OpenHardwareMonitorServerService
{
    public partial class HeadlessMonitorService : ServiceBase
    {
        HeadlessSensorMonitor monitor;
        public HeadlessMonitorService()
        {
            InitializeComponent();
            monitor = new HeadlessSensorMonitor();
        }

        public void Init()
        {
            monitor.Start();
        }

        protected override void OnStart(string[] args)
        {
            this.Init();
        }

        protected override void OnStop()
        {
            monitor.Close();
        }
    }
}
