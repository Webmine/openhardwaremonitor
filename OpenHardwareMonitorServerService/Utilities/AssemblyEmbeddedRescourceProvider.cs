using OpenHardwareMonitor.Utilities;
using OpenHardwareMonitorServerService.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenHardwareMonitorServerService
{
    public class AssemblyEmbeddedRescourceProvider : IResourceProvider
    {
        string resourcePrefix;

        public AssemblyEmbeddedRescourceProvider(string resourcePrefix)
        {
            this.resourcePrefix = resourcePrefix;
        }

        public byte[] GetResource(string name)
        {
            name = resourcePrefix + "." + name;

            string[] names =
              Assembly.GetExecutingAssembly().GetManifestResourceNames();
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].Replace('\\', '.') == name)
                {
                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(names[i]))
                    {
                        byte[] buffer = stream.ReadToEnd();

                        return buffer;
                    }
                }
            }

            return new byte[1];
        }
    }

    public static class StreamHelper
    {
        public static byte[] ReadToEnd(this System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }
}
