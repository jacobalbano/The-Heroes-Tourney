using System;
using Punk;
using Punk.Graphics;
using Punk.Utils;
using SFML.Window;
using SNHU.Components;

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
			Icon = new Image(Library.GetTexture("assets/fus.png"));
		}
		
		public override void Added()
		{
			base.Added();
			
			FusStrength = float.Parse(GameWorld.gameManager.Config["Fus", "Strength"]);
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
					
					Parent.World.Add(new FusBlast(Parent.X, Parent.Y));
				}
			}
		}
		
		public override EffectMessage MakeEffect()
		{
			EffectMessage.Callback callback = delegate(Entity from, Entity to, float scalar)
			{
				Vector2f dir = new Vector2f(to.X - from.X, to.Y - from.Y)
					.Normalized(FusStrength);
				
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
				
				var i = new Image(Library.GetTexture("assets/fus_active.png"));
				i.Scale = 0.1f;
				i.CenterOO();
				
				AddComponent(i);
				
				Tweener.Tween(i, new { Scale = 50 }, 0.5f)
					.OnComplete(() => World.Remove(this));
			}
		}
	}
}
