using System;
using Punk;
using Punk.Utils;
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
			#endif
			
			FP.World = new MenuWorld();
			FP.Screen.Title = "The Heroes' Tourney";
			Input.CursorVisible = false;
		}
		
		public static void Main(string[] args)
		{
			var game = new Program();
		}
	}
}