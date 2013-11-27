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
		private Image bg;
		
		private Chunk currentChunk;
		private Chunk nextChunk;
		private bool changingLevels;
		
		private Dictionary<uint, string> playerImageNames;
		
		public GameWorld(Dictionary<uint, string> playerImageNames) : base()
		{
			this.playerImageNames = playerImageNames;
			
			bg = new Image(Library.GetTexture("assets/bg.png"));
			bg.ScrollX = bg.ScrollY = 0;
			AddGraphic(bg);
			
			gameManager = new GameManager();
			Add(gameManager);
			
			changingLevels = false;
			
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
				FP.World = new GameWorld(playerImageNames);
			}
			
			if (Input.Pressed(Keyboard.Key.P))
			{
				gameManager.TogglePauseGame(true);
			}
			
			if (Input.Pressed(Keyboard.Key.Return))
			{
				AdvanceLevel();
			}
			
			if (Input.Pressed(Keyboard.Key.Escape))
			{
				FP.World = new MenuWorld();
			}
		}
		
		public void CheckControllers()
		{
			foreach (var i in playerImageNames.Keys)
			{
				if (Joystick.IsConnected(i))
				{
					gameManager.AddPlayer(0, 0, i, playerImageNames[i]);
				}
			}
		}
		
		public void DelayBegin()
		{
			gameManager.StartGame();
		}
		
		public override void End()
		{
			base.End();
			
			Mixer.music.Stop();
		}
		
		
		public void AdvanceLevel()
		{
			if (!changingLevels && !gameManager.GameEnding)
			{
				changingLevels = true;
				
				if (gameManager != null)
				{
					foreach (var player in gameManager.Players)
					{
						if (player.IsAlive)
						{
							Remove(player);
						}
					}
				}
				
				var all = new List<Entity>();
				GetType("camerashake", all);
				RemoveList(all);
				
				nextChunk = new Chunk(180, (FP.Camera.Y - FP.HalfHeight) - FP.Height);
				Add(nextChunk);
			}
		}
		
		public void OnFinishAdvance()
		{
			if (currentChunk != null && currentChunk.World != null)
			{
				Remove(currentChunk);
			}
			
			currentChunk = nextChunk;
			
			SpawnPlayers();
			
			changingLevels = false;
		}
		
		public void SpawnPlayers()
		{
			foreach (var player in gameManager.Players)
			{
				if (player.Lives > 0)
				{
					player.X = currentChunk.spawnPoints[player.id].X;
					player.Y = currentChunk.spawnPoints[player.id].Y;
					Add(player);
				}
			}
		}
		
		public void UnloadCurrentChunk()
		{
			Remove(currentChunk);
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
