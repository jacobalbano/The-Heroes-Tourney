using System;
using Punk;
using Punk.Graphics;
using Punk.Utils;
using SFML.Window;
using SNHU.Components;

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
			
			MagnetStrength = float.Parse(GameWorld.gameManager.Config["Magnet", "Strength"]);
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				Vector2f dir = new Vector2f(to.X - from.X, to.Y - from.Y)
					.Normalized(MagnetStrength);
				
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
					Parent.World.BroadcastMessageIf(e => e != owner, EffectMessage.Message.OnEffect, MakeEffect());
					Parent.World.BroadcastMessage(CameraShake.Message.Shake, 60.0f, 0.5f);
					owner.SetUpgrade(null);
					Mixer.Fus.Play();
				}
			}
		}
	}
}
