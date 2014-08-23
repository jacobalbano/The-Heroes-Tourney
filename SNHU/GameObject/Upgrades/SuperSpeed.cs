using System;
using System.Collections.Generic;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Utils;
using SNHU.Components;
using SNHU.GameObject.Upgrades.Helper;

//using SFML.Graphics;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of SuperSpeed.
	/// </summary>
	public class SuperSpeed : Upgrade
	{
		public float NewSpeed { get; private set; } // 12.0f;
		
		public SuperSpeed()
		{
			Icon  = new Image(Library.GetTexture("assets/speed.png"));
		}
		
		public override void Added()
		{
			base.Added();
			NewSpeed = float.Parse(GameWorld.gameManager.Config["SuperSpeed", "NewSpeed"]);
			Lifetime = float.Parse(GameWorld.gameManager.Config["SuperSpeed", "Lifetime"]);
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				to.AddComponent(new SuperSpeedEmitter(Lifetime));
				to.OnMessage(Movement.Message.Speed, NewSpeed);
			};
			
			return new EffectMessage(owner, callback);
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				Parent.World.BroadcastMessageIf(e => e != owner, EffectMessage.Message.OnEffect, MakeEffect());	
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			owner.SetUpgrade(null);
		}
	}
}
