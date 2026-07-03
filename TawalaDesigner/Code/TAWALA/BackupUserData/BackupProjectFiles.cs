using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;

namespace BackupUserData
{
	[RunInstaller(true)]
	public partial class BackupProjectFiles : Installer
	{
		public BackupProjectFiles()
		{
			InitializeComponent();
		}

		public override void Install(System.Collections.IDictionary stateSaver)
		{
			base.Install(stateSaver);

			// parameters set via the CustomActionData property in the Setup Custom Actions Editor
			string personalFolderRoot = Context.Parameters["PersonalFolderRoot"];
			string productName = Context.Parameters["ProductName"];
			if (string.IsNullOrEmpty(personalFolderRoot) || string.IsNullOrEmpty(productName))
			{
				return;
			}

			string projectFolder = Path.Combine(personalFolderRoot, productName);
			string backupFolder = Path.Combine(projectFolder, "__Backup (installation)");

			try
			{
				Directory.CreateDirectory(backupFolder);
				if (Directory.Exists(backupFolder))
				{
					string[] files = Directory.GetFiles(backupFolder, "*.tawala");
					foreach (string file in files)
					{
						File.SetAttributes(file, File.GetAttributes(file) & ~FileAttributes.ReadOnly);
					}
					
					files = Directory.GetFiles(projectFolder, "*.tawala");
					foreach (string file in files)
					{
						File.Copy(file, Path.Combine(backupFolder, Path.GetFileName(file)), true);
					}
				}
			}
			catch (Exception)
			{
				// no need to throw exception
				// failure to make backup copies of Project files is not fatal
			}
		}
	}
}