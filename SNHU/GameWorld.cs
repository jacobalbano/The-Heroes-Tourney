/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/15/2013
 * Time: 7:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
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
		
		public GameWorld() : base()
		{
			gameManager = new GameManager();
			
			AddGraphic(new Image(Library.GetTexture("assets/bg.png")));	
			Add(new Player(FP.HalfWidth, 0));
			Add(new Chunk(180, 0));
			Add(gameManager);
			
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
		}
	}
}
