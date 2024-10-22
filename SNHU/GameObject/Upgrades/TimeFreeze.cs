﻿using System;
using System.Collections.Generic;
using Indigo;
using Indigo.Graphics;
using SNHU.Components;
using SNHU.Config.Upgrades;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of TimeFreeze.
	/// </summary>
	[DisabledInBuild]
	public class TimeFreeze : Upgrade
	{
		public List<Player> players;
		
		public TimeFreeze()
		{
			Icon  = new Image(Library.GetTexture("timeFreeze.png"));
			players = new List<Player>();
		}
		
		public override void Added()
		{
			base.Added();
			Lifetime = Library.GetConfig<TimeFreezeConfig>("config/upgrades/timefreeze.ini").Lifetime;
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				to.AddComponent(new Freeze(to.X, to.Y, Lifetime));
			};
			
			FP.Log("FROZEN FOR ", Lifetime, " SECONDS YO");
			
			return new EffectMessage(owner, callback);
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				Parent.World.BroadcastMessage(EffectMessage.Message.OnEffect, MakeEffect());	
			}
		}
	
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			FP.Log("u can b unfrozen now bro");
			owner.SetUpgrade(null);
		}
	}
	
	public class Freeze : Component
	{
		private float x, y, lifetime;
		
		public Freeze(float x, float y, float lifetime)
		{
			this.x = x;
			this.y = y;
			this.lifetime = lifetime;
			
			AddResponse(ChunkManager.Message.Advance, Remove);
			AddResponse(Player.Message.Die, Remove);
		}
		
		private void Remove(params object[] args)
		{
			Parent.RemoveComponent(this);
		}
		
		public override void Added()
		{
			base.Added();
			Parent.GetComponent<Movement>().Active = false;
			Parent.GetComponent<PhysicsBody>().Active = false;
		}
		
		public override void Removed()
		{
			base.Removed();
			Parent.GetComponent<Movement>().Active = true;
			Parent.GetComponent<PhysicsBody>().Active = true;
		}
		
		public override void Update()
		{
			base.Update();
			lifetime -= FP.Elapsed;
			
			if (lifetime <= 0)
			{
				FP.Log("gone pls");
				Parent.RemoveComponent(this);
				return;
			}
			
			Parent.X = x;
			Parent.Y = y;
		}
	}
}
