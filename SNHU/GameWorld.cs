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
			
			Input.ControllerConnected += delegate(object sender, JoystickConnectEventArgs e)
			{
				CheckControllers();
			};
			
			CheckControllers();
			
			AddGraphic(new Image(Library.GetTexture("assets/bg.png")));	
			Add(new Chunk(0, 0));
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
		}
		
		public void CheckControllers()
		{
			if (Joystick.IsConnected(0))
			{
				gameManager.AddPlayer(FP.HalfWidth, -50, 0);
			}
			
			if (Joystick.IsConnected(1))
			{
				gameManager.AddPlayer(FP.HalfWidth, -50, 1);
			}
			
			if (Joystick.IsConnected(2))
			{
				gameManager.AddPlayer(FP.HalfWidth, -50, 2);
			}
			
			if (Joystick.IsConnected(3))
			{
				gameManager.AddPlayer(FP.HalfWidth, -50, 3);
			}
		}
	}
}
