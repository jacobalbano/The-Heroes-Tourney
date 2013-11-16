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
		public List<Entity> activeSpawns;
		public SpawnManager spawnManager;
		private Image bg;
		
		private Chunk bottomChunk;
		private Chunk topChunk;
		
		public GameWorld() : base()
		{
			FP.Camera.X = FP.HalfWidth;
			FP.Camera.Y = FP.HalfHeight;
			
			gameManager = new GameManager();
			
			Input.ControllerConnected += delegate(object sender, JoystickConnectEventArgs e)
			{
				// player isn't created when plugged in after game starts
				CheckControllers();
			};
			
			AddTween(new Alarm(0.1f, CheckControllers, Tween.ONESHOT), true);
			
			bg = new Image(Library.GetTexture("assets/bg.png"));
			bg.ScrollX = bg.ScrollY = 0;
			AddGraphic(bg);	
			
			bottomChunk = new Chunk(0,0, "start");
			bottomChunk.X = 180;
			bottomChunk.Y = 0;
			
			topChunk = new Chunk(0,0);
			topChunk.X = 180;
			topChunk.Y = (FP.Camera.Y - FP.Height / 2) - Chunk.CHUNK_HEIGHT;
			
			Add(bottomChunk);
			Add(topChunk);
			
			Add(gameManager);
			spawnManager = new SpawnManager();
			Add(spawnManager);
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
			
//			if (bottomChunk.IsBelowCamera && bottomChunk.World != null)
//			{
//				Remove(bottomChunk);
//				
//				bottomChunk = midChunk;
//				midChunk = topChunk;
//				
//				if (ChunkQueue.Count > 0)
//				{
//					topChunk = ChunkQueue.Dequeue();
//					topChunk.X = 180;
//					topChunk.Y = midChunk.Y - Chunk.CHUNK_HEIGHT;
//					Add(topChunk);
//				}
//			}
			
			
		}
		
		public void AdvanceLevel()
		{
			var tween = new VarTween(OnFinishAdvance, ONESHOT);
			tween.Tween(FP.Camera, "Y", FP.Camera.Y - FP.Height, 1, Ease.ElasticOut);
			AddTween(tween, true);
		}
		
		private void OnFinishAdvance()
		{
			Remove(bottomChunk);
			
			bottomChunk = topChunk;
			
			topChunk = new Chunk(0,0);
			topChunk.X = 180;
			topChunk.Y = (FP.Camera.Y - FP.Height / 2) - Chunk.CHUNK_HEIGHT;
			
		}
		
		public void CheckControllers()
		{
			if (Joystick.IsConnected(0))
			{
				gameManager.AddPlayer(64, 480, 0);
			}
			
			if (Joystick.IsConnected(1))
			{
				gameManager.AddPlayer(144, 480, 1);
			}
			
			if (Joystick.IsConnected(2))
			{
				gameManager.AddPlayer(464, 480, 2);
			}
			
			if (Joystick.IsConnected(3))
			{
				gameManager.AddPlayer(560, 480, 3);
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
