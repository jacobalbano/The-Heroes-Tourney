using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using SNHU.Components;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of Fist.
	/// </summary>
	public class Fist : Entity
	{
		public float Multiplier;
		
		private Image image;
		private Player parent;
		
		private float offsetX, offsetY;
		private bool left;
		private bool punchy, punching, canPunch;
		
		private Vector2f forceVector;
		
		public const string Collision = "fist";
		
		private const float BASE_PUNCH_FORCE = 30;
		private const float REBOUND_PUNCH_FORCE = 40;
		
		private const float PUNCH_BOOST_Y = -10;
		
		public const float DEFAULT_PUNCH_MULT = 1;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="hand">True for left hand image, false for right.</param>
		public Fist(bool hand, Player parent)
		{
			Multiplier = DEFAULT_PUNCH_MULT;
			
			forceVector = new Vector2f();
			
			left = hand;
			this.parent = parent;
			punchy = false;
			punching = false;
			canPunch = true;
			
			image = new Image(Library.GetTexture("assets/glove" + (left ? "2" : "1") + ".png"));
			image.CenterOO();
			Graphic = image;
			
			SetHitboxTo(image);
			CenterOrigin();
			
			Type = Collision;
			
			if (left)
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
			
			if (left)
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
			
			if (left)
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
			if (forceVector.X == 0) forceVector.X = image.FlippedX ? -0.8f : 0.8f;
			
			forceVector.Y = (float) Math.Round(axis.Y);
			
			image.Angle = (image.FlippedX ? -1 : 1) * FP.Angle(0, 0, forceVector.X, forceVector.Y);
			
			var to = new {
				offsetX = (offsetX + (left ? 40 : 50)) * forceVector.X,
				offsetY = (offsetY + (left ? 30 : 20)) * forceVector.Y
			};
			
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
				offsetX = (left ? 40 : 50) * (facing ? -1 : 1),
				offsetY = (left ? 5 : 15)
			};
			
			punchy = false;
			
			var tween = new MultiVarTween(() => FinishPunch(facing), ONESHOT);
			tween.Tween(this, to, 0.07f);
			AddTween(tween, true);
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
					if (player != null && player != parent && !player.Invincible)
				 	{
						canPunch = false;
						World.BroadcastMessage(CameraShake.SHAKE, 10.0f, 0.5f);
				 		Mixer.Audio["hit1"].Play();
				 		
			 			var hsign = FP.Sign(p.X - parent.X);
				 		if (player.Rebounding)
				 		{
				 			parent.OnMessage(PhysicsBody.IMPULSE, (REBOUND_PUNCH_FORCE * Multiplier) * -hsign, REBOUND_PUNCH_FORCE * Multiplier);
				 		}
				 		else
				 		{
				 			p.OnMessage(PhysicsBody.IMPULSE, BASE_PUNCH_FORCE * forceVector.X, PUNCH_BOOST_Y + BASE_PUNCH_FORCE * forceVector.Y);
				 		
//				 			parent.OnMessage(PhysicsBody.IMPULSE, (BASE_PUNCH_FORCE_X * Multiplier) * hsign, BASE_PUNCH_FORCE_Y * Multiplier);
				 		}
				 	}
				}
			 	
			}
			
			MoveTowards(parent.CenterX, parent.CenterY, 20);
			
			X = parent.CenterX + offsetX;
			Y = parent.CenterY + offsetY;
		}
	}
}
