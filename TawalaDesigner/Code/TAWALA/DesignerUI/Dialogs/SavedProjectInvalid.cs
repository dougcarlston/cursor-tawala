// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Tawala.DesignerUI
{
    public partial class SavedProjectInvalid : Form
    {
        private static readonly string mailToFormat = "mailto:{0}?subject={1}&body={2}";
        private static readonly string subjectFormat = "Tawala Project file error: {0}";
        private static readonly string tawalaContactList = "jdf@tawala.com; johns@tawala.com";

        private static string bodyFormat =
            "1. ATTACH THE FOLLOWING FILE TO THIS MESSAGE (you can drag it from the Explorer window we just opened):" + Environment.NewLine +
            Environment.NewLine +
            "            {0}" + Environment.NewLine + Environment.NewLine +
            "2. IN THE SPACE BELOW, PLEASE PROVIDE ANY INFORMATION YOU CAN regarding actions you took prior to seeing this error:" +
            Environment.NewLine +
            Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine +
            "Please send the file even you have no additional information to provide. The data it contains will help us track down the bug and improve Tawala." +
            Environment.NewLine +
            "You can delete the file from your system after sending it." +
            Environment.NewLine + Environment.NewLine +
            "Thank you," + Environment.NewLine +
            "The Tawala Team";

        private string debugFilePath = null;

        public SavedProjectInvalid()
        {
            InitializeComponent();
        }

        public SavedProjectInvalid(string debugFilePath)
            : this()
        {
            this.debugFilePath = debugFilePath;
            labelFile.Text = debugFilePath;
        }

        private void linkLabelNotepad_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("notepad", string.Format("\"{0}\"", debugFilePath));
        }

        private void linkLabelView_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            launchExplorerWithBadFileSelected();
        }

        private void linkLabelSendReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process explorer = launchExplorerWithBadFileSelected();
            Thread.Sleep(1000);

            string subject = string.Format(subjectFormat, Path.GetFileName(debugFilePath));
            string body = Uri.EscapeUriString(string.Format(bodyFormat, debugFilePath));

            var sb = new StringBuilder();
            sb.AppendFormat(mailToFormat,
                            tawalaContactList,
                            subject,
                            body);

            Process.Start(sb.ToString());

            // Note: The following didn't work with larger files; got a "data area passed to a system call is too small" exception;
            //       Also tried attaching the file (with "&attachment" in the mailto link) but Outlook wouldn't accept that
            //																					jdf - 8/08
            //string debugFileContents = Uri.EscapeUriString(File.ReadAllText(badFilePath));
            //
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat(mailToFormat,
            //                recipients,
            //                subject,
            //                body + debugFileContents);
        }

        private Process launchExplorerWithBadFileSelected()
        {
            return Process.Start("explorer", string.Format("/select,{0}", debugFilePath));
        }
    }
}