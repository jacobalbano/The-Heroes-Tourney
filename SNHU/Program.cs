using System;
using Punk;
using SFML.Window;

namespace SNHU
{
	class Program : Engine
	{
		public Program() : base(1000, 600, 60) {}
		
		public override void Init()
		{
			base.Init();
			
			#if DEBUG
			FP.Console.Enable();
			FP.Screen.GainedFocus += delegate { Library.Reload(); };
			#endif
			
			FP.World = new MenuWorld();
			FP.Screen.SetTitle("The Heroes' Tourney");
			FP.Screen.SetMouseCursorVisible(false);
		}
		
		public static void Main(string[] args)
		{
			var game = new Program();
		}
	}
}