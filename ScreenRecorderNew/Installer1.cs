using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ScreenRecorderNew
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        public Installer1()
        {
            InitializeComponent();
        }
        public override void Install(IDictionary savedState)
        {
            base.Install(savedState);
            // File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//Screen Recorder.lnk", Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "//Screen Recorder.lnk", true);
            //MessageBox.Show("Install");
        }
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
        }
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            SetStartup();
          //  MessageBox.Show("Commit");
        }



        public static string GetApplicationDirectory_New()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        public void SetStartup()
        {
            RegistryKey rKey = Registry.ClassesRoot.OpenSubKey("TrezleRecorder", true);
            //if (rKey == null)
            //{
                rKey = Registry.ClassesRoot.CreateSubKey("TrezleRecorder");
                rKey.SetValue("", "URL: Trezle Recorder Protocol");
                rKey.SetValue("URL Protocol", "");

                rKey = rKey.CreateSubKey(@"shell\open\command");
                rKey.SetValue("", "\"" + GetApplicationDirectory_New() + "\\ScreenRecorder.exe" + "\" %1");
            //}
            //if (rKey != null)
            //{
            //    rKey.Close();
            //}
        }
    }
}
