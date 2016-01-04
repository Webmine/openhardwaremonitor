/*
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at http://mozilla.org/MPL/2.0/.
 
  Copyright (C) 2010-2012 Michael Möller <mmoeller@openhardwaremonitor.org>
	
*/

using System;
using System.Collections.Generic;
using OpenHardwareMonitor.Hardware;
using OpenHardwareMonitorServerService.Utilities;

namespace OpenHardwareMonitor.GUI
{
    public class HardwareTypeImage
    {
        private static HardwareTypeImage instance = new HardwareTypeImage();

        private IDictionary<HardwareType, byte[]> images =
          new Dictionary<HardwareType, byte[]>();

        protected IResourceProvider resourceProvider;

        private HardwareTypeImage() { }

        public static HardwareTypeImage GetInstance(IResourceProvider resourceProvider)
        {
            instance.resourceProvider = resourceProvider;
            return instance;
        }

        public byte[] GetImage(HardwareType hardwareType)
        {
            byte[] image;
            if (images.TryGetValue(hardwareType, out image))
            {
                return image;
            }
            else {
                switch (hardwareType)
                {
                    case HardwareType.CPU:
                        image = resourceProvider.GetResource("cpu.png");
                        break;
                    case HardwareType.GpuNvidia:
                        image = resourceProvider.GetResource("nvidia.png");
                        break;
                    case HardwareType.GpuAti:
                        image = resourceProvider.GetResource("ati.png");
                        break;
                    case HardwareType.HDD:
                        image = resourceProvider.GetResource("hdd.png");
                        break;
                    case HardwareType.Heatmaster:
                        image = resourceProvider.GetResource("bigng.png");
                        break;
                    case HardwareType.Mainboard:
                        image = resourceProvider.GetResource("mainboard.png");
                        break;
                    case HardwareType.SuperIO:
                        image = resourceProvider.GetResource("chip.png");
                        break;
                    case HardwareType.TBalancer:
                        image = resourceProvider.GetResource("bigng.png");
                        break;
                    case HardwareType.RAM:
                        image = resourceProvider.GetResource("ram.png");
                        break;
                    default:
                        image = new byte[1];
                        break;
                }
                images.Add(hardwareType, image);
                return image;
            }
        }
    }
}
