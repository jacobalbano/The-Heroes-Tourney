/*
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
		public List<Entity> spawnPoints;
		public SpawnManager spawnManager;
		private Image bg;
		
		private Chunk currentChunk;
		private Chunk nextChunk;
		
		public GameWorld() : base()
		{
			bg = new Image(Library.GetTexture("assets/bg.png"));
			bg.ScrollX = bg.ScrollY = 0;
			AddGraphic(bg);
			
			FP.Camera.X = FP.HalfWidth;
			FP.Camera.Y = FP.HalfHeight;
			
			gameManager = new GameManager();
			
			Input.ControllerConnected += delegate(object sender, JoystickConnectEventArgs e)
			{
				// player isn't created when plugged in after game starts
				CheckControllers();
			};
			
			
			AddTween(new Alarm(0.1f, CheckControllers, Tween.ONESHOT), true);
			AddTween(new Alarm(0.2f, DelayBegin, ONESHOT), true);
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
				FP.World = new GameWorld();
			}
			
			if (Input.Pressed(Keyboard.Key.P))
			{
				gameManager.TogglePauseGame(true);
			}
		}
		
		public void CheckControllers()
		{
			if (Joystick.IsConnected(0))
			{
				gameManager.AddPlayer(0, 0, 0);
			}
			
			if (Joystick.IsConnected(1))
			{
				gameManager.AddPlayer(0, 0, 1);
			}
			
			if (Joystick.IsConnected(2))
			{
				gameManager.AddPlayer(0, 0, 2);
			}
			
			if (Joystick.IsConnected(3))
			{
				gameManager.AddPlayer(0, 0, 3);
			}
		}
		
		public void DelayBegin()
		{
			base.Begin();
			
			Add(gameManager);
			
			AdvanceLevel();
		}
		
		
		public void AdvanceLevel()
		{
			if (gameManager != null)
			{
				foreach (var player in gameManager.Players)
				{
					if (player.World != null)
					{
						FP.Log("BAD ", player.World == null);
						Remove(player);
					}
				}
			}
			
			nextChunk = new Chunk(180, (FP.Camera.Y - FP.HalfHeight) - FP.Height);
			Add(nextChunk);
		}
		
		public void ChunkLoadComplete()
		{
			var tween = new VarTween(OnFinishAdvance, ONESHOT);
			tween.Tween(FP.Camera, "Y", FP.Camera.Y - FP.Height, 1, Ease.ElasticOut);
			AddTween(tween, true);
		}
		
		private void OnFinishAdvance()
		{
			if (currentChunk != null && currentChunk.World != null)
			{
				Remove(currentChunk);
			}
			currentChunk = nextChunk;
			
			SpawnPlayers();
		}
		
		public void SpawnPlayers()
		{
			FP.Log("spawning players ", gameManager.Players.Count, currentChunk.spawnPoints.Count);
			
			foreach (var player in gameManager.Players)
			{
				try
				{
					FP.Log("spawning player ", player.id, " at ", currentChunk.spawnPoints[player.id].X, currentChunk.spawnPoints[player.id].Y);
					player.X = currentChunk.spawnPoints[player.id].X;
					player.Y = currentChunk.spawnPoints[player.id].Y;
					Add(player);
				}
				catch (ArgumentOutOfRangeException)
				{
					throw new Exception("spawnPoints[" + player.id + "] is out of range. Make sure you have 4 spawn points in the level");
				}
			}
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
