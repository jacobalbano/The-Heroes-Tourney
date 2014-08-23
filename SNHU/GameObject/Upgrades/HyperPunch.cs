using System;
using System.Collections.Generic;
using Indigo;
using Indigo.Core;
using Indigo.Graphics;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{	
	public class HyperFist : Entity
	{
		
		public Point direction;
		public float FistSpeed { get; private set; }
		public float ForceMultiplier { get; private set; }
		public float FistScale { get; private set; }
		
		private Player owner, originalOwner;
		private Image image;
		private HypeTween hypeTween;
		
		public HyperFist(Player owner)
		{
			FistScale = float.Parse(GameWorld.gameManager.Config["HyperPunch", "FistScale"]);
			ForceMultiplier = float.Parse(GameWorld.gameManager.Config["HyperPunch", "PunchMultiplier"]);
			FistSpeed = float.Parse(GameWorld.gameManager.Config["HyperPunch", "FistSpeed"]);
			
			this.owner = originalOwner = owner;
			
			hypeTween = new HypeTween(0.1f, Tweener);
			
			image = new Image(Library.GetTexture("assets/hyperPunch.png"));
			image.Scale = 0.1f * this.FistScale;
			image.CenterOO();
			AddComponent(image);
			
			SetHitbox((int) image.ScaledWidth, (int) image.ScaledHeight);
			CenterOrigin();
			
			direction = new Point();
			
			AddResponse(ChunkManager.Message.Advance, OnAdvance);
		}
		
		public override void Added()
		{
			base.Added();
			
			Layer = -9001;
		}
		
		public override void Removed()
		{
			base.Removed();
			
			originalOwner.SetUpgrade(null);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (!OnCamera)
			{
				var offX = Left > FP.Width || Right < 0;
				var offY = Top > FP.Height || Bottom < 0;
				
				if (offX && offY)
					World.Remove(this);
			}
			
			image.Color = hypeTween.Color;
			
			var l = new List<Entity>();
			CollideInto(Player.Collision, X, Y, l);
			
			foreach (var p in l)
			{
				var player = p as Player;
				
				if (player != owner)
				{
					if (player.Rebounding)
					{
						World.BroadcastMessage(CameraShake.Message.Shake, 10.0f, 0.5f);
		 				Mixer.Hit1.Play();
						
						var angle = FP.Angle(player.X, player.Y, owner.X, owner.Y);
						FP.AngleXY(ref direction.X, ref direction.Y, angle, 1);
						
						owner = player;
					}
				}
					
				if (player != owner)
			 	{
					if (!player.Invincible)
					{
						World.BroadcastMessage(CameraShake.Message.Shake, 10.0f, 0.5f);
		 				Mixer.Hit1.Play();
		 				
		 				var force = ForceMultiplier * Fist.BASE_PUNCH_FORCE;
		 				player.OnMessage(PhysicsBody.Message.Impulse, force * direction.X, force * direction.Y);
					}
			 	}
			}
			
			image.Angle = FP.Angle(0, 0, direction.X, direction.Y);
			
			direction.Normalize(FistSpeed);
			
			var rainbow = World.Add(new RainbowTrail(X, Y, direction, image.Scale, FistSpeed, Layer));
			
			X += direction.X;
			Y += direction.Y;
			
		}
		
		private void OnAdvance(params object[] args)
		{
			World.Remove(this);
		}
	}
	
	/// <summary>
	/// Description of HyperPunch.
	/// </summary>
	public class HyperPunch : Upgrade
	{	
		public float PunchMult { get; private set; }
		public float FistScale { get; private set; }
		private HyperFist fist;
		public const float PICKUP_IMAGE_SCALE = 1.5f;
		
		public HyperPunch()
		{
			Icon = new Image(Library.GetTexture("assets/hyperPunch.png"));
			Icon.Scale = 0.1f * PICKUP_IMAGE_SCALE;
			
			AddResponse(ChunkManager.Message.Advance, OnAdvance);
		}
		
		public override void Added()
		{
			base.Added();
			
			fist = new HyperFist(Parent as Player);
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				fist.X = (Parent as Player).Facing == -1 ? Parent.Left : Parent.Right;
				fist.Y = Parent.CenterY;
				fist.direction = new Point((Parent as Player).Facing, 0.0f);
				
				
				Parent.World.Add(fist);
			}
		}
		
		public override void Removed()
		{
			base.Removed();
			
			Parent.World.Remove(fist);
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			if (Parent != null)
			{
				if (Parent.World != null)
				{
					Parent.World.Remove(fist);
				}
				
				owner.SetUpgrade(null);
			}
		}
		
		private void OnAdvance(params object[] args)
		{
			if (Activated)
				OnLifetimeComplete();
		}
	}
	
	class RainbowTrail : Entity
	{
		private float sineticks;
		private Image rainbow;
		
		public RainbowTrail(float x, float y, Point direction, float scale, float FIST_SPEED, int layer)
		{
			X = x;
			Y = y;
			Layer = layer + 1;
			sineticks = 0;
			
			rainbow = new Image(Library.GetTexture("assets/rainbow.png"));
			rainbow.CenterOO();
			rainbow.Alpha = 0.5f;
			rainbow.Scale = scale;
			rainbow.Angle = FP.Angle(0, 0, direction.X, direction.Y);
			rainbow.ScaleX = rainbow.ScaledWidth / (FIST_SPEED / 2);
			rainbow.ScaleY = 0.75f;
			AddComponent(rainbow);
			
			var duration = 0.75f;
			
			Tweener.Tween(rainbow, new {Alpha = 0, ScaleY = 0}, duration)
				.OnComplete(() => World.Remove(this));
			
		}
		
		public override void Update()
		{
			base.Update();
			var upndown = 25;
			rainbow.Y = FP.Scale((float) Math.Sin(sineticks += 0.1f), -1, 1, -upndown, upndown);
		}
	}
}
