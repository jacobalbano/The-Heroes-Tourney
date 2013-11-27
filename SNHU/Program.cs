using System;
using Punk;

namespace SNHU
{
	class Program : Engine
	{
		public Program() : base(1000, 600, 60) {}
		
		public override void Init()
		{
			base.Init();
			
			FP.Console.Enable();
			FP.World = new MenuWorld();
			
			FP.Screen.SetTitle("The Heroes' Tourney");
			
			FP.Screen.GainedFocus += delegate { Library.Reload(); };
		}
		
		public static void Main(string[] args)
		{
			var game = new Program();
		}
	}
}