using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Utils;
using SFML.Window;
using SNHU.Components;
using SNHU.GameObject.Upgrades;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of Fist.
	/// </summary>
	public class Fist : Entity
	{
		public enum Message
		{
			PunchConnected,
			PunchSuccess
		}
		
		public const float DEFAULT_PUNCH_MULT = 1;
		
		public float ForceMultiplier;
		
		public Image Image { get; private set; }
		protected Player parent;
		
		
		private const float BackOffsetY = 5;
		private const float FrontOffsetY = 15;
		
		protected float offsetX, offsetY;
		private bool backHand;
		private bool punchy, canPunch;
		public bool Punching { get; private set; }
		
		private Vector2f forceVector;
		
		public const string Collision = "fist";
		
		public const float BASE_PUNCH_FORCE = 30;
		public const float REBOUND_PUNCH_FORCE = 40;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="hand">True for left hand image, false for right.</param>
		public Fist(bool hand, Player parent)
		{
			ForceMultiplier = DEFAULT_PUNCH_MULT;
			forceVector = new Vector2f();
			
			backHand = hand;
			this.parent = parent;
			punchy = false;
			Punching = false;
			canPunch = true;
			
			Image = new Image(Library.GetTexture("assets/glove" + (backHand ? "2" : "1") + ".png"));
			Image.CenterOO();
			AddComponent(Image);
			
			SetHitboxTo(Image);
			CenterOrigin();
			
			Type = Collision;
			
			if (backHand)
			{
				offsetY = BackOffsetY;
			}
			else
			{
				offsetY = FrontOffsetY;
			}
			
			FaceRight();
		}
		
		private void FinishPunch(bool facing)
		{
			if (facing)
			{
				FaceLeft();
			}
			else
			{
				FaceRight();
			}
			
			Punching = false;
			canPunch = true;
			Image.Angle = 0;
		}
		
		public void FaceLeft()
		{
			Image.FlippedX = true;
			
			if (backHand)
			{
				offsetX = -30;
			}
			else
			{
				offsetX = 0;
			}
		}
		
		public void FaceRight()
		{
			Image.FlippedX = false;
			
			if (backHand)
			{
				offsetX = 30;
			}
			else
			{
				offsetX = 0;
			}
		}
		
		public bool Punch(float directionX, float directionY)
		{
			if (Punching) return false;
			
			forceVector.X = (float) Math.Round(directionX);
			forceVector.Y = directionY;
			
			if (Math.Abs(forceVector.X) < float.Epsilon)
				forceVector.X = Image.FlippedX ? -1f : 1f;
						
			var to = new {
				offsetX = offsetX + (backHand ? 40 : 50) * forceVector.X,
				offsetY = offsetY + (backHand ? 30 : 20) * forceVector.Y
			};
			
			forceVector.Y *= 0.6f;
			if (Math.Abs(forceVector.Y) < float.Epsilon)
				forceVector.Y = -0.2f;
			
			punchy = true;
			Punching = true;
			
			Tweener.Tween(this, to, 0.05f)
				.OnComplete(() => UnPunch(Image.FlippedX));
			
			return true;
		}
		
		private void UnPunch(bool facing)
		{
			var to = new {
				offsetX = (backHand ? 30 : 0) * (facing ? -1 : 1),
				offsetY = (backHand ? 5 : 15)
			};
			
			punchy = false;
			
			Tweener.Tween(this, to, 0.07f)
				.OnComplete(() => FinishPunch(facing));
		}
		
		public override void Update()
		{
			base.Update();
			
			if (!canPunch) return;
			if (punchy)
			{
				var l = new List<Entity>();
				CollideInto(Player.Collision, X, Y, l);
				
				foreach (var p in l)
				{
					var player = p as Player;
					if (Image.FlippedX)
					{
						if (player.Left > parent.Left) continue;
					}
					else
					{
						if (player.Right < parent.Right) continue;
					}
					
					if (player != parent)
				 	{
						player.OnMessage(EffectMessage.Message.OnEffect, MakeEffect(player));
				 	}
				}
			 	
			}
			
			X = parent.CenterX + offsetX;
			Y = parent.CenterY + offsetY;
		}
		
		protected EffectMessage MakeEffect(Player player)
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				canPunch = false;
				
				parent.OnMessage(Fist.Message.PunchSuccess, player);
				World.BroadcastMessage(CameraShake.Message.Shake, 10.0f, 0.5f);
		 		Mixer.Hit1.Play();
		 		
		 		if (FP.Sign(from.X - to.X) == FP.Sign(forceVector.X))
		 			forceVector *= -1;
		 		
		 		var force = ForceMultiplier * BASE_PUNCH_FORCE * scalar;
		 		
		 		to.OnMessage(Message.PunchConnected);
	 			to.OnMessage(PhysicsBody.Message.Impulse, force * forceVector.X, force * forceVector.Y);
			};
			
			return new EffectMessage(parent, callback);
		}
		
		public void SetGuarding(bool guarding)
		{
			if (guarding)
			{
				if (backHand) offsetY = BackOffsetY - 15;
				else offsetY = FrontOffsetY - 15;
			}
			else
			{
				if (backHand) offsetY = BackOffsetY;
				else offsetY = FrontOffsetY;
			}
		}
	}
}
