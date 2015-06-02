using System;
using Indigo;
using Indigo.Inputs;
using Indigo.Utils;
using SNHU.GameObject;

namespace SNHU
{
	class Program : Engine
	{
		public Program() : base(1000, 600, 60)
		{
			#if DEBUG
			FP.Console.Enable();
			FP.Console.MirrorToSystemOut = true;
			#endif
			
			Library.AddPath("assets/");
			Library.AddPath("assets/mods");
			
			FP.World = new MenuWorld();
			Mixer.Preload();
			
			FP.Screen.Title = "The Heroes' Tourney";
			Mouse.CursorVisible = false;
			
			FP.GlobalKeys.Add(Quit)
				.Down(new InputList.Any(Keyboard.LAlt, Keyboard.RAlt))
				.Pressed(Keyboard.F4);
			
			FP.GlobalKeys.Add(Fullscreen)
				.Pressed(Keyboard.F);
			
			FP.GlobalKeys.Add(Fullscreen)
				.Down(new InputList.Any(Keyboard.LAlt, Keyboard.RAlt))
				.Pressed(Keyboard.Return);
		}
		
		public static void Main(string[] args)
		{
			var game = new Program();
			game.Run();
		}
		
		private void Fullscreen()
		{
			FP.Screen.Fullscreen = !FP.Screen.Fullscreen;
		}
	}
}