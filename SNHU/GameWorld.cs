using System;
using System.Collections.Generic;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Inputs;
using Indigo.Utils;
using SNHU.Config;
using SNHU.GameObject;
using SNHU.GameObject.Platforms;
using SNHU.Systems;

namespace SNHU
{
	/// <summary>
	/// Description of GameWorld.
	/// </summary>
	public class GameWorld : World
	{
//		private GameManager gameManager;
		private PlayerManager playerManager;
		private HUD hud;
		public ChunkManager chunkManager;
		
		private Image bg;
		
		private Dictionary<int, string> playerImageNames;
		
		public GameWorld(Dictionary<int, string> playerImageNames)
		{
			FP.Camera.X = FP.HalfWidth;
			FP.Camera.Y = FP.HalfHeight;
			
			this.playerImageNames = playerImageNames;
			
			bg = new Image(Library.GetTexture("bg.png"));
			bg.ScrollX = bg.ScrollY = 0;
			AddGraphic(bg, 0, 0, ObjectLayers.Background);
			
			AddManager(playerManager = new PlayerManager());
			Add(chunkManager = new ChunkManager());
			Add(hud = new HUD());
			
			playerManager.CreatePlayers(playerImageNames);
			hud.AddPlayers(playerManager.ActivePlayers);
		}
		
		public override void Begin()
		{
			base.Begin();
			
			var black = Image.CreateRect(FP.Width, FP.Height, new Color());
			AddGraphic(black, 0, 0, ObjectLayers.Foreground);
			
			Tweener.Tween(black, new { Alpha = 0 }, 0.25f)
				.Ease(Ease.SineOut)
				.OnComplete(StartGame);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Keyboard.R.Pressed)
				FP.World = new GameWorld(playerImageNames);
			
			if (Keyboard.Return.Pressed)
				chunkManager.OnMessage(ChunkManager.Message.Advance);
			
			if (Keyboard.Escape.Pressed)
			{
				FP.World = new MenuWorld();
			}
		}
		
		public override void End()
		{
			base.End();
			Mixer.Music.Stop();
		}

		private void StartGame()
		{
			chunkManager.OnMessage(ChunkManager.Message.Advance);
		}
	}
}
