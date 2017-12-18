using System;
using Indigo;
using Indigo.Inputs;
using Indigo.Utils;
using SNHU.GameObject;
using SNHU.Systems;

namespace SNHU
{
	class Program : Engine
	{
		public Program() : base(1000, 600, 60)
		{
#if DEBUG
			Engine.Console.Enable();
			//Engine.Console.MirrorToSystemOut = true;
			#endif
			
			Library.AddPath("assets/");
			Library.AddPath("assets/mods");

			Engine.World = new MenuWorld();
			Mixer.Preload();
			
			Engine.Screen.Title = "The Heroes' Tourney";
			Mouse.CursorVisible = false;

			Engine.GlobalKeys.Add(Quit)
				.Down(new InputList.Any(Keyboard.LAlt, Keyboard.RAlt))
				.Pressed(Keyboard.F4);

			Engine.GlobalKeys.Add(Fullscreen)
				.Pressed(Keyboard.F);

			Engine.GlobalKeys.Add(Fullscreen)
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
			Engine.Screen.Fullscreen = !Engine.Screen.Fullscreen;
		}
	}
}