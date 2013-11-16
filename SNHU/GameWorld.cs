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
			Add(new Chunk(0, 0));
			Add(gameManager);
			
			gameManager.StartGame();
		}
	}
}
