using System;
using System.Windows.Forms;

namespace xiSpec01
{
	internal static class Program
	{
		private static int demoMode = 0;

		private static bool ignoreCameraCalib = false;

		private static string fileNameTest = "";

		[STAThread]
		private static void Main(string[] args)
		{
			if (args.Length != 0)
			{
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].Equals("-demo", StringComparison.OrdinalIgnoreCase))
					{
						demoMode = 1;
					}
					if (args[i].Equals("-demoOverwrite", StringComparison.OrdinalIgnoreCase))
					{
						demoMode = 2;
					}
					if (args[i].Equals("-vision", StringComparison.OrdinalIgnoreCase))
					{
						demoMode = 3;
					}
					if (args[i].Equals("-visionFullScreen", StringComparison.OrdinalIgnoreCase))
					{
						demoMode = 4;
					}
					if (args[i].Equals("-icc", StringComparison.OrdinalIgnoreCase))
					{
						ignoreCameraCalib = true;
					}
					if (i + 1 < args.Length && args[i].Equals("-f", StringComparison.OrdinalIgnoreCase))
					{
						fileNameTest = args[++i];
					}
				}
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(defaultValue: false);
			Application.Run(new Form1(demoMode, ignoreCameraCalib, fileNameTest));
		}
	}
}
