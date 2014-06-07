using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
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
		public enum Message
		{
			Die,
			Lose,
			OnLand,
			Damage,
			UpgradeAcquired
		}
		
		public const string Collision = "player";
		private HashSet<Entity> excludeCollision;
		
		public const float JumpForce = -15;
		public const float JUMP_JUICE_FORCE = 0.3f;
		public const float JUMP_JUICE_DURATION = 0.17f;
		
		public Image player;
		private bool hand;
		public Fist left, right;
		
		public Controller Controller { get; private set; }
		private Axis axis;
		
		private OffscreenCursor cursor;
		private bool isOffscreen;
		
		public Queue<Upgrade> UpgradeQueue;
		public Upgrade CurrentUpgrade { get; private set; }
		public int UpgradeCapacity { get; private set; }
		
                public bool Invincible { get; private set; }
		public bool Rebounding { get; private set; }
		public bool Guarding { get; private set; }
		public int Facing { get { return player.FlippedX ? -1 : 1; } }
		
		public PhysicsBody physics;
		public DodgeController dodge;
		public Movement movement;
		public bool OnGround { get; private set; }
		
		public const float SPEED = 5.5f;
		private const float GUARD_SPEED_MULT = 0.6f;
		
		public int Health;
		public int Lives { get; private set; }
		public int PlayerId { get; private set; }
		public uint ControllerId { get; private set; }
		public bool IsAlive { get; private set; }
		
		public string ImageName { get; private set; }
		
		public Player(float x, float y, uint jid, int id, string imageName) : base(x, y)
		{
			excludeCollision = new HashSet<Entity>();
			ImageName = imageName;
			this.PlayerId = id;
			this.ControllerId = jid;
			
			hand = false;
			InitController();
				
			var tex = Library.GetTexture("assets/players/" + imageName + ".png");
			tex.Smooth = true;
			player = new Image(tex);
			player.Scale = 0.5f;
			AddComponent(player);
			
			cursor = new OffscreenCursor(this);
			isOffscreen = false;
			
			player.CenterOO();
			SetHitbox(30, 60, 15, 60);
			
			player.OriginY = player.Height;
			
			left = new Fist(true, this);
			right = new Fist(false, this);
			
			Type = Collision;
			
			Lives = GameWorld.gameManager.StartingLives;
			Health = GameWorld.gameManager.StartingHealth;
			IsAlive = false;
			
			Invincible = false;
			Rebounding = false;
			
			UpgradeQueue = new Queue<Upgrade>();
			UpgradeCapacity = 1;
			
			AddComponent(physics = new PhysicsBody(Platform.Collision, Type));
			AddComponent(movement = new Movement(physics, axis));
			AddComponent(dodge = new DodgeController(Controller, axis));
			
			AddResponse(Message.Damage, OnDamage);
			AddResponse(Fist.Message.PunchConnected, OnPunchConnected);
			AddResponse(EffectMessage.Message.OnEffect, OnEffect);
			AddResponse(Shield.Message.Set, SetShield);
			AddResponse(Rebound.Message.Set, SetRebound);
		}
		
		void InitController()
		{
			Controller = new Controller(ControllerId);
			
			if (Joystick.HasAxis(ControllerId, Joystick.Axis.PovX))	//	xbox
			{
				Controller.Define("jump", PlayerId, Controller.Button.A);
				
				Controller.Define("punch", PlayerId, Controller.Button.X);
				Controller.Define("dodge", PlayerId, Controller.Button.RB);
				Controller.Define("upgrade", PlayerId, Controller.Button.Y);
				Controller.Define("guard", PlayerId, Controller.Button.LB);
				
				Controller.Define("start", PlayerId, Controller.Button.Start);
			}
			else	//	snes
			{
				Controller.Define("jump", PlayerId, Controller.Button.X);
				
				Controller.Define("punch", PlayerId, Controller.Button.Y);
				Controller.Define("dodge", PlayerId, (Controller.Button) 5);
				Controller.Define("upgrade", PlayerId, Controller.Button.A);
				Controller.Define("guard", PlayerId, (Controller.Button) 4);
				
				Controller.Define("start", PlayerId, (Controller.Button) 9);
			}
			
			axis = Controller.LeftStick;
		}
		
		public override void Added()
		{
			base.Added();
			
			OnMessage(DodgeController.Message.CancelDodge);
			World.AddList(left, right);
			IsAlive = true;
			
			UpgradeCapacity = int.Parse(GameWorld.gameManager.Config["Player", "UpgradeCapacity"]);
		}
		
		public override void Removed()
		{
			base.Removed();
			World.RemoveList(left, right);
			
			OnMessage(PhysicsBody.Message.Cancel);
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
			
			if (excludeCollision.Count > 0)
			{
				var toRemove = new List<Entity>();
				
				foreach (var player in excludeCollision)
				{
					if (CollideWith(player, X, Y) == null)
						toRemove.Add(player);
				}
				
				foreach (var player in toRemove)
				{
					excludeCollision.Remove(player);
				}
			}
			
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
				if (!Guarding)
				{
				 	if (axis.X < 0)
				 	{
				 		FaceLeft();
				 	}
				 	else if (axis.X > 0)
				 	{
				 		FaceRight();
				 	}
				}
				
				if (Collide(Platform.Collision, X, Y + 1) == null)
				{
					OnGround = false;
				}
				else
				{
					OnMessage(PhysicsBody.Message.Friction, 0.75f);
				}
				
				
				if (!OnGround && dodge.IsDodging)
				{
					FP.Log("whoa");
					Entity result = 
						Collide(Platform.Collision, X + 1, Y) ??
						Collide(Platform.Collision, X - 1, Y) ??
						null;
					
					if (result != null)
					{
						dodge.CanDodge = true;
					}
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
			var newGuard = Controller.Check("guard");
			if (newGuard != Guarding)
			{
				if (!IsPunching())
			    {
					if (newGuard)
						physics.OnMessage(PhysicsBody.Message.ImpulseMult, 0.3);
					else
						physics.OnMessage(PhysicsBody.Message.ImpulseMult, 1);
					
					left.SetGuarding(newGuard);
					right.SetGuarding(newGuard);
					
					Guarding = newGuard;
				}
			}
			
			if (Controller.Pressed("jump"))
			{
				OnMessage(DodgeController.Message.CancelDodge);
				
				if (OnGround)
				{
					float jumpMult = 1;
					
					if(Collide("JumpPad", X, Y + 1) != null)
					{
						jumpMult = 1.4f;
						Mixer.JumpPad.Play();
					}
					else
					{
						FP.Choose(Mixer.Jump1, Mixer.Jump2, Mixer.Jump3).Play();
					}
					
					OnMessage(PhysicsBody.Message.Impulse, 0, JumpForce * jumpMult, true);
					
					Tweener.Cancel();
					
					player.ScaleX = 1 - JUMP_JUICE_FORCE;
					player.ScaleY = 1 + JUMP_JUICE_FORCE;
					
					Tweener.Tween(player, new { ScaleX = 1, ScaleY = 1}, JUMP_JUICE_DURATION);
				}
			}
			
			if (Controller.Pressed("upgrade"))
			{
				if (CurrentUpgrade != null)
				{
					CurrentUpgrade.Use();
				}
			}
			
			if (!Guarding)
			{
				if (Controller.Pressed("punch"))
				{
					Punch(hand = !hand);
				}
			}
			
			if (Controller.Pressed("start"))
			{
				GameWorld.gameManager.StartGame();
			}
		}
		
		bool IsPunching()
		{
			return left.Punching || right.Punching;
		}
		
		private void Punch(bool hand)
		{
			bool success = false;
			if (hand)
			{
				success = left.Punch(axis.X, axis.Y);
			}
			else
			{
				success = right.Punch(axis.X, axis.Y);
			}
			
			if (success)
			{
				FP.Choose(Mixer.Swing1, Mixer.Swing2).Play();
			}
		}
		
		public override bool MoveCollideY(Entity e)
		{
			if (e.Type == Platform.Collision)
			{
				if (!OnGround)
				{
					player.ScaleX = 1 + JUMP_JUICE_FORCE;
					player.ScaleY = 1 - JUMP_JUICE_FORCE;
					
					Tweener.Tween(player, new { ScaleX = 1, ScaleY = 1}, JUMP_JUICE_DURATION);
					
					OnGround = true;
					
					Mixer.Land1.Play();
					
					if (e.Y >= Y)
					{
						e.OnMessage(Platform.Message.ObjectCollide, this);
					}
					else
					{
						OnMessage(PhysicsBody.Message.Impulse, 0, 1, true);
					}
				}
				
				OnMessage(Message.OnLand);
			}
			else if (e.Type == Type)
			{
				if (e.Top >= Bottom)
				{
					OnMessage(PhysicsBody.Message.Impulse, FP.Rand(10) - 5, JumpForce);
					e.OnMessage(PhysicsBody.Message.Impulse, FP.Rand(10) - 5, -JumpForce);
				}
				else if (e.Bottom <= Top && Math.Abs(X - e.X) < HalfWidth)
				{
					e.OnMessage(PhysicsBody.Message.Impulse, FP.Rand(10) - 5, JumpForce * 1.1);
				}
				else return false;
			}
			
			return base.MoveCollideY(e);
		}
		
		public override bool MoveCollideX(Entity e)
		{
			if (e.Type == Type)
			{
				var p = e as Player;
				
				if (p.Invincible)
					return true;
				
				if (dodge.IsDodging)
					return false;
				
				if (excludeCollision.Contains(e) || p.excludeCollision.Contains(this))
					return false;
				
				if (Invincible)
				{
					p.MoveBy(axis.X * movement.Speed, 0, p.physics.Colliders);
					return true;
				}
				
				return false;
			}
			
			return base.MoveCollideX(e);
		}
		
		private void Kill()
		{
			if (IsAlive && !GameWorld.gameManager.GameEnding)
			{
				if (Lives > 0)
				{
					Lives -= 1;
				}
				
				Health = GameWorld.gameManager.StartingHealth;
				
				if (Lives <= 0)
				{
					World.BroadcastMessage(Player.Message.Lose, this);
				}
				
				IsAlive = false;
				World.BroadcastMessage(Player.Message.Die, this);
				World.BroadcastMessage(CameraShake.Message.Shake, 20.0f, 1.0f);
				World.Remove(this);
				
				Mixer.Death1.Play();
			}
		}
		
		public void SetGlovesLayer(int layer)
		{
			left.Layer = layer;
			right.Layer = layer;
		}
		
		public void SetUpgrade(Upgrade upgrade)
		{
			if (World == null)
			{
				FP.Tweener.Timer(0.01f).OnComplete(() => SetUpgrade(upgrade));
				return;
			}
			
			if (this.CurrentUpgrade != null)
			{
				World.BroadcastMessage(Upgrade.Message.Used, PlayerId);
				RemoveComponent(this.CurrentUpgrade);
			}
			
			this.CurrentUpgrade = upgrade;
			
			if (player != null && left != null && right != null)
			{
				player.Alpha = 1.0f;
				left.Image.Alpha = 1.0f;
				right.Image.Alpha = 1.0f;
			}
			
			Rebounding = false;
			Invincible = false;
			
			if (this.CurrentUpgrade != null)
			{
				AddComponent(this.CurrentUpgrade);
				World.BroadcastMessage(Player.Message.UpgradeAcquired, this, this.CurrentUpgrade);
			}
		}
		
		private void OnDamage(params object[] args)
		{
			Kill();
		}
		
		private void OnPunchConnected(params object[] args)
		{
			if (GameWorld.gameManager.StartingHealth == 0)
				return;
			
			Health -= GameWorld.gameManager.PunchDamage;
			World.BroadcastMessage(HUD.Message.UpdateDamage, this);
			if (Health <= 0)
				Kill();
		}
		
		private void OnEffect(params object[] args)
		{
			if (Invincible) return;
			
			var effect = (EffectMessage) args[0];
			
			var from = Rebounding ? this : effect.Sender;
			var to = Rebounding ? effect.Sender : this;
			var scalar = Rebounding ? 1.3f : 1;
			effect.Apply(from, to, scalar);
		}
		
		private void SetRebound(params object[] args)
		{
			Rebounding = (bool) args[0];
		}
		
		private void SetShield(params object[] args)
		{
			Invincible = (bool) args[0];
			if (Invincible)
			{
				var list = new List<Entity>();
				CollideInto(Type, X, Y, list);
				
				foreach (var e in list)
				{
					var player = e as Player;
					excludeCollision.Add(player);
					player.excludeCollision.Add(this);
				}
			}
		}
	}
}
