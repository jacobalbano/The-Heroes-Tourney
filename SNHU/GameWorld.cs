using System;
using System.Collections.Generic;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Inputs;
using Indigo.Utils;
using SNHU.GameObject;
using SNHU.GameObject.Platforms;

namespace SNHU
{
	/// <summary>
	/// Description of GameWorld.
	/// </summary>
	public class GameWorld : World
	{
		public static GameManager gameManager;
		public ChunkManager chunkManager;
		
		private Image bg;
		
		private Dictionary<int, string> playerImageNames;
		
		public GameWorld()
		{
			FP.Camera.X = FP.HalfWidth;
			FP.Camera.Y = FP.HalfHeight;
			
			bg = new Image(Library.GetTexture("assets/bg.png"));
			bg.ScrollX = bg.ScrollY = 0;
			AddGraphic(bg);
			
			Add(gameManager = new GameManager());
			Add(chunkManager = new ChunkManager());
		}
		
		public void Init(Dictionary<int, string> playerImageNames)
		{
			this.playerImageNames = playerImageNames;
		}
		
		public override void Update()
		{
			base.Update();
			
			if ((Keyboard.LAlt.Down || Keyboard.RAlt.Down) && Keyboard.F4.Pressed)
			{
				FP.Engine.Quit();
			}
			
			if (Keyboard.F.Pressed)
			{
				FP.Screen.Fullscreen = !FP.Screen.Fullscreen;
			}
			
			if (Keyboard.R.Pressed)
			{
				var gameWorld = new GameWorld();
				gameWorld.Init(playerImageNames);
				FP.World = gameWorld;
			}
			
			if (Keyboard.P.Pressed)
			{
				gameManager.TogglePauseGame(true);
			}
			
			if (Keyboard.Return.Pressed)
			{
				chunkManager.OnMessage(ChunkManager.Message.Advance);
			}
			
			if (Keyboard.Escape.Pressed)
			{
				FP.World = new MenuWorld();
			}
		}
		
		public override void Begin()
		{
			base.Begin();
			
			foreach (var i in playerImageNames.Keys)
			{
				if (GamepadManager.GetSlot(i).IsConnected)
				{
					gameManager.AddPlayer(0, 0, i, playerImageNames[i]);
				}
			}
			
			var black = Image.CreateRect(FP.Width, FP.Height, FP.Color(0));
			
			AddGraphic(black, -100);
			
			Tweener.Tween(black, new { Alpha = 0 }, 0.25f)
				.Ease(Ease.SineOut)
				.OnComplete(StartGame);
		}
		
		private void StartGame()
		{
			gameManager.StartGame();
			chunkManager.OnMessage(ChunkManager.Message.Advance);
		}
		
		public override void End()
		{
			base.End();
			
			Mixer.music.Stop();
		}
	}
}
