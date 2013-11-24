/*
 * Created by SharpDevelop.
 * User: Jake
 * Date: 11/15/2013
 * Time: 8:08 PM
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
		
		public const float JumpForce = -15;
		private const float JUMP_JUICE_FORCE = 0.3f;
		private const float JUMP_JUICE_DURATION = 0.17f;
		
		private Image player;
		private Fist left, right;
		private Controller controller;
		private Axis axis;
		
		private bool hand;
		
		private PhysicsBody physics;
		private bool OnGround;
		
		public const float SPEED = 5.5f;
		
		public const int STARTING_LIVES = 5;
		public bool IsAlive { get; private set; }
		
		public int Lives { get; private set; }
		public int id { get; private set;}
		
		public Player(float x, float y, int id, string imageName) : base(x, y)
		{
			this.id = id;
			
			hand = false;
			
			controller = new Controller((uint)id);
			axis = controller.LeftStick;
				
			player = new Image(Library.GetTexture("assets/" + imageName));
			player.Scale = 0.5f;
//			SetTint(id);
			AddGraphic(player);
			
			player.CenterOO();
			SetHitbox(player.ScaledWidth, player.ScaledHeight, (int) player.OriginX, player.ScaledHeight);
			CenterOrigin();
			
			OriginY = player.ScaledHeight;
			player.OriginY = player.Height;
			
			left = new Fist(true, this);
			right = new Fist(false, this);
			
			Type = Collision;
			physics = new PhysicsBody();
			physics.Colliders.Add(Platform.Collision);
			physics.Colliders.Add(Type);
			AddLogic(physics);
			
			Lives = STARTING_LIVES;
			IsAlive = false;
			
			#if DEBUG
			AddLogic(new CheckRestart(controller));
			#endif
		}
		
		public override void Added()
		{
			base.Added();
			
			World.AddList(left, right);
			IsAlive = true;
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
		 	
			if (!GameWorld.gameManager.GameEnding)
			{
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
						OnMessage(PhysicsBody.IMPULSE, axis.X * SPEED, 0, true);
					}
					else
					{
						OnMessage(PhysicsBody.IMPULSE, axis.X * (SPEED / 3f), 0, true);
					}
				}
				
				if(this.Y - this.Height > FP.Camera.Y + FP.HalfHeight)
				{
					Kill();
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
			
			if (controller.Pressed(Controller.Button.B))
			{
				(World as GameWorld).AdvanceLevel();
			}
			
			if (controller.Pressed(Controller.Button.Start))
			{
				GameWorld.gameManager.StartGame();
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
					OnMessage(PhysicsBody.IMPULSE, (int) FP.Rand(10) - 5, JumpForce);
					e.OnMessage(PhysicsBody.IMPULSE, (int) FP.Rand(10) - 5, -JumpForce);
				}
			}
			
			return base.MoveCollideY(e);
		}
		
		public void SetTint(int id)
		{
			switch (id)
			{
				case 0:
					player.Color = FP.Color(0xFF8888);
					break;
				case 1:
					player.Color = FP.Color(0x88FF88);
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
		
		public void Kill()
		{
			if (IsAlive && !GameWorld.gameManager.GameEnding)
			{
				IsAlive = false;
				World.BroadcastMessage("player_die", this);
				World.BroadcastMessage(GameManager.SHAKE, 20.0f, 1.0f);
				World.Remove(this);
				
				if (Lives > 0)
				{
					Lives -= 1;
				}
				
				if (Lives <= 0)
				{
					World.BroadcastMessage("player_lose", this);
				}
			}
		}
		
		public void DisablePhysics()
		{
			RemoveLogic(physics);
		}
		
		public void SetGlovesLayer(int layer)
		{
			left.Layer = layer;
			right.Layer = layer;
		}
	}
}
