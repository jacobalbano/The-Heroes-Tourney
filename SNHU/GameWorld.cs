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

namespace SNHU
{
	/// <summary>
	/// Description of GameWorld.
	/// </summary>
	public class GameWorld : World
	{
		public GameManager gameManager;
		private Image bg;
		
		private Queue<Chunk> ChunkQueue;
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
			
			LoadChunks();
			
			bottomChunk = ChunkQueue.Dequeue();
			bottomChunk.X = 0;
			bottomChunk.Y = 0;
			
			topChunk = ChunkQueue.Dequeue();
			topChunk.X = 0;
			topChunk.Y = (FP.Camera.Y - FP.Height / 2) - Chunk.CHUNK_HEIGHT;
			
			Add(bottomChunk);
			Add(topChunk);
			Add(gameManager);
			
			//gameManager.AddPlayer(FP.HalfWidth, 0, 0);
			
			gameManager.StartGame();
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Input.Down(Keyboard.Key.LAlt) && Input.Pressed(Keyboard.Key.F4))
			{
				FP.Screen.Close();
			}
			
			if (Input.Pressed(Keyboard.Key.F5))
			{
				FP.World = new GameWorld();
			}
			
			if (bottomChunk.IsBelowCamera && bottomChunk.World != null)
			{
				Remove(bottomChunk);
				
				bottomChunk = topChunk;
				
				if (ChunkQueue.Count > 0)
				{
					topChunk = ChunkQueue.Dequeue();
					topChunk.X = 0;
					topChunk.Y = (FP.Camera.Y - FP.Height / 2) - Chunk.CHUNK_HEIGHT;
					Add(topChunk);
				}
			}
		}
		
		public void CheckControllers()
		{
			if (Joystick.IsConnected(0))
			{
				gameManager.AddPlayer(FP.HalfWidth - 100, -50, 0);
			}
			
			if (Joystick.IsConnected(1))
			{
				gameManager.AddPlayer(FP.HalfWidth - 50, -50, 1);
			}
			
			if (Joystick.IsConnected(2))
			{
				gameManager.AddPlayer(FP.HalfWidth, -50, 2);
			}
			
			if (Joystick.IsConnected(3))
			{
				gameManager.AddPlayer(FP.HalfWidth - 75, -50, 3);
			}
		}
		
		private void LoadChunks()
		{
			ChunkQueue = new Queue<Chunk>();
			
			ChunkQueue.Enqueue(new Chunk(0,0,"start"));
			
			for (int i = 0; i < 5; i++)
			{
				ChunkQueue.Enqueue(new Chunk(0,0));
			}
			
			ChunkQueue.Enqueue(new Chunk(0,0,"end"));
		}
	}
}
