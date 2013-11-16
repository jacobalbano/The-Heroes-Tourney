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
		
		private const float JUMP_JUICE_FORCE = 0.3f;
		private const float JUMP_JUICE_DURATION = 0.17f;
		
		private Image image;
		private Controller controller;
		private Axis axis;
		
		private PhysicsBody physics;
		private bool OnGround;
		
		public const float SPEED = 6;
		
		private int points;
		private uint deaths;
		private uint id;
		
		public Player(float x, float y, uint id) : base(x, y)
		{
			this.id = id;
			controller = new Controller(id);
			if (Joystick.IsConnected(id))
			{
				axis = controller.LeftStick;
			}
			else
			{
				axis = VirtualAxis.WSAD();
			}
			
			Graphic = image = new Image(Library.GetTexture("assets/player.png"));
			image.Color = FP.Color(FP.Rand(uint.MaxValue));
			
			image.CenterOO();
			SetHitboxTo(image);
			CenterOrigin();
			
			OriginY = Height;
			image.OriginY = Height;
			
			
			physics = new PhysicsBody();
			physics.Colliders.Add(Platform.Collision);
			AddLogic(physics);
			
			points = 0;
			deaths = 0;
			
			#if DEBUG
			AddLogic(new CheckRestart(controller));
			#endif
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Collide(Platform.Collision, X, Y + 1) == null)
			{
				OnGround = false;
			}
			else
			{
				OnMessage(PhysicsBody.FRICTION, 0.75f);
			}
			
			if (OnGround && (controller.Pressed(Controller.Button.A) || Input.Pressed(Keyboard.Key.Space)))
			{
				OnMessage(PhysicsBody.IMPULSE, 0, JumpForce);
				Mixer.Audio[FP.Choose("jump1", "jump2", "jump3")].Play();
			}
			
			if (Math.Abs(physics.MoveDelta.X) < SPEED)
			{
				var delta = FP.Sign(physics.MoveDelta.X);
				var ax = FP.Sign(axis.X);
				
				if (delta == 0 || delta == ax)
				{
					OnMessage(PhysicsBody.IMPULSE, axis.X * SPEED, 0);
				}
				else
				{
					OnMessage(PhysicsBody.IMPULSE, axis.X * (SPEED / 3f), 0);
				}
				
			}
		}
		
		public override bool MoveCollideY(Entity e)
		{
			if (e.Type == Platform.Collision)
			{
				if (!OnGround)
				{
					image.ScaleX = 1 + JUMP_JUICE_FORCE;
					image.ScaleY = 1 - JUMP_JUICE_FORCE;
					
					var tween = new MultiVarTween(null, ONESHOT);
					tween.Tween(image, new { ScaleX = 1, ScaleY = 1}, JUMP_JUICE_DURATION);
					AddTween(tween, true);
					
					OnGround = true;
					
					Mixer.Audio["land1"].Play();
				}
				
				OnMessage(OnLand);
			}
			
			return base.MoveCollideY(e);
		}
		
		public int Points
		{
			get { return points; }
		}
		
		public uint Deaths
		{
			get { return deaths; }
		}
	}
}
