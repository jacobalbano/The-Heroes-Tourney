﻿using System;
using GlideTween;
using Punk;
using Punk.Graphics;
using Punk.Utils;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of GroundSmash.
	/// </summary>
	public class GroundSmash : Upgrade
	{
		public float SmashRadius { get; private set; }
		public float FallSpeed { get; private set; }
		
		public GroundSmash()
		{
			Icon = new Image(Library.GetTexture("assets/groundsmash.png"));
			AddResponse(Player.Message.OnLand, OnPlayerLand);
		}
		
		public override void Added()
		{
			base.Added();
			
			SmashRadius = float.Parse(GameWorld.gameManager.Config["GroundSmash", "SmashRadius"]);
			FallSpeed = float.Parse(GameWorld.gameManager.Config["GroundSmash", "FallSpeed"]);
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				if (to.CollideRect(to.X, to.Y, from.X - SmashRadius, from.Y - 10, SmashRadius * 2, 10))
				{
					to.OnMessage(Player.Message.Damage);
				}
			};
			
			return new EffectMessage(owner, callback);
		}
		
		public override void Use()
		{
			if (!Activated && !owner.OnGround)
			{
				base.Use();
				
				owner.physics.OnMessage(PhysicsBody.Message.UseGravity, false);
			}
		}
		
		public override void Update()
		{
			base.Update();
			
			if (Activated)
			{
				Parent.OnMessage(PhysicsBody.Message.Impulse, 0, FallSpeed, true);
			}
		}
		
		public void OnPlayerLand(params object[] args)
		{
			if (Activated)
			{
				var emitter = new Emitter(Library.GetTexture("assets/groundsmash_particle.png"), 3, 3);
				emitter.Relative = false;
				
				for (int i = 0; i < 4; i++)
				{
					var name = i.ToString();
					emitter.NewType(name, FP.Frames(i));
					emitter.SetAlpha(name, 0, 1f);
					emitter.SetMotion(name, 90, 100, 0.5f, 10, 10, 0.1f, Ease.SineOut);
					emitter.SetGravity(name, 5, 2);
				}
				
				var emitterEnt = Parent.World.AddGraphic(emitter, -9010);
				emitterEnt.Tweener.Timer(3)
					.OnComplete(() => FP.World.Remove(emitterEnt));
				
				for (float i = -SmashRadius; i < SmashRadius; i++)
				{
					emitter.Emit(FP.Choose("0", "1", "2", "3"), Parent.X + i + FP.Random - FP.Random, Parent.Y + FP.Rand(10) - 5);
				}
				
				Parent.World.BroadcastMessage(CameraShake.Message.Shake, 100.0f, 0.5f);
				Parent.World.BroadcastMessageIf(e => e != owner, EffectMessage.Message.OnEffect, MakeEffect());
				
				owner.physics.OnMessage(PhysicsBody.Message.UseGravity, true);
				
				owner.SetUpgrade(null);
			}
		}
	}
}
