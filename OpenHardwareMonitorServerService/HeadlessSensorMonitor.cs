using OpenHardwareMonitor;
using OpenHardwareMonitor.GUI;
using OpenHardwareMonitor.Hardware;
using OpenHardwareMonitor.Utilities;
using OpenHardwareMonitor.WMI;
using OpenHardwareMonitorServerService.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OpenHardwareMonitorServerService
{
    public class HeadlessSensorMonitor
    {
        Node root;
        PersistentSettings settings;
        UnitManager unitManager;
        Computer computer;
        UpdateVisitor updateVisitor;
        Timer timer;
        WmiProvider wmiProvider;
        HttpServer server;
        IResourceProvider resourceProvider;

        public HeadlessSensorMonitor()
        {
            settings = new PersistentSettings();
            this.settings.Load(Path.ChangeExtension(Assembly.GetEntryAssembly().Location, ".config"));
            unitManager = new UnitManager(settings);

            resourceProvider = new AssemblyEmbeddedRescourceProvider("OpenHardwareMonitorServerService.Resources");

            root = new Node(System.Environment.MachineName);
            root.Image = resourceProvider.GetResource("computer.png");

            updateVisitor = new UpdateVisitor();
            computer = new Computer() { CPUEnabled = true, FanControllerEnabled = true, GPUEnabled = true, HDDEnabled = true, MainboardEnabled = true, RAMEnabled = true };
            wmiProvider = new WmiProvider(computer);
            server = new HttpServer(root, 8085, resourceProvider);

            timer = new Timer(1000);
            timer.Elapsed += (s, a) => timer_Tick();

            computer.HardwareAdded += new HardwareEventHandler(HardwareAdded);
            computer.HardwareRemoved += new HardwareEventHandler(HardwareRemoved);
        }

        public void Start()
        {
            computer.Open();

            server.StartHTTPListener();

            timer.Start();
        }

        public void Close()
        {
            timer.Close();

            wmiProvider.Dispose();

            computer.Close();

            server.StopHTTPListener();
        }

        private void timer_Tick()
        {
            computer.Accept(updateVisitor);

            if (wmiProvider != null)
                wmiProvider.Update();
        }

        private void InsertSorted(Collection<Node> nodes, HardwareNode node)
        {
            int i = 0;
            while (i < nodes.Count && nodes[i] is HardwareNode &&
              ((HardwareNode)nodes[i]).Hardware.HardwareType <
                node.Hardware.HardwareType)
                i++;
            nodes.Insert(i, node);
        }

        private void SubHardwareAdded(IHardware hardware, Node node)
        {
            HardwareNode hardwareNode =
              new HardwareNode(hardware, settings, unitManager, resourceProvider);

            InsertSorted(node.Nodes, hardwareNode);

            foreach (IHardware subHardware in hardware.SubHardware)
                SubHardwareAdded(subHardware, hardwareNode);
        }


        private void HardwareAdded(IHardware hardware)
        {
            SubHardwareAdded(hardware, root);
        }

        private void HardwareRemoved(IHardware hardware)
        {
            List<HardwareNode> nodesToRemove = new List<HardwareNode>();
            foreach (Node node in root.Nodes)
            {
                HardwareNode hardwareNode = node as HardwareNode;
                if (hardwareNode != null && hardwareNode.Hardware == hardware)
                    nodesToRemove.Add(hardwareNode);
            }
            foreach (HardwareNode hardwareNode in nodesToRemove)
            {
                root.Nodes.Remove(hardwareNode);
            }
        }
    }
}
