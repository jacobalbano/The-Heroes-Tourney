using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
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
		private bool punchy;
		
		public const string Collision = "fist";
		
		private const float BASE_PUNCH_FORCE_X = 25;
		private const float REBOUND_PUNCH_FORCE_X = 40;
		
		private const float BASE_PUNCH_FORCE_Y = -15;
		private const float REBOUND_PUNCH_FORCE_Y = -25;
		
		public const float DEFAULT_PUNCH_MULT = 1;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="hand">True for left hand image, false for right.</param>
		public Fist(bool hand, Player parent)
		{
			Multiplier = DEFAULT_PUNCH_MULT;
			
			left = hand;
			this.parent = parent;
			punchy = false;
			
			image = new Image(Library.GetTexture("assets/glove" + (left ? "2" : "1") + ".png"));
			image.CenterOO();
			Graphic = image;
			
			SetHitboxTo(image);
			CenterOrigin();
			
			Type = Collision;
			
			if (left)
			{
				offsetY = -30;
			}
			else
			{
				offsetY = -20;
			}
			
			FaceRight();
		}
		
		private void ResetFace(bool facing)
		{
			if (facing)
			{
				FaceLeft();
			}
			else
			{
				FaceRight();
			}
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
		
		public void Punch()
		{
			var to = offsetX + (left ? 40 : 50) * (image.FlippedX ? -1 : 1);
			punchy = true;
			
			var tween = new VarTween(() => UnPunch(image.FlippedX), ONESHOT);
			tween.Tween(this, "offsetX", to, 0.05f);
			AddTween(tween, true);
		}
		
		private void UnPunch(bool facing)
		{
			var to = offsetX - (left ? 40 : 50) * (facing ? -1 : 1);
			
			punchy = false;
			
			var tween = new VarTween(() => ResetFace(facing), ONESHOT);
			tween.Tween(this, "offsetX", to, 0.07f);
			AddTween(tween, true);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (punchy)
			{
				var l = new List<Entity>();
				CollideInto(Player.Collision, X, Y, l);
				
				foreach (var p in l)
				{
					var player = p as Player;
					if (player != null && player != parent && !player.Invincible)
				 	{	
						World.BroadcastMessage(CameraShake.SHAKE, 10.0f, 0.5f);
				 		Mixer.Audio["hit1"].Play();
				 		
			 			var hsign = FP.Sign(p.X - parent.X);
				 		if (player.Rebounding)
				 		{
				 			parent.OnMessage(PhysicsBody.IMPULSE, (REBOUND_PUNCH_FORCE_X * Multiplier) * -hsign, REBOUND_PUNCH_FORCE_Y * Multiplier);
				 		}
				 		else
				 		{
				 			p.OnMessage(PhysicsBody.IMPULSE, (BASE_PUNCH_FORCE_X * Multiplier) * hsign, BASE_PUNCH_FORCE_Y * Multiplier);
				 		}
				 	}
				}
			 	
			}
			
			MoveTowards(parent.X, parent.Y, 20);
			
			X = parent.X + offsetX;
			Y = parent.Y + offsetY;
		}
	}
}
