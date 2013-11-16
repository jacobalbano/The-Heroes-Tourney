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
		
		private Image player;
		private Image glove1, glove2;
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
			
			player = new Image(Library.GetTexture("assets/player.png"));
			glove1 = new Image(Library.GetTexture("assets/glove1.png"));
			glove2 = new Image(Library.GetTexture("assets/glove2.png"));
			
			glove1.OriginX = 20;
			glove1.OriginY = 40;
			
			glove2.OriginX = 40;
			glove2.OriginY = 50;
			
			AddGraphic(glove2);
			AddGraphic(player);
			AddGraphic(glove1);
			
			player.Color = FP.Color(FP.Rand(uint.MaxValue));
			
			player.CenterOO();
			SetHitboxTo(player);
			CenterOrigin();
			
			OriginY = Height;
			player.OriginY = Height;
			
			
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
			
			if (axis.X < 0)
			{
				player.FlippedX = true;
				glove1.FlippedX = true;
				glove2.FlippedX = true;
				
				glove1.X = 0;
				glove2.X = 0;
			}
			else if (axis.X > 0)
			{
				player.FlippedX = false;
				glove1.FlippedX = false;
				glove2.FlippedX = false;
				
				glove1.X = 5;
				glove2.X = 50;
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
			
			if (controller.Pressed(Controller.Button.X))
			{
				var tween = new VarTween(() =>
                {
					var back = new VarTween(null, ONESHOT);
					back.Tween(glove1, "X", 0, 0.05f);
					AddTween(back, true);
                }, ONESHOT);
				tween.Tween(glove1, "X", -50, 0.05f);
				AddTween(tween, true);
			}
			
		}
		
		public override bool MoveCollideY(Entity e)
		{
			if (e.Type == Platform.Collision)
			{
				if (!OnGround)
				{
					ClearTweens();
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
