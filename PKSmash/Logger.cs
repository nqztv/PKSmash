using System;
using System.IO;

namespace PKSmash
{
	public static class Logger
	{
		static string file = "";

		public static void Init()
		{
			file = "debug" + DateTime.Now.ToString("o").Replace(':', '-').Substring(0, 19) + ".log";
		}

		public static void Log(string message)
		{
			if (file == "")
			{
				return;
			}

			using (StreamWriter w = File.AppendText(file))
			{
				w.WriteLine(DateTime.Now.ToString("o") + ": " + message);
			}
		}
	}
}