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
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using SNHU.Components;
using SNHU.GameObject.Platforms;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of Player.
	/// </summary>
	public class Player : Entity
	{
		public const string OnLand = "player_onLand";
		
		public const float JumpForce = -13;
		
		private const float JUMP_JUICE = 0.3f;
		
		private Image image;
		private VirtualAxis axis;
		
		private PhysicsBody physics;
		private bool OnGround;
		
		public const float SPEED = 6;
		
		public Player(float x, float y) : base(x, y)
		{
			Graphic = image = Image.CreateRect(32, 64, FP.Color(0xff00ff));
			image.CenterOO();
			SetHitboxTo(image);
			CenterOrigin();
			
			OriginY = Height;
			image.OriginY = Height;
			
			axis = VirtualAxis.WSAD();
			
			physics = new PhysicsBody();
			physics.Colliders.Add(Platform.Collision);
			AddLogic(physics);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Collide(Platform.Collision, X, Y + 1) == null)
			{
				OnGround = false;
			}
			
			if (Input.Pressed(Keyboard.Key.Space))
			{
				OnMessage(PhysicsBody.IMPULSE, 0, JumpForce);
			}
			
			MoveBy(axis.X * SPEED, 0, Platform.Collision);
		}
		
		public override bool MoveCollideY(Entity e)
		{
			if (e.Type == Platform.Collision)
			{
				if (!OnGround)
				{
					image.ScaleX = 1 + JUMP_JUICE;
					image.ScaleY = 1 - JUMP_JUICE;
					
					var tween = new MultiVarTween(null, ONESHOT);
					tween.Tween(image, new { ScaleX = 1, ScaleY = 1}, 0.2f);
					AddTween(tween, true);
					
					OnGround = true;
				}
				
				OnMessage(OnLand);
				Mixer.Audio["land1"].Play();
			}
			
			return base.MoveCollideY(e);
		}
	}
}
