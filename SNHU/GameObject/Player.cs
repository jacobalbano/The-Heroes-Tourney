/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/15/2013
 * Time: 8:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using Punk.Utils;
using SFML.Window;
using SNHU.Components;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of Player.
	/// </summary>
	public class Player : Entity
	{
		private Image image;
		private VirtualAxis axis;
		
		private PhysicsBody physics;
		
		public const float SPEED = 6;
		
		public Player(float x, float y) : base(x, y)
		{
			Graphic = image = Image.CreateRect(32, 64, FP.Color(0xff00ff));
			image.CenterOO();
			SetHitboxTo(image);
			CenterOrigin();
			
			axis = VirtualAxis.WSAD();
			
			physics = new PhysicsBody();
			physics.Colliders.Add(Platform.Collision);
			AddLogic(physics);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Input.Pressed(Keyboard.Key.Space))
			{
				//	NO WORKY
				OnMessage(PhysicsBody.IMPULSE, 0, -25);
			}
			
			MoveBy(axis.X * SPEED, 0, Platform.Collision);
		}
	}
}
