using System;
using Indigo;
using Indigo.Content;
using Indigo.Core;
using Indigo.Graphics;
using SNHU.Components;
using SNHU.Config.Upgrades;
using SNHU.Systems;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of FUS.
	/// </summary>
	public class FUS : Upgrade
	{
		public float FusStrength { get; private set; }
			
		public FUS()
		{
			Icon = new Image(Library.Get<Texture>("fus.png"));
			FusStrength = Library.Get<FusConfig>("config/upgrades/fus.ini").Strength;
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
					
					Parent.World.Add(new FusBlast(Parent.X, Parent.Y));
				}
			}
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				if (to == from) return; // sender;
				
				var dir = new Point(to.X - from.X, to.Y - from.Y);
				dir.Normalize(FusStrength);
				
				to.OnMessage(PhysicsBody.Message.Impulse, dir.X, dir.Y);
			};
			
			return new EffectMessage(owner, callback);
		}
		
		private class FusBlast : Entity
		{
			public FusBlast(float X, float Y)
			{
				this.X = X;
				this.Y = Y;
				
				var i = new Image(Library.Get<Texture>("fus_active.png"));
				i.Scale = 0.1f;
				i.CenterOrigin();
				
				AddComponent(i);
				
				Tweener.Tween(i, new { Scale = 50 }, 0.5f)
					.OnComplete(() => World.Remove(this));
			}
		}
	}
}
