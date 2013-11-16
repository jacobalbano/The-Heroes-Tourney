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
		
		public const string Collision = "player";
		
		public const float JumpForce = -20;
		
		private const float JUMP_JUICE_FORCE = 0.3f;
		private const float JUMP_JUICE_DURATION = 0.17f;
		
		private Image player;
		private Fist left, right;
		private Controller controller;
		private Axis axis;
		
		private bool hand;
		
		private PhysicsBody physics;
		private bool OnGround;
		
		public const float SPEED = 6;
		
		public int Points;
		public uint Deaths { get; private set; }
		public uint id { get; private set;}
		
		public Player(float x, float y, uint id) : base(x, y)
		{
			this.id = id;
			
			hand = false;
			
			controller = new Controller(id);
//			if (Joystick.IsConnected(id))
//			{
				axis = controller.LeftStick;
//			}
//			else
//			{
//				axis = VirtualAxis.WSAD();
//			}
//			
			player = new Image(Library.GetTexture("assets/player.png"));
			SetTint(id);
			AddGraphic(player);
			
			player.CenterOO();
			SetHitboxTo(player);
			CenterOrigin();
			
			OriginY = Height;
			player.OriginY = Height;
			
			left = new Fist(true, this);
			right = new Fist(false, this);
			
			Type = Collision;
			physics = new PhysicsBody();
			physics.Colliders.Add(Platform.Collision);
			physics.Colliders.Add(Type);
			AddLogic(physics);
			
			Points = 0;
			Deaths = 0;
			
			#if DEBUG
			AddLogic(new CheckRestart(controller));
			#endif
		}
		
		public override void Added()
		{
			base.Added();
			World.AddList(left, right);
		}
		
		public override void Removed()
		{
			base.Removed();
			World.RemoveList(left, right);
		}
		
		void FaceLeft()
		{
			player.FlippedX = true;
			left.FaceLeft();
			right.FaceLeft();
		}
		
		void FaceRight()
		{
			player.FlippedX = false;
			
			left.FaceRight();
			right.FaceRight();
		}
		
		public override void Update()
		{
			base.Update();
		 	
		 	if (axis.X < 0)
		 	{
		 		FaceLeft();
		 	}
		 	else if (axis.X > 0)
		 	{
		 		FaceRight();
		 	}
		 	
			if (Collide(Platform.Collision, X, Y + 1) == null)
			{
				OnGround = false;
			}
			else
			{
				OnMessage(PhysicsBody.FRICTION, 0.75f);
			}
			
			HandleInput();
			
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
		
		private void HandleInput()
		{
			if (OnGround && (controller.Pressed(Controller.Button.A) || Input.Pressed(Keyboard.Key.Space)))
			{
				OnMessage(PhysicsBody.IMPULSE, 0, JumpForce);
				Mixer.Audio[FP.Choose("jump1", "jump2", "jump3")].Play();
				
				ClearTweens();
				player.ScaleX = 1 - JUMP_JUICE_FORCE;
				player.ScaleY = 1 + JUMP_JUICE_FORCE;
				
				var tween = new MultiVarTween(null, ONESHOT);
				tween.Tween(player, new { ScaleX = 1, ScaleY = 1}, JUMP_JUICE_DURATION);
				AddTween(tween, true);
			}
			
			if (controller.Pressed(Controller.Button.Y))
			{
				World.Add(new Meteor());
			}
			
			if (controller.Pressed(Controller.Button.X))
			{
				Punch();
			}
			
		}
		
		private void Punch()
		{
			if (hand)
			{
				left.Punch();
			}
			else
			{
				right.Punch();
			}
			
			hand = !hand;
		}
		
		
		public override bool MoveCollideY(Entity e)
		{
			if (e.Type == Platform.Collision)
			{
				if (!OnGround)
				{
					player.ScaleX = 1 + JUMP_JUICE_FORCE;
					player.ScaleY = 1 - JUMP_JUICE_FORCE;
					
					var tween = new MultiVarTween(null, ONESHOT);
					tween.Tween(player, new { ScaleX = 1, ScaleY = 1}, JUMP_JUICE_DURATION);
					AddTween(tween, true);
					
					OnGround = true;
					
					Mixer.Audio["land1"].Play();
					
					if (e.Y >= Y)
					{
						e.OnMessage(Platform.PlayerLand, this);
					}
					else
					{
						OnMessage(PhysicsBody.IMPULSE, 0, 1, true);
					}
				}
				
				
				OnMessage(OnLand);
			}
			else if (e.Type == Type)
			{
				if (e.Y > Y)
				{
					OnMessage(PhysicsBody.IMPULSE, (int) FP.Rand(10) - 5, JumpForce, true);
					e.OnMessage(PhysicsBody.IMPULSE, (int) FP.Rand(10) - 5, -JumpForce / 2, true);
				}
			}
			
			return base.MoveCollideY(e);
		}
		
		public void SetTint(uint id)
		{
			switch (id)
			{
				case 0:
					player.Color = FP.Color(0x88FF88);
					break;
				case 1:
					player.Color = FP.Color(0xFF8888);
					break;
				case 2:
					player.Color = FP.Color(0x8888FF);
					break;
				case 3:
					player.Color = FP.Color(0xFFFF88);
					break;
				default:
					break;
			}
		}
	}
}
