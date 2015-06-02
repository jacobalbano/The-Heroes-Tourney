﻿using System;
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
		public Fist left, right;
		
		private Directional Direction;
		
		private OffscreenCursor cursor;
		
		public Queue<Upgrade> UpgradeQueue;
		public Upgrade CurrentUpgrade { get; private set; }
		public int UpgradeCapacity { get; private set; }
		
		public bool Invincible { get; private set; }
		public bool Rebounding { get; private set; }
		public bool Guarding { get; private set; }
		public int Facing { get { return player.FlippedX ? -1 : 1; } }
		
		public Input Jump, Attack, Dodge, ActivateUpgrade, Guard, Start;
		
		private StateMachine inputState;
		private int Standing, Jumping, Falling;
		
		private PhysicsBody Physics;
		public DodgeController DodgeController;
		public Movement Movement;
		public bool OnGround { get; private set; }
		
		public const float SPEED = 5.5f;
		private const float GUARD_SPEED_MULT = 0.6f;
		
		public int Health, PunchDamage, Lives;
		
		public int PlayerId { get; private set; }
		public int ControllerId { get; private set; }
		public bool IsAlive { get; private set; }
		
		public string ImageName { get; private set; }
		
		public Player(int slot, int playerId, string imageName)
		{
			excludeCollision = new HashSet<Entity>();
			ImageName = imageName;
			PlayerId = playerId;
			ControllerId = slot;
			
			Layer = ObjectLayers.Players;
			
			hand = false;
			InitController();
				
			var tex = Library.GetTexture("players/" + imageName + ".png");
			tex.Smooth = true;
			player = new Image(tex);
			player.Scale = 0.5f;
			AddComponent(player);
			
			cursor = new OffscreenCursor(this);
			
			player.CenterOO();
			SetHitbox(30, 60, 15, 60);
			
			player.OriginY = player.Height;
			
			left = new Fist(true, this);
			right = new Fist(false, this);
			
			Type = Collision;
			IsAlive = false;
			
			Invincible = false;
			Rebounding = false;
			
			UpgradeQueue = new Queue<Upgrade>();
			UpgradeCapacity = 1;
			
			Physics = AddComponent(new PhysicsBody(Platform.Collision, Type));
			Movement = AddComponent(new Movement(Physics, Direction));
			DodgeController = AddComponent(new DodgeController(Dodge, Direction));
			inputState = AddComponent(new StateMachine());
			
			AddResponse(Message.Damage, OnDamage);
			AddResponse(Fist.Message.PunchConnected, OnPunchConnected);
			AddResponse(EffectMessage.Message.OnEffect, OnEffect);
			AddResponse(Shield.Message.Set, SetShield);
			AddResponse(Rebound.Message.Set, SetRebound);
			AddResponse(Fist.Message.PunchSuccess, OnPunchSuccess);
			
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
			
			if (cursor.World == null)
				World.Add(cursor);
			
			OnMessage(DodgeController.Message.CancelDodge);
			World.AddList(left, right);
			IsAlive = true;
			
			UpgradeCapacity = Library.GetConfig<PlayerConfig>("config/player.ini").UpgradeCapacity;
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
				
				foreach (var p in excludeCollision)
				{
					if (!CollideWith(p, X, Y))
						toRemove.Add(p);
				}
				
				foreach (var p in toRemove)
				{
					excludeCollision.Remove(p);
				}
			}
			
			if (!Guarding)
			{
			 	if (Direction.X < 0)
			 	{
			 		FaceLeft();
			 	}
			 	else if (Direction.X > 0)
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
			
			if (!OnGround && DodgeController.IsDodging)
			{
				Entity result = 
					Collide(Platform.Collision, X + 1, Y) ??
					Collide(Platform.Collision, X - 1, Y);
				
				if (result != null)
				{
				DodgeController.CanDodge = true;
				}
			}
			
			HandleInput();
			
			if(Top > FP.Camera.Y + FP.HalfHeight)
			{
				Kill();
			}
		}
		
		private void HandleInput()
		{
			var newGuard = Guard.Down;
			if (newGuard != Guarding)
			{
				if (!IsPunching())
			    {
					if (newGuard)
						Physics.OnMessage(PhysicsBody.Message.ImpulseMult, 0.3);
					else
						Physics.OnMessage(PhysicsBody.Message.ImpulseMult, 1);
					
					left.SetGuarding(newGuard);
					right.SetGuarding(newGuard);
					
					Guarding = newGuard;
				}
			}
			
			if (Jump.Pressed)
			{
				OnMessage(DodgeController.Message.CancelDodge);
				
				if (OnGround)
				{
					float jumpMult = 1;
					
					if(Collide(JumpPad.Collision, X, Y + 1) != null)
					{
						jumpMult = 1.4f;
						Mixer.JumpPad.Play();
					}
					else
					{
						FP.Choose.Option(Mixer.Jump1, Mixer.Jump2, Mixer.Jump3).Play();
					}
					
					OnMessage(PhysicsBody.Message.Impulse, 0, JumpForce * jumpMult, true);
					
					Tweener.Cancel();
					
					player.ScaleX = 1 - JUMP_JUICE_FORCE;
					player.ScaleY = 1 + JUMP_JUICE_FORCE;
					
					Tweener.Tween(player, new { ScaleX = 1, ScaleY = 1}, JUMP_JUICE_DURATION);
				}
			}
			
			if (ActivateUpgrade.Pressed)
			{
				if (CurrentUpgrade != null)
				{
					CurrentUpgrade.Use();
				}
			}
			
			if (!Guarding)
			{
				if (Attack.Pressed)
				{
					Punch(hand = !hand);
				}
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
				success = left.Punch(Direction.X, Direction.Y);
			}
			else
			{
				success = right.Punch(Direction.X, Direction.Y);
			}
			
			if (success)
			{
				FP.Choose.Option(Mixer.Swing1, Mixer.Swing2).Play();
			}
		}
		
		public override bool MoveCollideY(Entity e)
		{
			if (e.Type == Platform.Collision)
			{
				if (!OnGround)
				{
					OnGround = true;
					
					player.ScaleX = 1 + JUMP_JUICE_FORCE;
					player.ScaleY = 1 - JUMP_JUICE_FORCE;
					
					Tweener.Tween(player, new { ScaleX = 1, ScaleY = 1}, JUMP_JUICE_DURATION);
					
					Mixer.Land1.Play();
					
					if (e.Y >= Y)
					{
						//	send OnLand only if we're above the object.
						e.OnMessage(Message.OnLand, this);
					}
					else
					{
						OnMessage(PhysicsBody.Message.Impulse, 0, 1, true);
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
				
				if (DodgeController.IsDodging)
					return false;
				
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
			if (PunchDamage == 0)
				return;
			
			Health -= PunchDamage;
			World.BroadcastMessage(HUD.Message.UpdateDamage, this);
			if (Health <= 0)
				Kill();
		}
		
		private void OnPunchSuccess(params object[] args)
		{
			if (GetComponent<HitFreeze>() == null)
				AddComponent(new HitFreeze(X, Y));
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
