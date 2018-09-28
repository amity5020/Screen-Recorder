using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetResolution
{
    public class GetResolution
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public GetResolution()
        {
            SetDpiAwareness(ProcessDPIAwareness.ProcessPerMonitorDPIAware);
            Height = SystemInformation.VirtualScreen.Height;
            Width = SystemInformation.VirtualScreen.Width;
        }

        private enum ProcessDPIAwareness
        {
            ProcessDPIUnaware = 0,
            ProcessSystemDPIAware = 1,
            ProcessPerMonitorDPIAware = 2
        }

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);

        private static void SetDpiAwareness(ProcessDPIAwareness processDPIAwareness)
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    SetProcessDpiAwareness(processDPIAwareness);
                }
            }
            catch (EntryPointNotFoundException)//this exception occures if OS does not implement this API, just ignore it.
            {
            }
        }
    }
}
