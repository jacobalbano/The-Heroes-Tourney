using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Rebound.
	/// </summary>
	public class Rebound : Upgrade
	{
		public new enum Message { Set }
		
		private Image shield_1, shield_2;
		
		public Rebound()
		{
			Icon = new Image(Library.GetTexture("assets/rebound.png"));
		}
		
		public override void Added()
		{
			base.Added();
			
			Lifetime = float.Parse(GameWorld.gameManager.Config["Rebound", "Lifetime"]);
		}
		
		public override EffectMessage MakeEffect()
		{
			throw new NotImplementedException();
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				shield_1 = new Image(Library.GetTexture("assets/rebound_active_1.png"));
				shield_2 = new Image(Library.GetTexture("assets/rebound_active_2.png"));
				
				shield_1.CenterOO();
				shield_2.CenterOO();
				
				shield_1.Alpha = 0.0f;
				shield_2.Alpha = 0.0f;
			
				Parent.AddComponent(shield_1);
				Parent.AddComponent(shield_2);
				
				shield_1.Y = 10 - (shield_1.Height / 2);
				shield_2.Y = 10 - (shield_2.Height / 2);
				
				Tweener.Tween(shield_1, new { Alpha = 0.6f }, 0.45f);
				Tweener.Tween(shield_2, new { Alpha = 0.6f }, 0.45f);
				
				owner.OnMessage(Rebound.Message.Set, true);
				
				Mixer.ReboundUp.Play();
				
				var existingCollisions = new List<Entity>();
				Parent.CollideInto(Parent.Type, Parent.X, Parent.Y, existingCollisions);
				foreach (var player in existingCollisions)
				{
					player.OnMessage(PhysicsBody.Message.Impulse, 0, -Fist.BASE_PUNCH_FORCE);
				}
			}
		}
		
		public override void Removed()
		{
			base.Removed();
			owner.OnMessage(Rebound.Message.Set, false);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (shield_1 != null)
			{
				FP.Log(lifeTimer.TimeRemaining, lifeTimer.Completion);
				shield_1.Alpha = 0.6f - (0.6f * lifeTimer.Completion);
				shield_2.Alpha = 0.6f - (0.6f * lifeTimer.Completion);
				
				shield_1.Angle++;
				shield_2.Angle--;
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			Tweener.Tween(shield_1, new { Alpha = 0 }, 0.45f);
			Tweener.Tween(shield_2, new { Alpha = 0 }, 0.45f)
				.OnComplete(OnFadeOutComplete);
		}
		
		public void OnFadeOutComplete()
		{
			Mixer.ReboundDown.Play();	
			owner.SetUpgrade(null);
		}
	}
}
