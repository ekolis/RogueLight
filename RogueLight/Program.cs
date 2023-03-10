using RogueLight.Mapping;
using RogueLight.UI;

namespace RogueLight
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			int when = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			int seed;
			if (args.Length > 0)
			{
				if (!int.TryParse(args[0], out seed))
					seed = when;
			}
			else
				seed = when;
			World.Setup(seed);
			World.Instance.ClimbDown();
			Application.Run(new GameForm());
		}
	}
}