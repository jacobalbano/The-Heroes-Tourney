/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 5/10/2014
 * Time: 4:46 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{	
	public class HyperFist : Entity
	{
		
		public Vector2f direction;
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
			
			hypeTween = new HypeTween(0.1f);
			AddTween(hypeTween, true);
			
			image = new Image(Library.GetTexture("assets/hyperPunch.png"));
			image.Scale = 0.1f * this.FistScale;
			image.CenterOO();
			Graphic = image;
			
			SetHitbox(image.ScaledWidth, image.ScaledHeight);
			CenterOrigin();
			
			direction = new Vector2f();
			
			AddResponse(ChunkManager.Advance, OnAdvance);
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
						World.BroadcastMessage(CameraShake.SHAKE, 10.0f, 0.5f);
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
						World.BroadcastMessage(CameraShake.SHAKE, 10.0f, 0.5f);
		 				Mixer.Hit1.Play();
		 				
		 				var force = ForceMultiplier * Fist.BASE_PUNCH_FORCE;
		 				player.OnMessage(PhysicsBody.IMPULSE, force * direction.X, force * direction.Y);
					}
			 	}
			}
			
			image.Angle = FP.Angle(0, 0, direction.X, direction.Y);
			
			var moveBy = VectorHelper.Normalized(direction, FistSpeed);
			
			var rainbow = World.Add(new RainbowTrail(X, Y, moveBy, image.Scale, FistSpeed, Layer));
			
			X += moveBy.X;
			Y += moveBy.Y;
			
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
			
			AddResponse(ChunkManager.Advance, OnAdvance);
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
				fist.direction = new Vector2f((Parent as Player).Facing, 0.0f);
				
				
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
		
		public RainbowTrail(float x, float y, Vector2f direction, float scale, float FIST_SPEED, int layer)
		{
			X = x;
			Y = y;
			Layer = layer + 1;
			sineticks = 0;
			
			rainbow = new Image(Library.GetTexture("assets/rainbow.png"));
			Graphic = rainbow;
			rainbow.CenterOO();
			rainbow.Alpha = 0.5f;
			rainbow.Scale = scale;
			rainbow.Angle = FP.Angle(0, 0, direction.X, direction.Y);
			rainbow.ScaleX = rainbow.ScaledWidth / (FIST_SPEED / 2);
			rainbow.ScaleY = 0.75f;
			
			var duration = 0.75f;
			
			Tween.OnComplete complete = () => World.Remove(this);
			var fader = new MultiVarTween(complete, ONESHOT);
			fader.Tween(rainbow, new {Alpha = 0, ScaleY = 0}, duration);
			AddTween(fader, true);
			
//			var mover = new MultiVar5
			
		}
		
		public override void Update()
		{
			base.Update();
			var upndown = 25;
			rainbow.Y = FP.Scale((float) Math.Sin(sineticks += 0.1f), -1, 1, -upndown, upndown);
		}
	}
}
