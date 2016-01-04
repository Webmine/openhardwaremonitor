using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHardwareMonitorServerService.Utilities
{
    public interface IResourceProvider
    {
        byte[] GetResource(string name);
    }
}
