using System;
using Indigo;
using Indigo.Inputs;
using Indigo.Utils;

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
			#endif
			
			FP.World = new MenuWorld();
			FP.Screen.Title = "The Heroes' Tourney";
			Mouse.CursorVisible = false;
		}
		
		public static void Main(string[] args)
		{
			var game = new Program();
		}
	}
}