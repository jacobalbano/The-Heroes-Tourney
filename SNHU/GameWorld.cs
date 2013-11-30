﻿/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/15/2013
 * Time: 7:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
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
		
		private Dictionary<uint, string> playerImageNames;
		
		public GameWorld()
		{
			FP.Camera.X = FP.HalfWidth;
			FP.Camera.Y = FP.HalfHeight;
			
			bg = new Image(Library.GetTexture("assets/bg.png"));
			bg.ScrollX = bg.ScrollY = 0;
			AddGraphic(bg);
			
			Add(gameManager = new GameManager());
			Add(chunkManager = new ChunkManager());
			
			chunkManager.OnMessage(ChunkManager.PreloadNext);
		}
		
		public void Init(Dictionary<uint, string> playerImageNames)
		{
			this.playerImageNames = playerImageNames;
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Input.Down(Keyboard.Key.LAlt) && Input.Pressed(Keyboard.Key.F4))
			{
				FP.Screen.Close();
			}
			
			if (Input.Pressed(Keyboard.Key.F5) || Input.Pressed(Keyboard.Key.R))
			{
				var gameWorld = new GameWorld();
				gameWorld.Init(playerImageNames);
				FP.World = gameWorld;
			}
			
			if (Input.Pressed(Keyboard.Key.P))
			{
				gameManager.TogglePauseGame(true);
			}
			
			if (Input.Pressed(Keyboard.Key.Return))
			{
				chunkManager.OnMessage(ChunkManager.Advance);
			}
			
			if (Input.Pressed(Keyboard.Key.Escape))
			{
				FP.World = new MenuWorld();
			}
		}
		
		public override void Begin()
		{
			base.Begin();
			
			foreach (var i in playerImageNames.Keys)
			{
				if (Joystick.IsConnected(i))
				{
					gameManager.AddPlayer(0, 0, i, playerImageNames[i]);
				}
			}
			
			var black = Image.CreateRect(FP.Width, FP.Height, FP.Color(0));
			
			AddGraphic(black, -100);
			
			var tween = new VarTween(StartGame, ONESHOT);
			tween.Tween(black, "Alpha", 0, 0.25f, Ease.SineOut);
			AddTween(tween, true);
		}
		
		private void StartGame()
		{
			gameManager.StartGame();
			chunkManager.OnMessage(ChunkManager.Advance);
		}
		
		public override void End()
		{
			base.End();
			
			Mixer.music.Stop();
		}
		
		public static bool OnCamera(float x, float y)
		{
			return
				x > FP.Camera.X - FP.HalfWidth &&
				x < FP.Camera.X + FP.HalfWidth &&
				y > FP.Camera.Y - FP.HalfHeight &&
				y < FP.Camera.Y + FP.HalfHeight;
		}
	}
}
