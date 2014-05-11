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
		public float FIST_SPEED { get; private set; }
		
		public Vector2f direction;
		public float ForceMultiplier { get; private set; }
		public float FistScale { get; private set; }
		
		private Player owner;
		private Image image;
		private HypeTween hypeTween;
		private Entity emitterEnt;
		private Emitter emitter;
		
		public HyperFist(Player owner)
		{
			FistScale = float.Parse(GameWorld.gameManager.Config["HyperPunch", "FistScale"]);
			ForceMultiplier = float.Parse(GameWorld.gameManager.Config["HyperPunch", "PunchMultiplier"]);
			FIST_SPEED = float.Parse(GameWorld.gameManager.Config["HyperPunch", "FistSpeed"]);
			
			this.owner = owner;
			
			hypeTween = new HypeTween(0.1f);
			AddTween(hypeTween, true);
			
			image = new Image(Library.GetTexture("assets/hyperPunch.png"));
			image.Scale = 0.1f * this.FistScale;
			image.CenterOO();
			Graphic = image;
			
			SetHitbox(image.ScaledWidth, image.ScaledHeight);
			CenterOrigin();
			
			direction = new Vector2f();
			
			emitter = new Emitter(Library.GetTexture("assets/bullet_sparkle.png"), 20, 20);
			emitter.Relative = false;
			
			var name = "spark";
			emitter.NewType(name, FP.Frames(0, 1, 2, 3, 4));
			emitter.SetAlpha(name, 1, 0);
			emitter.SetMotion(name, 0, 0, 0.5f, 0, 0, 0.25f, Ease.CircOut);
			
			AddResponse(ChunkManager.Advance, OnAdvance);
		}
		
		public override void Added()
		{
			base.Added();
			
			emitterEnt = World.AddGraphic(emitter, -9010);
		}
		
		public override void Update()
		{
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
		 				Mixer.Audio["hit1"].Play();
						
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
		 				Mixer.Audio["hit1"].Play();
		 				
		 				var force = ForceMultiplier * Fist.BASE_PUNCH_FORCE;
		 				player.OnMessage(PhysicsBody.IMPULSE, force * direction.X, force * direction.Y);
					}
			 	}
			}
			
			var randX = FP.Rand(50) - 25;
			var randY = FP.Rand(50) - 25;
			emitter.Emit("spark", X + randX, Y + randY);
			
			image.Angle = FP.Angle(0, 0, direction.X, direction.Y);
			
			var moveBy = VectorHelper.Normalized(direction, FIST_SPEED);
			X += moveBy.X;
			Y += moveBy.Y;
		}
		
		public override void Removed()
		{
			base.Removed();
			
			World.AddTween(new Alarm(3, () => FP.World.Remove(emitterEnt), Tween.ONESHOT), true);
			
			for (int i = 0; i < 10; i++)
			{
				var randX = FP.Rand(100) - 50;
				var randY = FP.Rand(100) - 50;
				emitter.Emit("spark", X + randX, Y + randY);		
			}
		}
		
		public override bool MoveCollideX(Entity e)
		{
			if (e == owner)	return false;
				
			return false;
		}
		
		public override bool MoveCollideY(Entity e)
		{
			if (e == owner)	return false;
			
			return false;
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
		
		
		public override EffectMessage MakeEffect()
		{
			throw new NotSupportedException("Don't be using this pls");
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
}
