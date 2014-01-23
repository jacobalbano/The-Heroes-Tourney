using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;
using SFML.Window;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{
	public class Magnet : Upgrade
	{
		const float MAGNET_STRENGTH = 65.0f;
			
		public Magnet()
		{
			Icon = new Image(Library.GetTexture("assets/magnet.png"));
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				Vector2f dir = new Vector2f(to.X - from.X, to.Y - from.Y)
					.Normalized(MAGNET_STRENGTH);
				
				to.OnMessage(PhysicsBody.IMPULSE, -dir.X, -dir.Y);
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
					Parent.World.BroadcastMessageIf(e => e != owner, EffectMessage.ON_EFFECT, MakeEffect());
					Parent.World.BroadcastMessage(CameraShake.SHAKE, 60.0f, 0.5f);
					owner.SetUpgrade(null);
					Mixer.Audio["fus"].Play();
				}
			}
		}
	}
}
