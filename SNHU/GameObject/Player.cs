using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using SNHU.Components;
using SNHU.GameObject.Platforms;
using SNHU.GameObject.Upgrades;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of Player.
	/// </summary>
	public class Player : Entity
	{
		public const string Die = "player_lose";
		public const string Lose = "player_die";
		public const string OnLand = "player_onLand";
		public const string Damage = "player_damage";
		
		public const string Collision = "player";
		
		public const float JumpForce = -15;
		public const float JUMP_JUICE_FORCE = 0.3f;
		public const float JUMP_JUICE_DURATION = 0.17f;
		
		public Image player;
		private bool hand;
		public Fist left, right;
		private Image upgradeIcon;
		
		public Controller Controller { get; private set; }
		private Axis axis;
		
		private OffscreenCursor cursor;
		private bool isOffscreen;
		
		public Upgrade upgrade { get; private set; }
		public bool Invincible;
		public bool Rebounding;
		
		public PhysicsBody physics;
		public bool OnGround {get; private set; }
		
		public const float SPEED = 5.5f;
		
		public const int STARTING_LIVES = 5;
		
		public int Lives { get; private set; }
		public int PlayerId { get; private set; }
		public uint ControllerId { get; private set; }
		public bool IsAlive { get; private set; }
		
		public string ImageName { get; private set; }
		
		public Player(float x, float y, uint jid, int id, string imageName) : base(x, y)
		{
			ImageName = imageName;
			this.PlayerId = id;
			this.ControllerId = jid;
			
			hand = false;
			InitController();
				
			player = new Image(Library.GetTexture("assets/players/" + imageName + ".png"));
			player.Scale = 0.5f;
			AddGraphic(player);
			
			upgradeIcon = Image.CreateCircle(10, FP.Color(0xFF00FF));
			upgradeIcon.Y = Y - 80;
			upgradeIcon.CenterOrigin();
			
			cursor = new OffscreenCursor(this);
			isOffscreen = false;
			
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
			
			Lives = STARTING_LIVES;
			IsAlive = false;
			
			Invincible = false;
			Rebounding = false;
			
			
			AddLogic(physics);
			AddLogic(new Movement(physics, axis));
			AddLogic(new DodgeController(Controller));
			
			AddResponse(GroundSmash.GROUND_SMASH, OnGroundSmash);
			AddResponse(FUS.BE_FUS, OnFUS);
			AddResponse(Magnet.BE_MAGNET, OnMagnet);
			AddResponse(Damage, OnDamage);
		}
		
		void InitController()
		{
			Controller = new Controller(ControllerId);
			
			if (Joystick.HasAxis(ControllerId, Joystick.Axis.PovX))	//	xbox
			{
				Controller.Define("dodge", PlayerId, Controller.Button.B);
				Controller.Define("jump", PlayerId, Controller.Button.A);
				
				Controller.Define("punch", PlayerId, Controller.Button.X);
				Controller.Define("punch_r", PlayerId, (Controller.Button) 5);
				Controller.Define("punch_l", PlayerId, (Controller.Button) 4);
				
				Controller.Define("upgrade", PlayerId, Controller.Button.Y);
				Controller.Define("start", PlayerId, Controller.Button.Start);
			}
			else	//	snes
			{
				//	todo: dodge
				Controller.Define("jump", PlayerId, Controller.Button.X);
				
				Controller.Define("punch", PlayerId, Controller.Button.Y);
				Controller.Define("punch_r", PlayerId, (Controller.Button) 5);
				Controller.Define("punch_l", PlayerId, (Controller.Button) 4);
				
				Controller.Define("upgrade", PlayerId, Controller.Button.A);
				Controller.Define("start", PlayerId, (Controller.Button) 9);
			}
			
			axis = Controller.LeftStick;
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
			
			OnMessage(PhysicsBody.CANCEL);
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
		 	
			if (!isOffscreen && !GameWorld.OnCamera(X, Y))
			{
				isOffscreen = true;
				World.Add(cursor);
			}
			if (isOffscreen && GameWorld.OnCamera(X, Y))
			{	
				isOffscreen = false;
				World.Remove(cursor);
			}
				
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
				
				if(this.Y - this.Height > FP.Camera.Y + FP.HalfHeight)
				{
					Kill();
				}
			}
		}
		
		private void HandleInput()
		{
			if (OnGround && (Controller.Pressed("jump")))
			{
				float jumpMult = 1;
				
				if(Collide("JumpPad", X, Y + 1) != null)
				{
					jumpMult = 1.4f;
					Mixer.Audio["jumpPad"].Play();
				}
				else
				{
					Mixer.Audio[FP.Choose("jump1", "jump2", "jump3")].Play();
				}
				
				OnMessage(PhysicsBody.IMPULSE, 0, JumpForce * jumpMult);
				
				ClearTweens();
				
				player.ScaleX = 1 - JUMP_JUICE_FORCE;
				player.ScaleY = 1 + JUMP_JUICE_FORCE;
				
				var tween = new MultiVarTween(null, ONESHOT);
				tween.Tween(player, new { ScaleX = 1, ScaleY = 1}, JUMP_JUICE_DURATION);
				AddTween(tween, true);
			}
			
			if (Controller.Pressed("upgrade"))
			{
				if (upgrade != null)
				{
					upgrade.Use();
					World.BroadcastMessage(Upgrade.Used, PlayerId);
				}
			}
			
			if (Controller.Pressed("punch"))
			{
				Punch(hand = !hand);
			}
			
			if (Controller.Pressed("punch_r"))
			{
				Punch(false);
			}
			
			if (Controller.Pressed("punch_l"))
			{
				Punch(true);
			}
			
			if (Controller.Pressed("start"))
			{
				GameWorld.gameManager.StartGame();
			}
		}
		
		private void Punch(bool hand)
		{
			if (hand)
			{
				left.Punch();
			}
			else
			{
				right.Punch();
			}
			
			Mixer.Audio[FP.Choose("swing1","swing2")].Play();
			
//			hand = !hand;
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
					OnMessage(PhysicsBody.IMPULSE, FP.Rand(10) - 5, JumpForce);
					e.OnMessage(PhysicsBody.IMPULSE, FP.Rand(10) - 5, -JumpForce);
				}
				else if (e.Y < Y && Math.Abs(X - e.X) < HalfWidth)
				{
					e.OnMessage(PhysicsBody.IMPULSE, FP.Rand(10) - 5, JumpForce * 1.1);
				}
			}
			
			return base.MoveCollideY(e);
		}
		
		private void Kill()
		{
			if (IsAlive && !Invincible && !GameWorld.gameManager.GameEnding)
			{
				if (Lives > 0)
				{
					Lives -= 1;
				}
				
				if (Lives <= 0)
				{
					World.BroadcastMessage(Player.Lose, this);
				}
				
				IsAlive = false;
				World.BroadcastMessage(Player.Die, this);
				World.BroadcastMessage(CameraShake.SHAKE, 20.0f, 1.0f);
				World.Remove(this);
				
				Mixer.Audio["death1"].Play();
				
			}
		}
		
		public void SetGlovesLayer(int layer)
		{
			left.Layer = layer;
			right.Layer = layer;
		}
		
		public void SetUpgrade(Upgrade upgrade)
		{
			if (this.upgrade != null)
			{
				RemoveLogic(this.upgrade);
			}
			
			this.upgrade = upgrade;
			
			if (player != null && left != null && right != null)
			{
				player.Alpha = 1.0f;
				(left.Graphic as Image).Alpha = 1.0f;
				(right.Graphic as Image).Alpha = 1.0f;
			}
			Rebounding = false;
			Invincible = false;
			
			if (this.upgrade != null)
			{
				AddLogic(this.upgrade);
			}
		}
		
		private void OnDamage(params object[] args)
		{
			Kill();
		}
		
		private void OnGroundSmash(params object[] args)
		{
			Player p = args[0] as Player;
			if (p != this)
			{
				if (CollideRect(X,Y,p.X - GroundSmash.SMASH_RADIUS, p.Y - 10, GroundSmash.SMASH_RADIUS * 2, 10))
				{
					if (Rebounding)
					{
						p.Kill();
					}
					else
					{
						Kill();
					}
				}
			}
		}
		
		private void OnFUS(params object[] args)
		{
			var str = (float) args[0];
			var sender = (Player) args[1];
			
			float fromX = sender.X;
			float fromY = sender.Y;
			Vector2f dir = new Vector2f(fromX - X, fromY - Y);
			
			dir = dir.Normalized(str);
	
			if (Rebounding)
			{
				sender.OnMessage(PhysicsBody.IMPULSE, dir.X, dir.Y);
			}
			else if (!Invincible)
			{
				OnMessage(PhysicsBody.IMPULSE, -dir.X, -dir.Y);
			}
		}
		
		private void OnMagnet(params object[] args)
		{
			var str = (float) args[0];
			var sender = (Player) args[1];
			
			float fromX = sender.X;
			float fromY = sender.Y;
			Vector2f dir = new Vector2f(fromX - X, fromY - Y);
			
			dir = dir.Normalized(str);
	
			if (Rebounding)
			{
				sender.OnMessage(PhysicsBody.IMPULSE, -dir.X, -dir.Y);
			}
			else if (!Invincible)
			{
				OnMessage(PhysicsBody.IMPULSE, dir.X, dir.Y);
			}
		}
	}
}
