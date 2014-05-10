using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
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
		public const string PUNCH_CONNECTED = "fist_punchConnected";
		public const string PUNCH_SUCCESS = "fist_punchSuccess";
		
		public const float DEFAULT_PUNCH_MULT = 1;
		
		public float ForceMultiplier;
		
		private Image image;
		private Player parent;
		
		private float offsetX, offsetY;
		private bool backHand;
		private bool punchy, punching, canPunch;
		
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
			punching = false;
			canPunch = true;
			
			image = new Image(Library.GetTexture("assets/glove" + (backHand ? "2" : "1") + ".png"));
			image.CenterOO();
			Graphic = image;
			
			SetHitboxTo(image);
			CenterOrigin();
			
			Type = Collision;
			
			if (backHand)
			{
				offsetY = 5;
			}
			else
			{
				offsetY = 15;
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
			
			punching = false;
			canPunch = true;
			image.Angle = 0;
		}
		
		public void FaceLeft()
		{
			image.FlippedX = true;
			
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
			image.FlippedX = false;
			
			if (backHand)
			{
				offsetX = 30;
			}
			else
			{
				offsetX = 0;
			}
		}
		
		public bool Punch(Axis axis)
		{
			if (punching) return false;
			
			forceVector.X = (float) Math.Round(axis.X);
			forceVector.Y = axis.Y;
			
			if (Math.Abs(forceVector.X) < float.Epsilon)
				forceVector.X = image.FlippedX ? -1f : 1f;
						
			var to = new {
				offsetX = offsetX + (backHand ? 40 : 50) * forceVector.X,
				offsetY = offsetY + (backHand ? 30 : 20) * forceVector.Y
			};
			
			forceVector.Y *= 0.6f;
			if (Math.Abs(forceVector.Y) < float.Epsilon)
				forceVector.Y = -0.2f;
			
			punchy = true;
			punching = true;
			
			var tween = new MultiVarTween(() => UnPunch(image.FlippedX), ONESHOT);
			tween.Tween(this, to, 0.05f);
			AddTween(tween, true);
			
			return true;
		}
		
		private void UnPunch(bool facing)
		{
			var to = new {
				offsetX = (backHand ? 30 : 0) * (facing ? -1 : 1),
				offsetY = (backHand ? 5 : 15)
			};
			
			punchy = false;
			
			var tween = new MultiVarTween(() => FinishPunch(facing), ONESHOT);
			tween.Tween(this, to, 0.07f);
			AddTween(tween, true);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (parent.Guarding)
			{				
				return;
			}
			
			if (!canPunch) return;
			if (punchy)
			{
				var l = new List<Entity>();
				CollideInto(Player.Collision, X, Y, l);
				
				foreach (var p in l)
				{
					var player = p as Player;
					if (image.FlippedX)
					{
						if (player.Left > parent.Left) continue;
					}
					else
					{
						if (player.Right < parent.Right) continue;
					}
					
					if (player != parent)
				 	{
						player.OnMessage(EffectMessage.ON_EFFECT, MakeEffect(player));
				 	}
				}
			 	
			}
			
			X = parent.CenterX + offsetX;
			Y = parent.CenterY + offsetY;
		}
		
		EffectMessage MakeEffect(Player player)
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				canPunch = false;
				
				parent.OnMessage(Fist.PUNCH_SUCCESS, player);
				World.BroadcastMessage(CameraShake.SHAKE, 10.0f, 0.5f);
		 		Mixer.Audio["hit1"].Play();
		 		
		 		if (FP.Sign(from.X - to.X) == FP.Sign(forceVector.X))
		 			forceVector *= -1;
		 		
		 		var force = ForceMultiplier * BASE_PUNCH_FORCE * scalar;
		 		
		 		to.OnMessage(PUNCH_CONNECTED);
	 			to.OnMessage(PhysicsBody.IMPULSE, force * forceVector.X, force * forceVector.Y);
			};
			
			return new EffectMessage(parent, callback);
		}
	}
}
