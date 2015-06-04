using System;
using System.Collections.Generic;
using Indigo;
using Indigo.Components;
using Indigo.Graphics;
using Indigo.Inputs;
using Indigo.Inputs.Gamepads;
using Indigo.Utils;
using SFML.Window;
using SNHU.Components;
using SNHU.Config;
using SNHU.GameObject.Platforms;
using SNHU.GameObject.Upgrades;
using SNHU.Systems;

namespace SNHU.GameObject
{
	public class Player : Entity
	{
		public enum Message
		{
			Die,
			OnLand,
			Damage,
			Hit,
			UpgradeAcquired
		}
		
		public const string Collision = "player";
		private HashSet<Entity> excludeCollision;
		
		public const float JumpForce = -15;
		public const float JUMP_JUICE_FORCE = 0.3f;
		public const float JUMP_JUICE_DURATION = 0.17f;
		
		public Image player;
		private bool hand;
		public Fists Fists;
		
		private Directional Direction;
		
		public Queue<Upgrade> UpgradeQueue;
		public Upgrade CurrentUpgrade { get; private set; }
		public int UpgradeCapacity { get; private set; }
		
		public bool Invincible { get; private set; }
		public bool Rebounding { get; private set; }
		public bool Guarding { get; private set; }
		
		public int Facing { get { return player.FlippedX ? -1 : 1; } }
		
		public Input Jump, Attack, Dodge, ActivateUpgrade, Guard, Start;
		
		private StateMachine states;
		private int Standing, Jumping, Falling, Dodging;
		
		private PhysicsBody Physics;
		public DodgeController DodgeController;
		public Movement Movement;
		
		public const float SPEED = 5.5f;
		private const float GUARD_SPEED_MULT = 0.6f;
		
		public int Health, PunchDamage, Lives;
		private int JumpsRemaining;
		
		public int PlayerId { get; private set; }
		public int ControllerId { get; private set; }
		public bool IsAlive { get; private set; }
		
		public string ImageName { get; private set; }
		
		public Player(int slot, int playerId, string imageName)
		{
			ControllerId = slot;
			PlayerId = playerId;
			ImageName = imageName;
			
			Layer = ObjectLayers.Players;
			excludeCollision = new HashSet<Entity>();
			UpgradeQueue = new Queue<Upgrade>();
			UpgradeCapacity = 1;
			
			InitController();
			
			SetHitbox(30, 60, 15, 60);
			
			Type = Collision;
			
			IsAlive = false;
			Invincible = false;
			Rebounding = false;
			
			Physics = AddComponent(new PhysicsBody(Platform.Collision, Type));
			Movement = AddComponent(new Movement(Physics, Direction));
			DodgeController = AddComponent(new DodgeController(Dodge, Direction));
			
			player = AddComponent(new Image(Library.GetTexture("players/" + imageName + ".png")));
			player.Source.Smooth = true;
			player.Scale = 0.5f;
			player.OriginX = player.Width / 2;
			player.OriginY = player.Height;
			
			AddComponent(new OffscreenCursor(ImageName));
			Fists = AddComponent(new Fists());
			
			states = AddComponent(new StateMachine());
			Standing = states.AddState(BeginStand, WhileStanding, null);
			Jumping = states.AddState(BeginJump, WhileJumping, null);
			Falling = states.AddState(BeginFalling, WhileFalling, null);
			Dodging = states.AddState(WhileDodging);
			
			AddResponse(Message.Damage, OnDamage);
			AddResponse(Fist.Message.PunchConnected, OnPunchConnected);
			AddResponse(EffectMessage.Message.OnEffect, OnEffect);
			AddResponse(Shield.Message.Set, SetShield);
			AddResponse(Rebound.Message.Set, SetRebound);
		}
		
		void BeginStand()
		{
			player.ScaleX = 1 + JUMP_JUICE_FORCE;
			player.ScaleY = 1 - JUMP_JUICE_FORCE;
			
			Tweener.Tween(player, new { ScaleX = 1, ScaleY = 1}, JUMP_JUICE_DURATION);
			Mixer.Land1.Play();
			
			JumpsRemaining = 2;
		}
		
		void WhileStanding()
		{
			if (!Guarding)
				UpdateFacing();
			
			if (Jump.Pressed)
				states.ChangeState(Jumping);
			
			OnMessage(PhysicsBody.Message.Friction, 0.75f);
			
			if (Collide(Platform.Collision, X, Y + 1) == null)
				states.ChangeState(Falling);
		}
		
		void BeginJump()
		{
			Tweener.Cancel();
			
			OnMessage(PhysicsBody.Message.Impulse, 0, JumpForce, true);
			
			if (JumpsRemaining == 2)
			{
				Mixer.Jump.Play();
			}
			else
			{
				Mixer.DoubleJump.Play();
				
				for (int i = 0; i < 5; i++)
				{
					var randX = FP.Random.Int(-12, 12);
					var randY = FP.Random.Int(3);
					World.BroadcastMessage(GlobalEmitter.Message.DoubleJump, "dust", X + randX, Y + randY - 15);
				}
			}
			
			Tweener.Tween(player, new { ScaleX = 1, ScaleY = 1}, JUMP_JUICE_DURATION)
				.From(new { ScaleX = 1 - JUMP_JUICE_FORCE, ScaleY = 1 + JUMP_JUICE_FORCE});
		}
		
		void WhileJumping()
		{
			UpdateFacing();
			
			var delta = Physics.MoveDelta.Y;
			if (delta >= -5)
				states.ChangeState(Falling);
		}
		
		void BeginFalling()
		{
			JumpsRemaining--;
		}
		
		void WhileFalling()
		{
			UpdateFacing();
			if (Jump.Pressed && JumpsRemaining > 0)
				states.ChangeState(Jumping);
		}
		
		void WhileDodging()
		{
		}
		
		private void UpdateFacing()
		{
			if (Direction.X < 0)
		 	{
				player.FlippedX = true;
				Fists.FaceLeft();
		 	}
		 	else if (Direction.X > 0)
		 	{
				player.FlippedX = false;
				Fists.FaceRight();
		 	}
		}
		
		void InitController()
		{
			var slot = GamepadManager.GetSlot(ControllerId);
			if (!slot.IsConnected) return;
			
			if (SnesController.IsMatch(slot))
			{
				var snes = new SnesController(slot);
				Jump = snes.B;
				Attack = snes.Y;
				Dodge = snes.R;
				ActivateUpgrade = snes.X;
				Guard = snes.L;
				Start = snes.Start;
				
				Direction = snes.Dpad;
			}
			else if (Xbox360Controller.IsMatch(slot))
			{
				var xbox = new Xbox360Controller(slot);
				
				Jump = xbox.A;
				Attack = xbox.X;
				Dodge = xbox.RB;
				ActivateUpgrade = xbox.Y;
				Guard = xbox.LB;
				Start = xbox.Start;
				
				Direction = xbox.LeftStick;
			}
			else
			{
				throw new Exception("invalid controller woh");
			}
			
		}
		
		public override void Added()
		{
			base.Added();
			
			states.ChangeState(Falling);
			IsAlive = true;
			
			UpgradeCapacity = Library.GetConfig<PlayerConfig>("config/player.ini").UpgradeCapacity;
		}
		
		public override void Removed()
		{
			base.Removed();
			
			OnMessage(PhysicsBody.Message.Cancel);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (excludeCollision.Count > 0)
			{
				var toRemove = new List<Entity>();
				
				foreach (var p in excludeCollision)
					if (!CollideWith(p, X, Y))
						toRemove.Add(p);
				
				foreach (var p in toRemove)
					excludeCollision.Remove(p);
			}
			
			HandleInput();
			
			if(Top > FP.Camera.Y + FP.HalfHeight)
				Kill();
		}
		
		private void HandleInput()
		{
			var newGuard = Guard.Down;
			if (newGuard != Guarding)
			{
				if (!Fists.Punching)
			    {
					if (newGuard)
						Physics.OnMessage(PhysicsBody.Message.ImpulseMult, 0.3);
					else
						Physics.OnMessage(PhysicsBody.Message.ImpulseMult, 1);
					
					Guarding = Fists.Guarding = newGuard;
				}
			}
			
			if (ActivateUpgrade.Pressed)
			{
				if (CurrentUpgrade != null)
				{
					CurrentUpgrade.Use();
				}
			}
			
			if (!Guarding && Attack.Pressed)
			{
				if (Fists.Punch(Direction.X, Direction.Y))
					Mixer.Swing.Play();
			}
		}
		
		public override bool MoveCollideY(Entity e)
		{
			if (e.Type == Platform.Collision)
			{
				if (states.CurrentState != Standing)
				{	
					if (Physics.MoveDelta.Y > 0)
					{
						//	send OnLand only if we're above the object.
						states.ChangeState(Standing);
						e.OnMessage(Message.OnLand, this);
					}
					else
					{
						JumpsRemaining--;
						OnMessage(PhysicsBody.Message.Impulse, 0, 1, true);
						states.ChangeState(Falling);
					}
				}
				
				e.OnMessage(Platform.Message.Bump);
				OnMessage(Message.OnLand, this);
			}
			else if (e.Type == Type)
			{
				if (e.Top >= Bottom)
				{
					OnMessage(PhysicsBody.Message.Impulse, FP.Random.Int(10) - 5, JumpForce);
					e.OnMessage(PhysicsBody.Message.Impulse, FP.Random.Int(10) - 5, -JumpForce);
				}
				else if (e.Bottom <= Top && Math.Abs(X - e.X) < HalfWidth)
				{
					e.OnMessage(PhysicsBody.Message.Impulse, FP.Random.Int(10) - 5, JumpForce * 1.1);
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
				
				if (excludeCollision.Contains(e) || p.excludeCollision.Contains(this))
					return false;
				
				if (Invincible)
				{
					p.MoveBy(Direction.X * Movement.Speed, 0, p.Physics.Colliders);
					return true;
				}
				
				return false;
			}
			else if (e.Type == Platform.Collision)
			{
				e.OnMessage(Platform.Message.Bump);
			}
			
			return base.MoveCollideX(e);
		}
		
		private void Kill()
		{
			if (IsAlive)
			{
				if (Lives > 0)
					Lives -= 1;
				
				IsAlive = false;
				World.BroadcastMessage(Player.Message.Die, this);
				World.BroadcastMessage(CameraManager.Message.Shake, 20.0f, 1.0f);
				World.Remove(this);
				
				Mixer.Death1.Play();
			}
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
			player.Alpha = Fists.Alpha = 1;
			
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
			if (PunchDamage == 0)
				return;
			
			Health -= PunchDamage;
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
