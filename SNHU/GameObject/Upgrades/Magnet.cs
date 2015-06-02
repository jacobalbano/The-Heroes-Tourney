using System;
using Indigo;
using Indigo.Core;
using Indigo.Graphics;
using SNHU.Components;
using SNHU.Config.Upgrades;
using SNHU.Systems;

namespace SNHU.GameObject.Upgrades
{
	public class Magnet : Upgrade
	{
		public float MagnetStrength { get; private set; }
			
		public Magnet()
		{
			Icon = new Image(Library.GetTexture("assets/magnet.png"));
		}
		
		public override void Added()
		{
			base.Added();
			
			MagnetStrength = Library.GetConfig<MagnetConfig>("assets/config/upgrades/magnet.ini").Strength;
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				if (to == from) return; // sender;
				
				var dir = new Point(to.X - from.X, to.Y - from.Y);
				dir.Normalize(MagnetStrength);
				
				to.OnMessage(PhysicsBody.Message.Impulse, -dir.X, -dir.Y);
			};
			
			return new EffectMessage(owner, callback);
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				if (Parent.World != null)
				{
					Parent.World.BroadcastMessage(EffectMessage.Message.OnEffect, MakeEffect());
					Parent.World.BroadcastMessage(CameraManager.Message.Shake, 60.0f, 0.5f);
					owner.SetUpgrade(null);
					Mixer.Fus.Play();
				}
			}
		}
	}
}
